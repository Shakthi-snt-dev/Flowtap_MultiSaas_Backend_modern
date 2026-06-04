using Flowtap_Application.Features.Sales.Commands.CreateSale;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Jewelry.Application.Exchange.CreateExchange;
using Flowtap_Jewelry.Application.Sales;
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
    // ── Sales (Jewelry POS) ──────────────────────────────────────────────────
    // Only exposes HallmarkDetails + ExchangeReference — no food tables, no repair tickets

    [HttpPost("sales")]
    [RequirePermission("POS")]
    public async Task<IActionResult> CreateSale([FromBody] JewelryCreateSaleRequest req, CancellationToken ct)
        => Created(await Sender.Send(new CreateSaleCommand(
            CompanyId:      CurrentTenantId,
            LocationId:     req.LocationId,
            ClientId:       req.ClientId,
            Source:         req.Source,
            TicketId:       null,
            Notes:          req.Notes,
            IdempotencyKey: req.IdempotencyKey,
            Items:          req.Items,
            Payments:       req.Payments,
            EmployeeId:     req.EmployeeId,
            IndustryContext: req.HallmarkDetails != null || req.ExchangeReference != null
                ? new Dictionary<string, object?>
                  {
                      ["hallmarkDetails"]   = req.HallmarkDetails,
                      ["exchangeReference"] = req.ExchangeReference
                  }.Where(kv => kv.Value != null)
                   .ToDictionary(kv => kv.Key, kv => kv.Value!)
                : null
        ), ct));

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
