using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.CreateWarehouse;

public record CreateWarehouseCommand(
    Guid CompanyId, string Code, string Name, int Type,
    string? City, string? State, string? Country, string? Address,
    bool IsDefault = false, bool HasRackSystem = false,
    Guid? StoreId = null) : IRequest<Result<Guid>>;
