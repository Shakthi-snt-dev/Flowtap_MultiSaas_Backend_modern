using Flowtap_Application.Common.DTOs;
using Flowtap_Medical.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Medical.Application.Patients.GetPatients;

public class GetPatientsQueryHandler(IMedicalDbContext db)
    : IRequestHandler<GetPatientsQuery, Result<List<PatientDto>>>
{
    public async Task<Result<List<PatientDto>>> Handle(GetPatientsQuery request, CancellationToken ct)
    {
        var query = db.Patients.Where(p => p.CompanyId == request.CompanyId && p.IsActive);

        if (request.LocationId.HasValue)
            query = query.Where(p => p.LocationId == request.LocationId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(s)
                                  || (p.Phone != null && p.Phone.Contains(s))
                                  || p.PatientNumber.ToLower().Contains(s));
        }

        var patients = await query
            .OrderBy(p => p.Name)
            .Select(p => new PatientDto(p.Id, p.LocationId, p.PatientNumber, p.Name,
                p.Phone, p.Email, p.DateOfBirth,
                p.Gender.HasValue ? p.Gender.ToString() : null,
                p.BloodGroup.HasValue ? p.BloodGroup.ToString() : null,
                p.Address, p.Allergies, p.ChronicConditions, p.IsActive))
            .ToListAsync(ct);

        return Result<List<PatientDto>>.Success(patients);
    }
}
