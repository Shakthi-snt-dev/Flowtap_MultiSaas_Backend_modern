using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Queries.GetTicket;

public class GetTicketQueryHandler(IRepairDbContext db)
    : IRequestHandler<GetTicketQuery, Result<TicketDto>>
{
    public async Task<Result<TicketDto>> Handle(GetTicketQuery request, CancellationToken ct)
    {
        var t = await db.ServiceTickets
            .Include(x => x.Items)
            .Include(x => x.TimeLogs)
            .FirstOrDefaultAsync(x => x.Id == request.TicketId && x.CompanyId == request.CompanyId, ct)
            ?? throw new NotFoundException("ServiceTicket", request.TicketId);

        // Load payments linked to this ticket (advances recorded before sale creation)
        var payments = await db.Payments
            .Where(p => p.TicketId == request.TicketId && p.CompanyId == request.CompanyId)
            .OrderBy(p => p.PaidAt)
            .ToListAsync(ct);

        // Resolve client details
        var clientName = "Walk-in Customer";
        var clientPhone = string.Empty;
        var clientEmail = string.Empty;
        var client = await db.Clients.FirstOrDefaultAsync(c => c.Id == t.ClientId, ct);
        if (client != null)
        {
            clientName = client.Name;
            clientPhone = client.Phone ?? string.Empty;
            clientEmail = client.Email ?? string.Empty;
        }

        // Resolve employee details (technician and manager)
        var employeeIds = new List<Guid>();
        if (t.ExecutorEmployeeId.HasValue) employeeIds.Add(t.ExecutorEmployeeId.Value);
        if (t.ManagerEmployeeId.HasValue) employeeIds.Add(t.ManagerEmployeeId.Value);

        var employeeMap = await db.Employees
            .Where(e => employeeIds.Contains(e.Id))
            .Join(db.UserProfiles,
                  e => e.UserAccountId,
                  p => p.UserAccountId,
                  (e, p) => new { e.Id, p.Name })
            .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

        var technicianName = t.ExecutorEmployeeId.HasValue && employeeMap.TryGetValue(t.ExecutorEmployeeId.Value, out var techName) ? techName : null;
        var managerName = t.ManagerEmployeeId.HasValue && employeeMap.TryGetValue(t.ManagerEmployeeId.Value, out var mgrName) ? mgrName : null;

        // Resolve brand and model names if stored as GUIDs
        var brandName = t.DeviceDetails?.Brand;
        if (Guid.TryParse(brandName, out var brandGuid))
        {
            var brand = await db.DeviceBrands.FirstOrDefaultAsync(b => b.Id == brandGuid, ct);
            if (brand != null) brandName = brand.Name;
        }

        var modelName = t.DeviceDetails?.Model;
        if (Guid.TryParse(modelName, out var modelGuid))
        {
            var model = await db.DeviceModels.FirstOrDefaultAsync(m => m.Id == modelGuid, ct);
            if (model != null) modelName = model.Name;
        }

        var dto = new TicketDto(
            t.Id, t.CompanyId, t.LocationId, t.ClientId,
            t.TicketNumber,
            t.Type.ToString(), t.Status.ToString(), t.Priority.ToString(),
            t.PrimaryServiceId,
            t.ExecutorEmployeeId, t.ManagerEmployeeId,
            t.CreatedAt, t.Deadline, t.ClosedAt,
            // Device details
            t.DeviceDetails?.DeviceType,
            brandName,
            modelName,
            t.DeviceDetails?.Serial,
            t.DeviceDetails?.Modification,
            t.DeviceDetails?.Appearance,
            t.DeviceDetails?.Password,
            t.DeviceDetails?.Equipment,
            // Notes & checklists
            t.Reason, t.MastersNotes, t.PreRepairChecklist, t.AccessoryList,
            // Financials
            t.Financials?.EstimatedCost ?? 0,
            t.Financials?.Prepayment ?? 0,
            t.Financials?.TotalCost ?? 0,
            t.Financials?.IsPaid ?? false,
            t.Financials?.PrepaymentMethod,
            t.Financials?.PrepaymentPaidAt,
            // Related sale
            t.SaleId,
            // Timer
            t.TimeLogs.Any(l => l.StoppedAt == null),
            (long)t.TimeLogs.Sum(l => ((l.StoppedAt ?? DateTime.UtcNow) - l.StartedAt).TotalSeconds),
            // Items
            t.Items.Select(i => new TicketItemDto(
                i.Id, i.ItemReferenceId, i.Name, i.Type.ToString(),
                i.Quantity, i.Price, i.Cost, i.DiscountAmount, i.TaxPercent
            )).ToList(),
            // Payments
            payments.Select(p => new TicketPaymentDto(
                p.Id, p.Amount, p.Method.ToString(), p.Purpose.ToString(),
                p.ExternalReference, p.Comment, p.PaidAt, p.SaleId
            )).ToList(),
            clientName,
            clientPhone,
            clientEmail,
            technicianName,
            managerName
        );

        return Result<TicketDto>.Success(dto);
    }
}

