using Flowtap_Application.Common.DTOs;
using Flowtap_Medical.Application.Reports.DTOs;
using Flowtap_Medical.DbContext;
using Flowtap_Medical.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Medical.Application.Reports;

public class GetPatientVisitReportQueryHandler(IMedicalDbContext db)
    : IRequestHandler<GetPatientVisitReportQuery, Result<PatientVisitReportDto>>
{
    public async Task<Result<PatientVisitReportDto>> Handle(GetPatientVisitReportQuery request, CancellationToken ct)
    {
        var apptQuery = db.Appointments
            .Where(a => a.CompanyId == request.CompanyId
                     && a.ScheduledAt >= request.From
                     && a.ScheduledAt < request.To.AddDays(1)
                     && a.Status == AppointmentStatus.Completed);
        if (request.LocationId.HasValue)
            apptQuery = apptQuery.Where(a => a.LocationId == request.LocationId.Value);

        var appointments = await apptQuery
            .Select(a => new { a.PatientId, a.ScheduledAt })
            .ToListAsync(ct);

        // A patient is "new" if their first appointment is in this date range
        var patientIds = appointments.Select(a => a.PatientId).Distinct().ToList();
        var firstVisitDates = await db.Appointments
            .Where(a => patientIds.Contains(a.PatientId) && a.Status == AppointmentStatus.Completed)
            .GroupBy(a => a.PatientId)
            .Select(g => new { PatientId = g.Key, FirstVisit = g.Min(a => a.ScheduledAt) })
            .ToDictionaryAsync(g => g.PatientId, g => g.FirstVisit, ct);

        var newPatientIds = firstVisitDates
            .Where(kvp => kvp.Value >= request.From && kvp.Value < request.To.AddDays(1))
            .Select(kvp => kvp.Key)
            .ToHashSet();

        var days = Enumerable.Range(0, (request.To.Date - request.From.Date).Days + 1)
            .Select(d => request.From.Date.AddDays(d))
            .Select(date =>
            {
                var dayAppts = appointments.Where(a => a.ScheduledAt.Date == date).ToList();
                return new DailyVisitDto(date, dayAppts.Count, dayAppts.Count(a => newPatientIds.Contains(a.PatientId)));
            })
            .ToList();

        return Result<PatientVisitReportDto>.Success(new PatientVisitReportDto(
            From: request.From,
            To: request.To,
            TotalVisits: appointments.Count,
            NewPatients: appointments.Count(a => newPatientIds.Contains(a.PatientId)),
            RepeatPatients: appointments.Count(a => !newPatientIds.Contains(a.PatientId)),
            ByDay: days
        ));
    }
}
