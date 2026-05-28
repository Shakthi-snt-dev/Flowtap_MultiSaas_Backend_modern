using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.DeleteReorderRule;

public record DeleteReorderRuleCommand(Guid Id, Guid CompanyId) : IRequest<Result<bool>>;
