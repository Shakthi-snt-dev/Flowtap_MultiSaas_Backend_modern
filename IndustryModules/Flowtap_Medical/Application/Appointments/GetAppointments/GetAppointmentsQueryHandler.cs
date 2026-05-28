using Flowtap_Application.Common.DTOs;
using Flowtap_Medical.DbContext;
using Flowtap_Medical.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Medical.Application.Appointments.GetAppointments;

public class GetAppointmentsQueryHandler(IMedicalDbContext db)
    : IRequestHandler<GetAppointmentsQuery, Result<List<AppointmentDto>>>
{
    public async Task<Result<List<AppointmentDto>>> Handle(GetAppointmentsQuery request, CancellationToken ct)
    {
        var query = db.Appointments
            .Include(a => a.Patient)
            .Where(a => a.CompanyId == request.CompanyId);

        if (request.LocationId.HasValue)
            query = query.Where(a => a.LocationId == request.LocationId.Value);

        if (request.PatientId.HasValue)
            query = query.Where(a => a.PatientId == request.PatientId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<AppointmentStatus>(request.Status, true, out var status))
            query = query.Where(a => a.Status == status);

        if (request.Date.HasValue)
        {
            var day = request.Date.Value.Date;
            query = query.Where(a => a.ScheduledAt.Date == day);
        }

        var appointments = await query
            .OrderBy(a => a.ScheduledAt)
            .Select(a => new AppointmentDto(
                a.Id, a.LocationId, a.PatientId, a.Patient.Name, a.Patient.PatientNumber,
                a.DoctorEmployeeId, a.AppointmentNumber, a.ScheduledAt,
                a.Type.ToString(), a.Status.ToString(), a.Reason, a.DurationMinutes, a.CreatedAt))
            .ToListAsync(ct);

        return Result<List<AppointmentDto>>.Success(appointments);
    }
}
