using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Jewelry.Application.Exchange.CreateExchange;
using Flowtap_Jewelry.Application.Exchange.GetExchanges;
using Flowtap_Jewelry.Application.MetalRates.CreateMetalRate;
using Flowtap_Jewelry.Application.MetalRates.GetMetalRates;
using Flowtap_Presentation.Authorization;
using Flowtap_Presentation.Controllers;
using Flowtap_Presentation.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Jewelry.Controllers;

[RequiresIndustry(IndustryType.Jewelry)]
[RequirePermission("Jewelry")]
[Route("api/v1/jewelry")]
public class JewelryController(ISender sender) : ApiController(sender)
{
    // ── Metal Rates ───────────────────────────────────────────────────────────

    [HttpGet("rates")]
    public async Task<IActionResult> GetRates(CancellationToken ct)
        => Ok(await Sender.Send(new GetMetalRatesQuery(CurrentTenantId), ct));

    [HttpPost("rates")]
    public async Task<IActionResult> CreateRate([FromBody] CreateMetalRateCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    // ── Metal Exchange ────────────────────────────────────────────────────────

    [HttpGet("exchange")]
    public async Task<IActionResult> GetExchanges([FromQuery] Guid? locationId, CancellationToken ct)
        => Ok(await Sender.Send(new GetExchangesQuery(CurrentTenantId, locationId ?? CurrentLocationId), ct));

    [HttpPost("exchange")]
    public async Task<IActionResult> CreateExchange([FromBody] CreateExchangeCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));
}
