using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.CreateWarehouseRack;

public record CreateWarehouseRackCommand(
    Guid CompanyId,
    Guid WarehouseId,
    string Code,
    string Name,
    string? ZoneLabel = null,
    string? ZoneType = null,
    string? Type = null,
    decimal? MaxLoadKg = null,
    int? Levels = null) : IRequest<Result<Guid>>;
