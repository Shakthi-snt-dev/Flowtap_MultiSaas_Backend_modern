using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using Flowtap_Repair.Domain.Entities;
using Flowtap_Repair.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Commands.UpdateTicketStatus;

public class UpdateTicketStatusCommandHandler(IRepairDbContext db, IDateTimeService dateTime)
    : IRequestHandler<UpdateTicketStatusCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateTicketStatusCommand request, CancellationToken ct)
    {
        var normalizedStatus = request.NewStatus?.Replace(" ", string.Empty);
        if (!Enum.TryParse<TicketStatus>(normalizedStatus, true, out var newStatus))
            return Result<bool>.Failure($"Invalid status: {request.NewStatus}");

        var ticket = await db.ServiceTickets
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.Id == request.TicketId && t.CompanyId == request.CompanyId, ct)
            ?? throw new NotFoundException("ServiceTicket", request.TicketId);

        var old = ticket.Status;
        ticket.Status = newStatus;

        if (newStatus is TicketStatus.Done or TicketStatus.Canceled)
            ticket.ClosedAt = dateTime.UtcNow;

        db.ActivityLogs.Add(new ActivityLog
        {
            CompanyId  = request.CompanyId,
            EntityType = ActivityEntityType.Ticket,
            EntityId   = ticket.Id,
            Action     = "StatusChanged",
            OldValue   = old.ToString(),
            NewValue   = newStatus.ToString(),
            Details    = request.Notes != null ? System.Text.Json.JsonSerializer.Serialize(new { notes = request.Notes }) : null,
            EmployeeId = Guid.Empty
        });

        await db.SaveChangesAsync(ct);

        // ── Auto-create Sale when ticket transitions to Done ───────────────────────
        // Only create if the ticket has items AND hasn't been linked to a sale yet.
        if (newStatus == TicketStatus.Done && ticket.SaleId == null && ticket.Items.Any())
        {
            await CreateSaleFromTicketAsync(ticket, request.CompanyId, ct);
        }

        return Result<bool>.Success(true);
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Builds a Sale from the ticket's line items (services / parts / products) and
    // links any previously recorded advance Payments to the new Sale.
    // ─────────────────────────────────────────────────────────────────────────────
    private async Task CreateSaleFromTicketAsync(
        ServiceTicket ticket, Guid companyId, CancellationToken ct)
    {
        // 1. Map ticket items → SaleItems
        var saleItems = ticket.Items.Select(i =>
        {
            var saleItemType = i.Type switch
            {
                TicketItemType.Service => SaleItemType.Service,
                TicketItemType.Part    => SaleItemType.Part,
                _                      => SaleItemType.Product,
            };

            var lineNet   = i.Price * i.Quantity - i.DiscountAmount;
            var lineTotal = lineNet * (1 + i.TaxPercent / 100);

            return new SaleItem
            {
                ProductId       = i.ItemReferenceId,
                ProductName     = i.Name,
                Type            = saleItemType,
                Quantity        = i.Quantity,
                UnitPrice       = i.Price,
                TaxPercent      = i.TaxPercent,
                DiscountPercent = 0,
                DiscountAmount  = i.DiscountAmount,
                Total           = Math.Round(lineTotal, 2),
            };
        }).ToList();

        // 2. Compute totals from items
        var subTotal      = Math.Round(saleItems.Sum(i => i.UnitPrice * i.Quantity), 2);
        var discountTotal = Math.Round(saleItems.Sum(i => i.DiscountAmount), 2);
        var taxAmount     = Math.Round(saleItems.Sum(i => (i.UnitPrice * i.Quantity - i.DiscountAmount) * i.TaxPercent / 100), 2);
        var total         = Math.Round(subTotal - discountTotal + taxAmount, 2);

        // Fall back to Financials.TotalCost if items have no prices set
        if (total == 0 && (ticket.Financials?.TotalCost ?? 0) > 0)
            total = ticket.Financials!.TotalCost;

        // 3. Load any advance payments already recorded on this ticket
        var advancePayments = await db.Payments
            .Where(p => p.TicketId == ticket.Id && p.CompanyId == companyId)
            .ToListAsync(ct);

        var totalAdvance = advancePayments.Sum(p => p.Amount);

        // 4. Build transaction number
        var count = await db.Sales.CountAsync(s => s.CompanyId == companyId, ct);
        var txNumber = $"INV-{count + 1:D6}";

        // 5. Determine initial status
        var status = totalAdvance >= total ? SaleStatus.Completed : SaleStatus.Draft;

        // 6. Create the Sale — do NOT add to context yet (we resolve accounts first below)
        var history = new List<SaleHistory>
        {
            new() { Message = $"Sale {txNumber} created from ticket {ticket.TicketNumber}. Total: {total:C}.", CreatedAt = dateTime.UtcNow }
        };

        if (totalAdvance > 0)
            history.Add(new SaleHistory { Message = $"Advance of {totalAdvance:C} applied from ticket {ticket.TicketNumber}.", CreatedAt = dateTime.UtcNow });

        if (status == SaleStatus.Completed)
            history.Add(new SaleHistory { Message = "Sale completed — fully paid via advance.", CreatedAt = dateTime.UtcNow });
        else if (totalAdvance > 0)
            history.Add(new SaleHistory { Message = $"Remaining balance: {total - totalAdvance:C}.", CreatedAt = dateTime.UtcNow });

        var sale = new Sale
        {
            CompanyId      = companyId,
            LocationId     = ticket.LocationId,
            ClientId       = ticket.ClientId,
            TransactionNumber = txNumber,
            Source         = SaleSource.Ticket,
            TicketId       = ticket.Id,
            SubTotal       = subTotal,
            DiscountAmount = discountTotal,
            TaxAmount      = taxAmount,
            TotalAmount    = total,
            Status         = status,
            Items          = saleItems,
            History        = history,
        };

        db.Sales.Add(sale);
        await db.SaveChangesAsync(ct);

        // 7. Link existing advance payments to the new Sale
        foreach (var payment in advancePayments)
            payment.SaleId = sale.Id;

        // 8. Update ticket: link sale + mark paid if completed
        ticket.SaleId = sale.Id;
        if (status == SaleStatus.Completed && ticket.Financials is not null)
            ticket.Financials.IsPaid = true;

        await db.SaveChangesAsync(ct);
    }
}
