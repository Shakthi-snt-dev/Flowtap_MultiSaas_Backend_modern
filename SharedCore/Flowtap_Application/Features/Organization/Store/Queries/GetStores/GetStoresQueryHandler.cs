using AutoMapper;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Organization.Store.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Organization.Store.Queries.GetStores;

public class GetStoresQueryHandler(IApplicationDbContext db, IMapper mapper)
    : IRequestHandler<GetStoresQuery, Result<List<StoreListItemDto>>>
{
    public async Task<Result<List<StoreListItemDto>>> Handle(GetStoresQuery request, CancellationToken ct)
    {
        var stores = await db.Stores
            .Where(s => s.CompanyId == request.CompanyId && s.IsActive)
            .OrderBy(s => s.Title)
            .ToListAsync(ct);

        return Result<List<StoreListItemDto>>.Success(mapper.Map<List<StoreListItemDto>>(stores));
    }
}
