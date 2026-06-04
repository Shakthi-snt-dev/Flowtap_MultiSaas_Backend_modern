using Flowtap_Application.Common.DTOs;
using Flowtap_Repair.Application.Reports.DTOs;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Reports;

public class GetRepairDashboardQueryHandler(IRepairDbContext db)
    : IRequestHandler<GetRepairDashboardQuery, Result<RepairDashboardDto>>
{
    public async Task<Result<RepairDashboardDto>> Handle(GetRepairDashboardQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var query = db.ServiceTickets.Where(t => t.CompanyId == request.CompanyId && t.IsActive);
        if (request.LocationId.HasValue)
            query = query.Where(t => t.LocationId == request.LocationId.Value);

        var tickets = await query
            .Select(t => new { t.Status, t.Deadline, t.SaleId, t.ClosedAt })
            .ToListAsync(ct);

        var openStatuses = new[] { TicketStatus.New, TicketStatus.InProgress, TicketStatus.WaitingForParts };

        var revenueQuery = db.Sales.Where(s => s.CompanyId == request.CompanyId
                                            && s.TicketId != null
                                            && s.CreatedAt >= monthStart);
        if (request.LocationId.HasValue)
            revenueQuery = revenueQuery.Where(s => s.LocationId == request.LocationId.Value);
        var revenueThisMonth = await revenueQuery.SumAsync(s => s.TotalAmount, ct);

        var statusBreakdown = tickets
            .GroupBy(t => t.Status.ToString())
            .Select(g => new TicketsByStatusDto(g.Key, g.Count()))
            .ToList();

        return Result<RepairDashboardDto>.Success(new RepairDashboardDto(
            OpenTickets: tickets.Count(t => openStatuses.Contains(t.Status)),
            PendingPickup: tickets.Count(t => t.Status == TicketStatus.Ready),
            OverdueTickets: tickets.Count(t => openStatuses.Contains(t.Status)
                                            && t.Deadline.HasValue
                                            && t.Deadline.Value < now),
            ClosedThisMonth: tickets.Count(t => t.Status == TicketStatus.Done
                                             && t.ClosedAt.HasValue
                                             && t.ClosedAt.Value >= monthStart),
            RevenueThisMonth: revenueThisMonth,
            StatusBreakdown: statusBreakdown
        ));
    }
}
