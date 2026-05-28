using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Medical.Application.Appointments.CreateAppointment;

public record CreateAppointmentCommand(
    Guid CompanyId, Guid LocationId, Guid PatientId, Guid? DoctorEmployeeId,
    DateTime ScheduledAt, string Type, string? Reason, int? DurationMinutes) : IRequest<Result<Guid>>;
