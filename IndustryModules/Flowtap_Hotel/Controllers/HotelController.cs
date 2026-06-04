using Flowtap_Application.Features.Sales.Commands.CreateSale;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Hotel.Application.Bookings.CreateBooking;
using Flowtap_Hotel.Application.Sales;
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
    // ── Sales (Hotel POS) ────────────────────────────────────────────────────
    // Only exposes RoomId + BookingReference — no food tables, no repair tickets

    [HttpPost("sales")]
    [RequirePermission("POS")]
    public async Task<IActionResult> CreateSale([FromBody] HotelCreateSaleRequest req, CancellationToken ct)
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
            IndustryContext: req.RoomId.HasValue || req.BookingReference != null
                ? new Dictionary<string, object?>
                  {
                      ["roomId"]           = req.RoomId,
                      ["bookingReference"] = req.BookingReference
                  }.Where(kv => kv.Value != null)
                   .ToDictionary(kv => kv.Key, kv => kv.Value!)
                : null
        ), ct));

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
