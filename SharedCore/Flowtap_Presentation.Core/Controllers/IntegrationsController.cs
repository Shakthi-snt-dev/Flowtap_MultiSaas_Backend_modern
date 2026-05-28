using Flowtap_Application.Features.Integrations.Commands.DisconnectIntegration;
using Flowtap_Application.Features.Integrations.Commands.UpsertIntegration;
using Flowtap_Application.Features.Integrations.Queries.GetIntegrations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Flowtap_Presentation.Authorization.RequirePermission("Settings")]
[Route("api/v1/integrations")]
public class IntegrationsController(ISender sender) : ApiController(sender)
{
    /// <summary>
    /// Get all integrations for a company, optionally filtered by category.
    /// GET /api/v1/integrations?companyId=...&amp;category=Payment
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetIntegrations(
        [FromQuery] Guid companyId,
        [FromQuery] string? category,
        [FromQuery] string? provider,
        CancellationToken ct)
        => Ok(await Sender.Send(new GetIntegrationsQuery(companyId, category, provider), ct));

    /// <summary>
    /// Create or update an integration for a provider.
    /// Uses upsert — safe to call multiple times.
    /// POST /api/v1/integrations
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> UpsertIntegration(
        [FromBody] UpsertIntegrationCommand command,
        CancellationToken ct)
        => Ok(await Sender.Send(command, ct));

    /// <summary>
    /// Disconnect an integration — clears credentials, sets IsEnabled=false.
    /// DELETE /api/v1/integrations/{id}?companyId=...
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Disconnect(
        Guid id,
        [FromQuery] Guid companyId,
        CancellationToken ct)
        => FromResult(await Sender.Send(new DisconnectIntegrationCommand(id, companyId), ct));
}
