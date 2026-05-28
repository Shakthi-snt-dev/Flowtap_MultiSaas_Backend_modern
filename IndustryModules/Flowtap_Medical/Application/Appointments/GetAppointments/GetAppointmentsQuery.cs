using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Medical.Application.Appointments.GetAppointments;

public record GetAppointmentsQuery(
    Guid CompanyId, Guid? LocationId, Guid? PatientId,
    string? Status, DateTime? Date) : IRequest<Result<List<AppointmentDto>>>;

public record AppointmentDto(
    Guid Id, Guid LocationId, Guid PatientId, string PatientName, string PatientNumber,
    Guid? DoctorEmployeeId, string AppointmentNumber, DateTime ScheduledAt,
    string Type, string Status, string? Reason, int? DurationMinutes, DateTime CreatedAt);
