using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Medical.Application.Appointments.UpdateAppointmentStatus;

public record UpdateAppointmentStatusCommand(Guid Id, Guid CompanyId, string Status) : IRequest<Result>;
