using Flowtap_Application.Common.DTOs;
using Flowtap_Medical.Application.Reports.DTOs;
using Flowtap_Medical.DbContext;
using Flowtap_Medical.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Medical.Application.Reports;

public class GetMedicalDashboardQueryHandler(IMedicalDbContext db)
    : IRequestHandler<GetMedicalDashboardQuery, Result<MedicalDashboardDto>>
{
    public async Task<Result<MedicalDashboardDto>> Handle(GetMedicalDashboardQuery request, CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        var monthStart = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var apptQuery = db.Appointments.Where(a => a.CompanyId == request.CompanyId);
        if (request.LocationId.HasValue)
            apptQuery = apptQuery.Where(a => a.LocationId == request.LocationId.Value);

        var todayAppts = await apptQuery
            .Where(a => a.ScheduledAt >= today && a.ScheduledAt < tomorrow)
            .Select(a => new { a.Status })
            .ToListAsync(ct);

        var totalPatients = await db.Patients.CountAsync(p => p.CompanyId == request.CompanyId, ct);

        var salesQuery = db.Sales.Where(s => s.CompanyId == request.CompanyId);
        if (request.LocationId.HasValue)
            salesQuery = salesQuery.Where(s => s.LocationId == request.LocationId.Value);

        var todayRevenue  = await salesQuery.Where(s => s.CreatedAt >= today && s.CreatedAt < tomorrow).SumAsync(s => s.TotalAmount, ct);
        var monthRevenue  = await salesQuery.Where(s => s.CreatedAt >= monthStart).SumAsync(s => s.TotalAmount, ct);

        return Result<MedicalDashboardDto>.Success(new MedicalDashboardDto(
            TotalPatients: totalPatients,
            TodayAppointments: todayAppts.Count,
            PendingAppointments: todayAppts.Count(a => a.Status == AppointmentStatus.Scheduled),
            CompletedAppointments: todayAppts.Count(a => a.Status == AppointmentStatus.Completed),
            TodayRevenue: todayRevenue,
            MonthRevenue: monthRevenue
        ));
    }
}
