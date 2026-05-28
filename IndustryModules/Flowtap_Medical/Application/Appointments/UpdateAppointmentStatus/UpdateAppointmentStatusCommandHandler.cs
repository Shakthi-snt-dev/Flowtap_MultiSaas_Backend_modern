using Flowtap_Application.Common.DTOs;
using Flowtap_Medical.DbContext;
using Flowtap_Medical.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Medical.Application.Appointments.UpdateAppointmentStatus;

public class UpdateAppointmentStatusCommandHandler(IMedicalDbContext db)
    : IRequestHandler<UpdateAppointmentStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateAppointmentStatusCommand request, CancellationToken ct)
    {
        var appointment = await db.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.CompanyId == request.CompanyId, ct);

        if (appointment is null) return Result.Failure("Appointment not found.");

        if (!Enum.TryParse<AppointmentStatus>(request.Status, true, out var status))
            return Result.Failure($"Invalid appointment status: {request.Status}");

        appointment.Status = status;
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
