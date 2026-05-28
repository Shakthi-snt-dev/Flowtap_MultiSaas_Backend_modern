using Flowtap_Application.Features.Sales.Commands.CreateCampaign;
using Flowtap_Application.Features.Sales.Commands.DeleteCampaign;
using Flowtap_Application.Features.Sales.Commands.UpdateCampaign;
using Flowtap_Application.Features.Sales.Queries.GetCampaigns;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Route("api/v1/campaigns")]
public class CampaignController(ISender sender) : ApiController(sender)
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid companyId, [FromQuery] bool activeOnly = false, CancellationToken ct = default)
        => Ok(await Sender.Send(new GetCampaignsQuery(companyId == Guid.Empty ? CurrentTenantId : companyId, activeOnly), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCampaignCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCampaignCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteCampaignCommand(id, CurrentTenantId), ct));
}
