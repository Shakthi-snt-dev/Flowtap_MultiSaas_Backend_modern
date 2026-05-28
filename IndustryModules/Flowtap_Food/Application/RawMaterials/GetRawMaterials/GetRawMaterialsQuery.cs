using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Food.Application.RawMaterials.GetRawMaterials;

public record GetRawMaterialsQuery(Guid CompanyId, Guid? WarehouseId) : IRequest<Result<List<RawMaterialDto>>>;

public record RawMaterialDto(
    Guid ProductId,
    string Name,
    string SKU,
    decimal? CurrentStock,
    string? Unit,
    decimal DefaultCostPrice);
