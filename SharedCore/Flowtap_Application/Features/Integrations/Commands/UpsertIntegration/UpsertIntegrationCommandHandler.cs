using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Integrations.DTOs;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Integrations.Commands.UpsertIntegration;

public class UpsertIntegrationCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpsertIntegrationCommand, Result<IntegrationDto>>
{
    public async Task<Result<IntegrationDto>> Handle(UpsertIntegrationCommand request, CancellationToken ct)
    {
        var existing = await db.Integrations
            .FirstOrDefaultAsync(i => i.CompanyId == request.CompanyId && i.Provider == request.Provider, ct);

        if (existing is not null)
        {
            // Update existing
            existing.Category = request.Category;
            existing.DisplayName = request.DisplayName;
            existing.IsEnabled = request.IsEnabled;
            existing.WebhookUrl = request.WebhookUrl;
            existing.UpdatedAt = DateTime.UtcNow;

            // Only update ConfigJson if provided (non-null) — avoids overwriting stored keys with empty
            if (request.ConfigJson is not null)
                existing.ConfigJson = request.ConfigJson;

            if (request.IsEnabled && existing.ConnectedAt is null)
                existing.ConnectedAt = DateTime.UtcNow;
            else if (!request.IsEnabled)
                existing.ConnectedAt = null;

            await db.SaveChangesAsync(ct);
            return Result<IntegrationDto>.Success(ToDto(existing));
        }

        // Create new
        var integration = new Integration
        {
            CompanyId = request.CompanyId,
            Category = request.Category,
            Provider = request.Provider,
            DisplayName = request.DisplayName,
            ConfigJson = request.ConfigJson,
            IsEnabled = request.IsEnabled,
            WebhookUrl = request.WebhookUrl,
            ConnectedAt = request.IsEnabled ? DateTime.UtcNow : null,
        };

        db.Integrations.Add(integration);
        await db.SaveChangesAsync(ct);
        return Result<IntegrationDto>.Success(ToDto(integration));
    }

    private static IntegrationDto ToDto(Integration i) => new(
        i.Id, i.CompanyId, i.Category, i.Provider, i.DisplayName,
        i.ConfigJson, i.IsEnabled, i.ConnectedAt, i.WebhookUrl,
        i.LastStatusMessage, i.LastCheckedAt, i.CreatedAt, i.UpdatedAt);
}
