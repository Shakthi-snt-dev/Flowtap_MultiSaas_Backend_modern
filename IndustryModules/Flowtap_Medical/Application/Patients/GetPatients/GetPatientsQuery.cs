using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Medical.Application.Patients.GetPatients;

public record GetPatientsQuery(Guid CompanyId, Guid? LocationId, string? Search) : IRequest<Result<List<PatientDto>>>;

public record PatientDto(
    Guid Id, Guid LocationId, string PatientNumber, string Name,
    string? Phone, string? Email, DateTime? DateOfBirth,
    string? Gender, string? BloodGroup, string? Address,
    string? Allergies, string? ChronicConditions, bool IsActive);
