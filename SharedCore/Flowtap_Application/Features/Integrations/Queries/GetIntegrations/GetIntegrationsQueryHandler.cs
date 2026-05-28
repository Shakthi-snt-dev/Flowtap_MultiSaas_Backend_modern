using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Integrations.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Integrations.Queries.GetIntegrations;

public class GetIntegrationsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetIntegrationsQuery, Result<List<IntegrationDto>>>
{
    public async Task<Result<List<IntegrationDto>>> Handle(GetIntegrationsQuery request, CancellationToken ct)
    {
        var query = db.Integrations
            .Where(i => i.CompanyId == request.CompanyId);

        if (!string.IsNullOrWhiteSpace(request.Category))
            query = query.Where(i => i.Category == request.Category);

        if (!string.IsNullOrWhiteSpace(request.Provider))
            query = query.Where(i => i.Provider == request.Provider);

        var integrations = await query
            .OrderBy(i => i.Category)
            .ThenBy(i => i.DisplayName)
            .Select(i => new IntegrationDto(
                i.Id,
                i.CompanyId,
                i.Category,
                i.Provider,
                i.DisplayName,
                i.ConfigJson,
                i.IsEnabled,
                i.ConnectedAt,
                i.WebhookUrl,
                i.LastStatusMessage,
                i.LastCheckedAt,
                i.CreatedAt,
                i.UpdatedAt))
            .ToListAsync(ct);

        return Result<List<IntegrationDto>>.Success(integrations);
    }
}
