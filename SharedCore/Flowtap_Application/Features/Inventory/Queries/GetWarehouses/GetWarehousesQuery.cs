using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetWarehouses;

public record GetWarehousesQuery(Guid CompanyId, bool ActiveOnly = true)
    : IRequest<Result<List<WarehouseDto>>>;
