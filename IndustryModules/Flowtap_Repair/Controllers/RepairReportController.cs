using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Presentation.Authorization;
using Flowtap_Presentation.Controllers;
using Flowtap_Presentation.Filters;
using Flowtap_Repair.Application.Reports;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Repair.Controllers;

[RequiresIndustry(IndustryType.RepairShop)]
[Route("api/v1/repair/reports")]
public class RepairReportController(ISender sender) : ApiController(sender)
{
    [HttpGet("dashboard")]
    [RequirePermission("ServiceTickets")]
    public async Task<IActionResult> GetDashboard([FromQuery] Guid? locationId, CancellationToken ct)
        => Ok(await Sender.Send(new GetRepairDashboardQuery(CurrentTenantId, locationId ?? CurrentLocationId), ct));

    [HttpGet("ticket-status")]
    [RequirePermission("ServiceTickets")]
    public async Task<IActionResult> GetTicketStatus(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? locationId,
        CancellationToken ct)
    {
        var dateFrom = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var dateTo   = to   ?? DateTime.UtcNow.Date;
        return Ok(await Sender.Send(new GetTicketStatusReportQuery(CurrentTenantId, locationId ?? CurrentLocationId, dateFrom, dateTo), ct));
    }

    [HttpGet("technician-performance")]
    [RequirePermission("ServiceTickets")]
    public async Task<IActionResult> GetTechnicianPerformance(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? locationId,
        CancellationToken ct)
    {
        var dateFrom = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var dateTo   = to   ?? DateTime.UtcNow.Date;
        return Ok(await Sender.Send(new GetTechnicianPerformanceQuery(CurrentTenantId, locationId ?? CurrentLocationId, dateFrom, dateTo), ct));
    }

    [HttpGet("part-consumption")]
    [RequirePermission("ServiceTickets")]
    public async Task<IActionResult> GetPartConsumption(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? locationId,
        CancellationToken ct)
    {
        var dateFrom = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var dateTo   = to   ?? DateTime.UtcNow.Date;
        return Ok(await Sender.Send(new GetPartConsumptionQuery(CurrentTenantId, locationId ?? CurrentLocationId, dateFrom, dateTo), ct));
    }
}
