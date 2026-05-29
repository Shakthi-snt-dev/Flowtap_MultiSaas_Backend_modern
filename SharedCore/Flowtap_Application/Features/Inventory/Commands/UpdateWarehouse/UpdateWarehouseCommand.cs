using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateWarehouse;

public record UpdateWarehouseCommand(
    Guid Id, Guid CompanyId, string? Name, string? Code,
    string? City, string? State, string? Country, string? Address,
    bool? IsActive, bool? HasRackSystem,
    Guid? StoreId          = null,
    Guid? ManagerEmployeeId = null,
    int?  Type             = null) : IRequest<Result<bool>>;
