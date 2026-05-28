using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Hotel.Application.Bookings.GetBookings;

public record GetBookingsQuery(
    Guid CompanyId, Guid? LocationId, string? Status,
    DateTime? From, DateTime? To) : IRequest<Result<List<HotelBookingDto>>>;

public record HotelBookingDto(
    Guid Id, Guid LocationId, Guid RoomId, string RoomNumber,
    string GuestName, string? GuestPhone, string? GuestEmail,
    DateTime CheckInDate, DateTime CheckOutDate,
    DateTime? ActualCheckIn, DateTime? ActualCheckOut,
    int GuestCount, string Status, decimal TotalAmount, decimal PaidAmount,
    string? BookingReference, DateTime CreatedAt);
