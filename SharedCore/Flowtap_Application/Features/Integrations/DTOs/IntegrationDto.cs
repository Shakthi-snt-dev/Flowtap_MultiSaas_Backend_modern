namespace Flowtap_Application.Features.Integrations.DTOs;

public record IntegrationDto(
    Guid Id,
    Guid CompanyId,
    string Category,
    string Provider,
    string DisplayName,
    string? ConfigJson,
    bool IsEnabled,
    DateTime? ConnectedAt,
    string? WebhookUrl,
    string? LastStatusMessage,
    DateTime? LastCheckedAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
