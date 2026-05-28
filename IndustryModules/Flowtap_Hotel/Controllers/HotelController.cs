using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Hotel.Application.Bookings.CreateBooking;
using Flowtap_Hotel.Application.Bookings.GetBookings;
using Flowtap_Hotel.Application.Bookings.UpdateBookingStatus;
using Flowtap_Hotel.Application.Rooms.CreateRoom;
using Flowtap_Hotel.Application.Rooms.GetRooms;
using Flowtap_Presentation.Authorization;
using Flowtap_Presentation.Controllers;
using Flowtap_Presentation.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Hotel.Controllers;

[RequiresIndustry(IndustryType.Hotel)]
[RequirePermission("Hotel")]
[Route("api/v1/hotel")]
public class HotelController(ISender sender) : ApiController(sender)
{
    // ── Rooms ─────────────────────────────────────────────────────────────────

    [HttpGet("rooms")]
    public async Task<IActionResult> GetRooms([FromQuery] Guid? locationId, [FromQuery] string? status, CancellationToken ct)
        => Ok(await Sender.Send(new GetRoomsQuery(CurrentTenantId, locationId ?? CurrentLocationId, status), ct));

    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    // ── Bookings ──────────────────────────────────────────────────────────────

    [HttpGet("bookings")]
    public async Task<IActionResult> GetBookings(
        [FromQuery] Guid? locationId, [FromQuery] string? status,
        [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
        => Ok(await Sender.Send(new GetBookingsQuery(CurrentTenantId, locationId ?? CurrentLocationId, status, from, to), ct));

    [HttpPost("bookings")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    [HttpPatch("bookings/{id:guid}/status")]
    public async Task<IActionResult> UpdateBookingStatus(Guid id, [FromBody] UpdateBookingStatusCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));
}
