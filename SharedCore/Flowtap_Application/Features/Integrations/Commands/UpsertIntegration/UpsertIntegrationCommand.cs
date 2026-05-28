using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Integrations.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Integrations.Commands.UpsertIntegration;

/// <summary>
/// Creates or updates an integration for a given provider.
/// If an integration with the same CompanyId + Provider already exists, it is updated.
/// Otherwise a new record is created.
/// </summary>
public record UpsertIntegrationCommand(
    Guid CompanyId,
    string Category,
    string Provider,
    string DisplayName,
    string? ConfigJson,
    bool IsEnabled,
    string? WebhookUrl
) : IRequest<Result<IntegrationDto>>;
