using Flowtap_Application.Common.DTOs;
using Flowtap_Hotel.DbContext;
using Flowtap_Hotel.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Hotel.Application.Bookings.UpdateBookingStatus;

public class UpdateBookingStatusCommandHandler(IHotelDbContext db)
    : IRequestHandler<UpdateBookingStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateBookingStatusCommand request, CancellationToken ct)
    {
        var booking = await db.HotelBookings
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.CompanyId == request.CompanyId, ct);

        if (booking is null) return Result.Failure("Booking not found.");

        if (!Enum.TryParse<BookingStatus>(request.Status, true, out var status))
            return Result.Failure($"Invalid booking status: {request.Status}");

        booking.Status = status;

        // Keep room status in sync
        switch (status)
        {
            case BookingStatus.CheckedIn:
                booking.ActualCheckIn   = DateTime.UtcNow;
                booking.Room.Status     = RoomStatus.Occupied;
                break;
            case BookingStatus.CheckedOut:
                booking.ActualCheckOut  = DateTime.UtcNow;
                booking.Room.Status     = RoomStatus.Cleaning;
                break;
            case BookingStatus.Cancelled:
            case BookingStatus.NoShow:
                booking.Room.Status     = RoomStatus.Available;
                break;
        }

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
