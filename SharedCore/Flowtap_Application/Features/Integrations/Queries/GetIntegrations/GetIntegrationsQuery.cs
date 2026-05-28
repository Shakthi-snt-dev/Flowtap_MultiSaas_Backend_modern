using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Integrations.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Integrations.Queries.GetIntegrations;

public record GetIntegrationsQuery(
    Guid CompanyId,
    string? Category = null,
    string? Provider = null
) : IRequest<Result<List<IntegrationDto>>>;
