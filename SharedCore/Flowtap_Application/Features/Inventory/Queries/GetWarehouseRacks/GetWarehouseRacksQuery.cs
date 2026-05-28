using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetWarehouseRacks;

public record WarehouseRackDto(
    Guid Id,
    string Code,
    string Name,
    string? ZoneLabel,
    string? ZoneType,
    string? Type,
    decimal? MaxLoadKg,
    int? Levels,
    bool IsActive);

public record GetWarehouseRacksQuery(Guid CompanyId, Guid WarehouseId) : IRequest<Result<List<WarehouseRackDto>>>;
