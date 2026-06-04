using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Jewelry.Application.Reports;
using Flowtap_Presentation.Authorization;
using Flowtap_Presentation.Controllers;
using Flowtap_Presentation.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Jewelry.Controllers;

[RequiresIndustry(IndustryType.Jewelry)]
[Route("api/v1/jewelry/reports")]
public class JewelryReportController(ISender sender) : ApiController(sender)
{
    [HttpGet("dashboard")]
    [RequirePermission("Jewelry")]
    public async Task<IActionResult> GetDashboard([FromQuery] Guid? locationId, CancellationToken ct)
        => Ok(await Sender.Send(new GetJewelryDashboardQuery(CurrentTenantId, locationId ?? CurrentLocationId), ct));

    [HttpGet("metal-stock")]
    [RequirePermission("Jewelry")]
    public async Task<IActionResult> GetMetalStock([FromQuery] Guid? warehouseId, CancellationToken ct)
        => Ok(await Sender.Send(new GetMetalStockReportQuery(CurrentTenantId, warehouseId), ct));

    [HttpGet("exchanges")]
    [RequirePermission("Jewelry")]
    public async Task<IActionResult> GetExchanges(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? locationId,
        CancellationToken ct)
    {
        var dateFrom = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var dateTo   = to   ?? DateTime.UtcNow.Date;
        return Ok(await Sender.Send(new GetExchangeReportQuery(CurrentTenantId, locationId ?? CurrentLocationId, dateFrom, dateTo), ct));
    }
}
