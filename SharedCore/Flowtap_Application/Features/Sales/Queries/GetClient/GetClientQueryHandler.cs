using AutoMapper;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Sales.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Queries.GetClient;

public class GetClientQueryHandler(IApplicationDbContext db, IMapper mapper)
    : IRequestHandler<GetClientQuery, Result<ClientDto>>
{
    public async Task<Result<ClientDto>> Handle(GetClientQuery request, CancellationToken ct)
    {
        var client = await db.Clients
            .FirstOrDefaultAsync(c => c.CompanyId == request.CompanyId && c.Id == request.ClientId, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Modules.Sales.Entities.Client), request.ClientId);

        return Result<ClientDto>.Success(mapper.Map<ClientDto>(client));
    }
}
