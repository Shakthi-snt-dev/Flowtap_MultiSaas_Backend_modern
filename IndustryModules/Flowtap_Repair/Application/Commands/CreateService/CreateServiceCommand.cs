using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Commands.CreateService;

public record CreateServiceCommand(
    Guid CompanyId, string Name, decimal BasePrice,
    Guid? ServiceCategoryId = null, string? Description = null,
    string? EstimatedDuration = null, Guid? TaxSlabId = null,
    bool RequiresInventory = false, Guid? InventoryProductId = null,
    bool IsUniversal = false) : IRequest<Result<Guid>>;

