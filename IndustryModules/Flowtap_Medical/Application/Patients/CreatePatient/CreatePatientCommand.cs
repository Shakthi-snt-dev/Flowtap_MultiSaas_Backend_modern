using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Medical.Application.Patients.CreatePatient;

public record CreatePatientCommand(
    Guid CompanyId, Guid LocationId, string Name, string? Phone, string? Email,
    DateTime? DateOfBirth, string? Gender, string? BloodGroup, string? Address,
    string? EmergencyContactName, string? EmergencyContactPhone,
    string? Allergies, string? ChronicConditions) : IRequest<Result<Guid>>;
