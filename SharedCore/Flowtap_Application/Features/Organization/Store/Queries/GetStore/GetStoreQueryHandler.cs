using AutoMapper;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Organization.Store.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Organization.Store.Queries.GetStore;

public class GetStoreQueryHandler(IApplicationDbContext db, IMapper mapper)
    : IRequestHandler<GetStoreQuery, Result<StoreDto>>
{
    public async Task<Result<StoreDto>> Handle(GetStoreQuery request, CancellationToken ct)
    {
        var store = await db.Stores
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId && s.Id == request.StoreId, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Core.Organization.Entities.Store), request.StoreId);

        return Result<StoreDto>.Success(mapper.Map<StoreDto>(store));
    }
}
