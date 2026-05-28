using Flowtap_Application.Features.Marketing.Commands.CreateMarketingCampaign;
using Flowtap_Application.Features.Marketing.Commands.DeleteMarketingCampaign;
using Flowtap_Application.Features.Marketing.Commands.UpdateMarketingCampaign;
using Flowtap_Application.Features.Marketing.Queries.GetMarketingCampaigns;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Route("api/v1/marketing")]
public class MarketingController(ISender sender) : ApiController(sender)
{
    // ── Marketing Campaigns ──────────────────────────────────────────────────

    [HttpGet("campaigns")]
    public async Task<IActionResult> GetCampaigns(
        [FromQuery] bool? isActive,
        CancellationToken ct)
        => Ok(await Sender.Send(new GetMarketingCampaignsQuery(CurrentTenantId, isActive), ct));

    [HttpPost("campaigns")]
    public async Task<IActionResult> CreateCampaign(
        [FromBody] CreateMarketingCampaignCommand command,
        CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    [HttpPut("campaigns/{id:guid}")]
    public async Task<IActionResult> UpdateCampaign(
        Guid id,
        [FromBody] UpdateMarketingCampaignCommand command,
        CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpDelete("campaigns/{id:guid}")]
    public async Task<IActionResult> DeleteCampaign(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteMarketingCampaignCommand(id, CurrentTenantId), ct));
}
