using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Hotel.Application.Reports;
using Flowtap_Presentation.Authorization;
using Flowtap_Presentation.Controllers;
using Flowtap_Presentation.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Hotel.Controllers;

[RequiresIndustry(IndustryType.Hotel)]
[Route("api/v1/hotel/reports")]
public class HotelReportController(ISender sender) : ApiController(sender)
{
    [HttpGet("dashboard")]
    [RequirePermission("Hotel")]
    public async Task<IActionResult> GetDashboard([FromQuery] Guid? locationId, CancellationToken ct)
        => Ok(await Sender.Send(new GetHotelDashboardQuery(CurrentTenantId, locationId ?? CurrentLocationId), ct));

    [HttpGet("occupancy")]
    [RequirePermission("Hotel")]
    public async Task<IActionResult> GetOccupancy(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? locationId,
        CancellationToken ct)
    {
        var dateFrom = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var dateTo   = to   ?? DateTime.UtcNow.Date;
        return Ok(await Sender.Send(new GetOccupancyReportQuery(CurrentTenantId, locationId ?? CurrentLocationId, dateFrom, dateTo), ct));
    }

    [HttpGet("bookings")]
    [RequirePermission("Hotel")]
    public async Task<IActionResult> GetBookings(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? locationId,
        CancellationToken ct)
    {
        var dateFrom = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var dateTo   = to   ?? DateTime.UtcNow.Date;
        return Ok(await Sender.Send(new GetBookingReportQuery(CurrentTenantId, locationId ?? CurrentLocationId, dateFrom, dateTo), ct));
    }

    [HttpGet("revpar")]
    [RequirePermission("Hotel")]
    public async Task<IActionResult> GetRevPar(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? locationId,
        CancellationToken ct)
    {
        var dateFrom = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var dateTo   = to   ?? DateTime.UtcNow.Date;
        return Ok(await Sender.Send(new GetRevParQuery(CurrentTenantId, locationId ?? CurrentLocationId, dateFrom, dateTo), ct));
    }
}
