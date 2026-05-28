using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Medical.Application.Consultations.CreateConsultation;

public record CreateConsultationCommand(
    Guid CompanyId, Guid AppointmentId,
    string? ChiefComplaint, string? Diagnosis, string? TreatmentPlan,
    string? VitalSigns, string? DoctorNotes, decimal? ConsultationFee,
    List<CreatePrescriptionDto>? Prescriptions) : IRequest<Result<Guid>>;

public record CreatePrescriptionDto(
    string MedicineName, Guid? ProductId, string Dosage,
    string? Duration, string? Instructions);
