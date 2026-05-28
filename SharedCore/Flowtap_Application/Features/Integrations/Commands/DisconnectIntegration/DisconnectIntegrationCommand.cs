using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Integrations.Commands.DisconnectIntegration;

public record DisconnectIntegrationCommand(
    Guid Id,
    Guid CompanyId
) : IRequest<Result>;
