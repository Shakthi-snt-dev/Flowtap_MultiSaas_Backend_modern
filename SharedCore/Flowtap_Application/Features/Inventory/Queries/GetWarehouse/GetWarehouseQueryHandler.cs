using AutoMapper;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetWarehouse;

public class GetWarehouseQueryHandler(IApplicationDbContext db, IMapper mapper)
    : IRequestHandler<GetWarehouseQuery, Result<WarehouseDto>>
{
    public async Task<Result<WarehouseDto>> Handle(GetWarehouseQuery request, CancellationToken ct)
    {
        var warehouse = await db.Warehouses
            .FirstOrDefaultAsync(w => w.CompanyId == request.CompanyId && w.Id == request.WarehouseId, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities.Warehouse), request.WarehouseId);

        return Result<WarehouseDto>.Success(mapper.Map<WarehouseDto>(warehouse));
    }
}
