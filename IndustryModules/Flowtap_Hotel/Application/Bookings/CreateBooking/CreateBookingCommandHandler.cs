using Flowtap_Application.Common.DTOs;
using Flowtap_Hotel.DbContext;
using Flowtap_Hotel.Domain.Entities;
using Flowtap_Hotel.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Hotel.Application.Bookings.CreateBooking;

public class CreateBookingCommandHandler(IHotelDbContext db)
    : IRequestHandler<CreateBookingCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBookingCommand request, CancellationToken ct)
    {
        var room = await db.HotelRooms.FirstOrDefaultAsync(
            r => r.Id == request.RoomId && r.CompanyId == request.CompanyId, ct);

        if (room is null) return Result<Guid>.Failure("Room not found.");
        if (room.Status == RoomStatus.OutOfOrder || room.Status == RoomStatus.Maintenance)
            return Result<Guid>.Failure($"Room {room.RoomNumber} is not available for booking.");

        // Check for overlapping bookings
        var overlap = await db.HotelBookings.AnyAsync(b =>
            b.RoomId == request.RoomId &&
            b.CompanyId == request.CompanyId &&
            b.Status != BookingStatus.Cancelled &&
            b.Status != BookingStatus.NoShow &&
            b.CheckInDate < request.CheckOutDate &&
            b.CheckOutDate > request.CheckInDate, ct);

        if (overlap) return Result<Guid>.Failure("Room is already booked for the selected dates.");

        var count = await db.HotelBookings.CountAsync(b => b.CompanyId == request.CompanyId, ct);

        var booking = new HotelBooking
        {
            CompanyId        = request.CompanyId,
            LocationId       = request.LocationId,
            RoomId           = request.RoomId,
            ClientId         = request.ClientId,
            GuestName        = request.GuestName,
            GuestPhone       = request.GuestPhone,
            GuestEmail       = request.GuestEmail,
            GuestIdNumber    = request.GuestIdNumber,
            CheckInDate      = request.CheckInDate,
            CheckOutDate     = request.CheckOutDate,
            GuestCount       = request.GuestCount,
            TotalAmount      = request.TotalAmount,
            Status           = BookingStatus.Confirmed,
            Notes            = request.Notes,
            BookingReference = $"BKG-{count + 1:D6}"
        };

        room.Status = RoomStatus.Reserved;

        db.HotelBookings.Add(booking);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(booking.Id);
    }
}
