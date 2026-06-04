using Flowtap_Application.Common.DTOs;
using Flowtap_Repair.Application.Reports.DTOs;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Reports;

public class GetTechnicianPerformanceQueryHandler(IRepairDbContext db)
    : IRequestHandler<GetTechnicianPerformanceQuery, Result<TechnicianPerformanceDto>>
{
    public async Task<Result<TechnicianPerformanceDto>> Handle(GetTechnicianPerformanceQuery request, CancellationToken ct)
    {
        var query = db.ServiceTickets
            .Where(t => t.CompanyId == request.CompanyId
                     && t.ExecutorEmployeeId != null
                     && t.CreatedAt >= request.From
                     && t.CreatedAt < request.To.AddDays(1));

        if (request.LocationId.HasValue)
            query = query.Where(t => t.LocationId == request.LocationId.Value);

        var tickets = await query
            .Select(t => new { t.ExecutorEmployeeId, t.Status, t.CreatedAt, t.ClosedAt, t.SaleId })
            .ToListAsync(ct);

        var employeeIds = tickets.Select(t => t.ExecutorEmployeeId!.Value).Distinct().ToList();

        // Employee name is stored in UserProfile — join via UserAccountId
        var employees = await db.Employees
            .Where(e => employeeIds.Contains(e.Id))
            .Join(db.UserProfiles,
                  e => e.UserAccountId,
                  p => p.UserAccountId,
                  (e, p) => new { e.Id, p.Name })
            .ToDictionaryAsync(e => e.Id, ct);

        var ticketSaleIds = tickets.Where(t => t.SaleId.HasValue).Select(t => t.SaleId!.Value).ToList();
        var salesRevenue = await db.Sales
            .Where(s => ticketSaleIds.Contains(s.Id))
            .Select(s => new { s.Id, s.TotalAmount, s.TicketId })
            .ToListAsync(ct);
        var revenueByTicketId = salesRevenue
            .Where(s => s.TicketId.HasValue)
            .GroupBy(s => s.TicketId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(s => s.TotalAmount));

        var techSummaries = tickets
            .GroupBy(t => t.ExecutorEmployeeId!.Value)
            .Select(g =>
            {
                var closed = g.Where(t => t.Status == TicketStatus.Done && t.ClosedAt.HasValue).ToList();
                var avgDays = closed.Count > 0
                    ? closed.Average(t => (t.ClosedAt!.Value - t.CreatedAt).TotalDays)
                    : 0;
                var revenue = g
                    .Where(t => t.SaleId.HasValue)
                    .Sum(t => revenueByTicketId.TryGetValue(t.SaleId!.Value, out var r) ? r : 0);
                var empName = employees.TryGetValue(g.Key, out var emp) ? emp.Name : "Unknown";

                return new TechSummaryDto(g.Key, empName, closed.Count, Math.Round(avgDays, 1), revenue);
            })
            .OrderByDescending(t => t.TicketsClosed)
            .ToList();

        return Result<TechnicianPerformanceDto>.Success(new TechnicianPerformanceDto(request.From, request.To, techSummaries));
    }
}
