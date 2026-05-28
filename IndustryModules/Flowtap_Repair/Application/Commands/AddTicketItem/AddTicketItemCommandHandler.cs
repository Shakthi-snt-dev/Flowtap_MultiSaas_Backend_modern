using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Domain.Entities;
using Flowtap_Repair.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Commands.AddTicketItem;

public class AddTicketItemCommandHandler(IRepairDbContext db)
    : IRequestHandler<AddTicketItemCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddTicketItemCommand request, CancellationToken ct)
    {
        if (request.Quantity <= 0)
            return Result<Guid>.Failure("Quantity must be greater than zero.");

        if (!Enum.TryParse<TicketItemType>(request.Type, true, out var itemType))
            itemType = TicketItemType.Service;

        var ticket = await db.ServiceTickets
            .FirstOrDefaultAsync(t => t.Id == request.TicketId && t.CompanyId == request.CompanyId, ct)
            ?? throw new NotFoundException("ServiceTicket", request.TicketId);

        // Add line item
        var item = new ServiceTicketItem
        {
            TicketId        = request.TicketId,
            ItemReferenceId = request.ItemReferenceId,
            Name            = request.Name,
            Type            = itemType,
            Quantity        = request.Quantity,
            Price           = request.Price,
            Cost            = 0,
            DiscountAmount  = request.DiscountAmount,
            TaxPercent      = request.TaxPercent,
        };
        db.ServiceTicketItems.Add(item);

        // Update ticket TotalCost
        var lineNet   = request.Price * request.Quantity - request.DiscountAmount;
        var lineTotal = lineNet * (1 + request.TaxPercent / 100);

        if (ticket.Financials is null)
            ticket.Financials = new ServiceFinancials();

        ticket.Financials.TotalCost = Math.Round(ticket.Financials.TotalCost + lineTotal, 2);

        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(item.Id);
    }
}

