using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateWarehouseBin;

public record UpdateWarehouseBinCommand(
    Guid Id,
    Guid CompanyId,
    string? Code = null,
    int? Level = null,
    int? Position = null,
    decimal? MaxWeightKg = null,
    bool? IsActive = null) : IRequest<Result<bool>>;
