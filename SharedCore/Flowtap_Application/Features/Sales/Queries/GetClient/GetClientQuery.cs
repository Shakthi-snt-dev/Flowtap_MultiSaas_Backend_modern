using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Sales.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Queries.GetClient;

public record GetClientQuery(Guid CompanyId, Guid ClientId) : IRequest<Result<ClientDto>>;
