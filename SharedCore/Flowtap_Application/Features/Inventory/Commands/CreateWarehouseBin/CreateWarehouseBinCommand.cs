using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.CreateWarehouseBin;

public record CreateWarehouseBinCommand(
    Guid CompanyId,
    Guid RackId,
    string Code,
    int? Level = null,
    int? Position = null,
    decimal? MaxWeightKg = null) : IRequest<Result<Guid>>;
