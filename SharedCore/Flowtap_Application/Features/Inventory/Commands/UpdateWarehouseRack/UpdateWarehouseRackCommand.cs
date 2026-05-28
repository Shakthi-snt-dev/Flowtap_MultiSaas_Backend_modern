using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateWarehouseRack;

public record UpdateWarehouseRackCommand(
    Guid Id,
    Guid CompanyId,
    string? Name = null,
    string? ZoneLabel = null,
    decimal? MaxLoadKg = null,
    int? Levels = null,
    bool? IsActive = null) : IRequest<Result<bool>>;
