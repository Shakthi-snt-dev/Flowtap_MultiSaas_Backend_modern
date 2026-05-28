using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.CreateMethodMapping;

public record CreateMethodMappingCommand(
    Guid CompanyId, Guid LocationId, string Method, Guid PaymentAccountId) : IRequest<Result<Guid>>;
