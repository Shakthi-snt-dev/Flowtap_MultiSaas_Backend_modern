using Flowtap_Application.Common.DTOs;
using Flowtap_Repair.Application.Reports.DTOs;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Reports;

public class GetTicketStatusReportQueryHandler(IRepairDbContext db)
    : IRequestHandler<GetTicketStatusReportQuery, Result<TicketStatusReportDto>>
{
    public async Task<Result<TicketStatusReportDto>> Handle(GetTicketStatusReportQuery request, CancellationToken ct)
    {
        var query = db.ServiceTickets
            .Where(t => t.CompanyId == request.CompanyId
                     && t.CreatedAt >= request.From
                     && t.CreatedAt < request.To.AddDays(1));

        if (request.LocationId.HasValue)
            query = query.Where(t => t.LocationId == request.LocationId.Value);

        var tickets = await query
            .Select(t => new { t.Status, t.CreatedAt, t.ClosedAt })
            .ToListAsync(ct);

        var closedWithTime = tickets
            .Where(t => t.Status == TicketStatus.Done && t.ClosedAt.HasValue)
            .ToList();
        var avgResolutionDays = closedWithTime.Count > 0
            ? closedWithTime.Average(t => (t.ClosedAt!.Value - t.CreatedAt).TotalDays)
            : 0;

        var days = Enumerable.Range(0, (request.To - request.From).Days + 1)
            .Select(d => request.From.Date.AddDays(d))
            .Select(date => new DailyTicketCountDto(
                date,
                tickets.Count(t => t.CreatedAt.Date == date),
                tickets.Count(t => t.ClosedAt.HasValue && t.ClosedAt.Value.Date == date)))
            .ToList();

        return Result<TicketStatusReportDto>.Success(new TicketStatusReportDto(
            From: request.From,
            To: request.To,
            Total: tickets.Count,
            Open: tickets.Count(t => t.Status == TicketStatus.New),
            InProgress: tickets.Count(t => t.Status == TicketStatus.InProgress),
            WaitingForParts: tickets.Count(t => t.Status == TicketStatus.WaitingForParts),
            Done: tickets.Count(t => t.Status == TicketStatus.Done),
            Cancelled: tickets.Count(t => t.Status == TicketStatus.Canceled),
            AvgResolutionDays: Math.Round(avgResolutionDays, 1),
            ByDay: days
        ));
    }
}
