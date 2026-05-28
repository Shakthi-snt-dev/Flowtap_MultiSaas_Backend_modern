using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetReorderRules;

public record GetReorderRulesQuery(Guid CompanyId)
    : IRequest<Result<List<ReorderRuleDto>>>;
