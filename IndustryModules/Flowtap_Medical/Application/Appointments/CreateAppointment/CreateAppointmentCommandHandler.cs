using Flowtap_Application.Common.DTOs;
using Flowtap_Medical.DbContext;
using Flowtap_Medical.Domain.Entities;
using Flowtap_Medical.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Medical.Application.Appointments.CreateAppointment;

public class CreateAppointmentCommandHandler(IMedicalDbContext db)
    : IRequestHandler<CreateAppointmentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateAppointmentCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<AppointmentType>(request.Type, true, out var type))
            type = AppointmentType.Consultation;

        var count = await db.Appointments.CountAsync(a => a.CompanyId == request.CompanyId, ct);

        var appointment = new Appointment
        {
            CompanyId          = request.CompanyId,
            LocationId         = request.LocationId,
            PatientId          = request.PatientId,
            DoctorEmployeeId   = request.DoctorEmployeeId,
            AppointmentNumber  = $"APT-{count + 1:D6}",
            ScheduledAt        = request.ScheduledAt,
            Type               = type,
            Status             = AppointmentStatus.Scheduled,
            Reason             = request.Reason,
            DurationMinutes    = request.DurationMinutes
        };

        db.Appointments.Add(appointment);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(appointment.Id);
    }
}
