using Flowtap_Application.Common.DTOs;
using Flowtap_Medical.DbContext;
using Flowtap_Medical.Domain.Entities;
using Flowtap_Medical.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Medical.Application.Patients.CreatePatient;

public class CreatePatientCommandHandler(IMedicalDbContext db)
    : IRequestHandler<CreatePatientCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePatientCommand request, CancellationToken ct)
    {
        var count = await db.Patients.CountAsync(p => p.CompanyId == request.CompanyId, ct);

        var patient = new Patient
        {
            CompanyId            = request.CompanyId,
            LocationId           = request.LocationId,
            PatientNumber        = $"PAT-{count + 1:D6}",
            Name                 = request.Name,
            Phone                = request.Phone,
            Email                = request.Email,
            DateOfBirth          = request.DateOfBirth,
            Gender               = request.Gender != null && Enum.TryParse<Gender>(request.Gender, true, out var g) ? g : null,
            BloodGroup           = request.BloodGroup != null && Enum.TryParse<BloodGroup>(request.BloodGroup, true, out var bg) ? bg : null,
            Address              = request.Address,
            EmergencyContactName  = request.EmergencyContactName,
            EmergencyContactPhone = request.EmergencyContactPhone,
            Allergies            = request.Allergies,
            ChronicConditions    = request.ChronicConditions
        };

        db.Patients.Add(patient);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(patient.Id);
    }
}
