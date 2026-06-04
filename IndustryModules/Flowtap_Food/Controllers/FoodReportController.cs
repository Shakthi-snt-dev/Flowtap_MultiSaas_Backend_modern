using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Food.Application.Reports;
using Flowtap_Presentation.Authorization;
using Flowtap_Presentation.Controllers;
using Flowtap_Presentation.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Food.Controllers;

[RequiresIndustry(IndustryType.Food)]
[Route("api/v1/food/reports")]
public class FoodReportController(ISender sender) : ApiController(sender)
{
    [HttpGet("dashboard")]
    [RequirePermission("Food")]
    public async Task<IActionResult> GetDashboard([FromQuery] Guid? locationId, CancellationToken ct)
        => Ok(await Sender.Send(new GetFoodDashboardQuery(CurrentTenantId, locationId ?? CurrentLocationId), ct));

    [HttpGet("kitchen-performance")]
    [RequirePermission("Food")]
    public async Task<IActionResult> GetKitchenPerformance(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? locationId,
        CancellationToken ct)
    {
        var dateFrom = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var dateTo   = to   ?? DateTime.UtcNow.Date;
        return Ok(await Sender.Send(new GetKitchenPerformanceQuery(CurrentTenantId, locationId ?? CurrentLocationId, dateFrom, dateTo), ct));
    }

    [HttpGet("table-turnover")]
    [RequirePermission("Food")]
    public async Task<IActionResult> GetTableTurnover(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? locationId,
        CancellationToken ct)
    {
        var dateFrom = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var dateTo   = to   ?? DateTime.UtcNow.Date;
        return Ok(await Sender.Send(new GetTableTurnoverQuery(CurrentTenantId, locationId ?? CurrentLocationId, dateFrom, dateTo), ct));
    }

    [HttpGet("recipe-profitability")]
    [RequirePermission("Food")]
    public async Task<IActionResult> GetRecipeProfitability(CancellationToken ct)
        => Ok(await Sender.Send(new GetRecipeProfitabilityQuery(CurrentTenantId), ct));
}
