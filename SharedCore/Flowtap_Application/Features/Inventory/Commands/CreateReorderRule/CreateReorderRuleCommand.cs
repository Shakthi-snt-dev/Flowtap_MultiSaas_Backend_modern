using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.CreateReorderRule;

public record CreateReorderRuleCommand(
    Guid CompanyId, Guid WarehouseId, Guid ProductId,
    decimal MinimumQuantity, decimal ReorderQuantity,
    Guid? PreferredSupplierId = null, int? LeadTimeDays = null) : IRequest<Result<Guid>>;
