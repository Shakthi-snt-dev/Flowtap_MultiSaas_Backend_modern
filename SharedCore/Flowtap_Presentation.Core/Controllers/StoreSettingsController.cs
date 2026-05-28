using Flowtap_Application.Features.Organization.StoreSetting.Commands.UpsertStoreSetting;
using Flowtap_Application.Features.Organization.StoreSetting.Queries.GetStoreSetting;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Route("api/v1/store-settings")]
public class StoreSettingsController(ISender sender) : ApiController(sender)
{
    [HttpGet("{locationId:guid}")]
    public async Task<IActionResult> Get(Guid locationId, CancellationToken ct)
        => Ok(await Sender.Send(new GetStoreSettingQuery(CurrentTenantId, locationId), ct));

    [HttpPut("{locationId:guid}")]
    public async Task<IActionResult> Upsert(Guid locationId, [FromBody] UpsertStoreSettingCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { CompanyId = CurrentTenantId, LocationId = locationId }, ct));
}
