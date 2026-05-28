using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetWarehouse;

public record GetWarehouseQuery(Guid CompanyId, Guid WarehouseId) : IRequest<Result<WarehouseDto>>;
