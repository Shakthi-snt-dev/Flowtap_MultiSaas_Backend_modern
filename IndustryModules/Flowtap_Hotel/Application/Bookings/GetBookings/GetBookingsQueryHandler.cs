using Flowtap_Application.Common.DTOs;
using Flowtap_Hotel.DbContext;
using Flowtap_Hotel.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Hotel.Application.Bookings.GetBookings;

public class GetBookingsQueryHandler(IHotelDbContext db)
    : IRequestHandler<GetBookingsQuery, Result<List<HotelBookingDto>>>
{
    public async Task<Result<List<HotelBookingDto>>> Handle(GetBookingsQuery request, CancellationToken ct)
    {
        var query = db.HotelBookings
            .Include(b => b.Room)
            .Where(b => b.CompanyId == request.CompanyId);

        if (request.LocationId.HasValue)
            query = query.Where(b => b.LocationId == request.LocationId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<BookingStatus>(request.Status, true, out var status))
            query = query.Where(b => b.Status == status);

        if (request.From.HasValue)
            query = query.Where(b => b.CheckInDate >= request.From.Value);

        if (request.To.HasValue)
            query = query.Where(b => b.CheckOutDate <= request.To.Value);

        var bookings = await query
            .OrderByDescending(b => b.CheckInDate)
            .Select(b => new HotelBookingDto(b.Id, b.LocationId, b.RoomId,
                b.Room.RoomNumber, b.GuestName, b.GuestPhone, b.GuestEmail,
                b.CheckInDate, b.CheckOutDate, b.ActualCheckIn, b.ActualCheckOut,
                b.GuestCount, b.Status.ToString(), b.TotalAmount, b.PaidAmount,
                b.BookingReference, b.CreatedAt))
            .ToListAsync(ct);

        return Result<List<HotelBookingDto>>.Success(bookings);
    }
}
