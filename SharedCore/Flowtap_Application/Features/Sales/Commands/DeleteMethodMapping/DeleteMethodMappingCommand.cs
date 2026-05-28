using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.DeleteMethodMapping;

public record DeleteMethodMappingCommand(Guid Id, Guid CompanyId) : IRequest<Result<bool>>;
