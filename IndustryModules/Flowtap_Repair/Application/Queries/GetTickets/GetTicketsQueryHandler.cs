using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Queries.GetTickets;

public class GetTicketsQueryHandler(IRepairDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetTicketsQuery, Result<PaginatedList<TicketListDto>>>
{
    public async Task<Result<PaginatedList<TicketListDto>>> Handle(GetTicketsQuery request, CancellationToken ct)
    {
        var query = db.ServiceTickets
            .Include(t => t.TimeLogs)
            .Where(t => t.CompanyId == request.CompanyId);

        var locationId = request.LocationId ?? currentUser.StoreId;
        if (locationId.HasValue && locationId.Value != Guid.Empty)
            query = query.Where(t => t.LocationId == locationId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var statusStr = request.Status.Trim();
            if (statusStr.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(t => t.Status == Flowtap_Repair.Domain.Enums.TicketStatus.New);
            }
            else if (statusStr.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(t => t.Status == Flowtap_Repair.Domain.Enums.TicketStatus.Done);
            }
            else if (statusStr.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(t => t.Status == Flowtap_Repair.Domain.Enums.TicketStatus.Canceled);
            }
            else
            {
                var normalizedStatus = statusStr.Replace(" ", string.Empty);
                if (Enum.TryParse<Flowtap_Repair.Domain.Enums.TicketStatus>(normalizedStatus, true, out var statusEnum))
                {
                    query = query.Where(t => t.Status == statusEnum);
                }
            }
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        // Bulk resolve Client names
        var clientIds = items.Select(t => t.ClientId).Distinct().ToList();
        var clientMap = await db.Clients
            .Where(c => clientIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, ct);

        // Bulk resolve Employee (technician / manager) names
        var employeeIds = items.Select(t => t.ExecutorEmployeeId)
            .Concat(items.Select(t => t.ManagerEmployeeId))
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();
        var employeeMap = await db.Employees
            .Where(e => employeeIds.Contains(e.Id))
            .Join(db.UserProfiles,
                  e => e.UserAccountId,
                  p => p.UserAccountId,
                  (e, p) => new { e.Id, p.Name })
            .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

        // Bulk resolve Brand names if stored as GUIDs
        var brandStrings = items.Select(t => t.DeviceDetails?.Brand).Where(b => !string.IsNullOrEmpty(b)).Distinct().ToList();
        var brandGuids = brandStrings.Where(b => Guid.TryParse(b, out _)).Select(b => Guid.Parse(b!)).Distinct().ToList();
        var brandMap = await db.DeviceBrands
            .Where(b => brandGuids.Contains(b.Id))
            .ToDictionaryAsync(b => b.Id.ToString(), b => b.Name, ct);

        // Bulk resolve Model names if stored as GUIDs
        var modelStrings = items.Select(t => t.DeviceDetails?.Model).Where(m => !string.IsNullOrEmpty(m)).Distinct().ToList();
        var modelGuids = modelStrings.Where(m => Guid.TryParse(m, out _)).Select(m => Guid.Parse(m!)).Distinct().ToList();
        var modelMap = await db.DeviceModels
            .Where(m => modelGuids.Contains(m.Id))
            .ToDictionaryAsync(m => m.Id.ToString(), m => m.Name, ct);

        string? ResolveBrand(string? brandInput)
        {
            if (string.IsNullOrEmpty(brandInput)) return brandInput;
            return brandMap.TryGetValue(brandInput, out var resolved) ? resolved : brandInput;
        }

        string? ResolveModel(string? modelInput)
        {
            if (string.IsNullOrEmpty(modelInput)) return modelInput;
            return modelMap.TryGetValue(modelInput, out var resolved) ? resolved : modelInput;
        }

        var dtos = items.Select(t => new TicketListDto(
            t.Id, t.ClientId, t.TicketNumber,
            t.Type.ToString(), t.Status.ToString(), t.Priority.ToString(),
            ResolveBrand(t.DeviceDetails?.Brand), ResolveModel(t.DeviceDetails?.Model), t.DeviceDetails?.Serial,
            t.ExecutorEmployeeId, t.ManagerEmployeeId,
            t.CreatedAt, t.Deadline,
            t.Financials?.EstimatedCost ?? 0, t.Financials?.TotalCost ?? 0, t.Financials?.IsPaid ?? false,
            t.TimeLogs.Any(l => l.StoppedAt == null),
            (long)t.TimeLogs.Sum(l => ((l.StoppedAt ?? DateTime.UtcNow) - l.StartedAt).TotalSeconds),
            clientMap.TryGetValue(t.ClientId, out var cName) ? cName : "Walk-in Customer",
            t.ExecutorEmployeeId.HasValue && employeeMap.TryGetValue(t.ExecutorEmployeeId.Value, out var tName) ? tName : null,
            t.ManagerEmployeeId.HasValue && employeeMap.TryGetValue(t.ManagerEmployeeId.Value, out var mName) ? mName : null
        )).ToList();

        return Result<PaginatedList<TicketListDto>>.Success(
            new PaginatedList<TicketListDto>(dtos, total, request.Page, request.PageSize));
    }
}

