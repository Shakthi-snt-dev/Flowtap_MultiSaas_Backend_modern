using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetWarehouseBins;

public record WarehouseBinDto(
    Guid Id,
    string Code,
    int? Level,
    int? Position,
    decimal? MaxWeightKg,
    string Status,
    bool IsActive);

public record GetWarehouseBinsQuery(Guid CompanyId, Guid RackId) : IRequest<Result<List<WarehouseBinDto>>>;
