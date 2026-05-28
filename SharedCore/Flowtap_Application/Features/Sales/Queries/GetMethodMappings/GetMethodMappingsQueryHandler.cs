using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Sales.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Queries.GetMethodMappings;

public class GetMethodMappingsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetMethodMappingsQuery, Result<List<MethodMappingDto>>>
{
    public async Task<Result<List<MethodMappingDto>>> Handle(GetMethodMappingsQuery request, CancellationToken ct)
    {
        var mappings = await db.PaymentMethodMappings
            .Include(m => m.PaymentAccount)
            .Where(m => m.CompanyId == request.CompanyId && m.LocationId == request.LocationId)
            .OrderBy(m => m.Method)
            .ToListAsync(ct);

        var dtos = mappings.Select(m => new MethodMappingDto(
            m.Id,
            m.Method.ToString(),
            m.PaymentAccountId,
            m.PaymentAccount.Name,
            m.PaymentAccount.Type.ToString()
        )).ToList();

        return Result<List<MethodMappingDto>>.Success(dtos);
    }
}
