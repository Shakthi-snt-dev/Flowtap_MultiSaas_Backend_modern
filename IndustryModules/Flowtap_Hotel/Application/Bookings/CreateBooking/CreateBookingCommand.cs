using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Hotel.Application.Bookings.CreateBooking;

public record CreateBookingCommand(
    Guid CompanyId, Guid LocationId, Guid RoomId,
    Guid? ClientId, string GuestName, string? GuestPhone, string? GuestEmail,
    string? GuestIdNumber, DateTime CheckInDate, DateTime CheckOutDate,
    int GuestCount, decimal TotalAmount, string? Notes) : IRequest<Result<Guid>>;
