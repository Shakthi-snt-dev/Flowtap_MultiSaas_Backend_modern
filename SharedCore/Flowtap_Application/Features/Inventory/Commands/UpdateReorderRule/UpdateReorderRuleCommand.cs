using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateReorderRule;

public record UpdateReorderRuleCommand(
    Guid Id, Guid CompanyId,
    decimal? MinimumQuantity, decimal? ReorderQuantity,
    int? LeadTimeDays, bool? IsActive) : IRequest<Result<bool>>;
