using Flowtap_Application.Common.DTOs;
using Flowtap_Hotel.Application.Reports.DTOs;
using Flowtap_Hotel.DbContext;
using Flowtap_Hotel.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Hotel.Application.Reports;

public class GetBookingReportQueryHandler(IHotelDbContext db)
    : IRequestHandler<GetBookingReportQuery, Result<BookingReportDto>>
{
    public async Task<Result<BookingReportDto>> Handle(GetBookingReportQuery request, CancellationToken ct)
    {
        var query = db.HotelBookings
            .Where(b => b.CompanyId == request.CompanyId
                     && b.CreatedAt >= request.From
                     && b.CreatedAt < request.To.AddDays(1));
        if (request.LocationId.HasValue)
            query = query.Where(b => b.LocationId == request.LocationId.Value);

        var bookings = await query
            .Select(b => new { b.Status, b.CheckInDate, b.CheckOutDate, b.TotalAmount, b.CreatedAt })
            .ToListAsync(ct);

        var days = Enumerable.Range(0, (request.To.Date - request.From.Date).Days + 1)
            .Select(d => request.From.Date.AddDays(d))
            .Select(date => new DailyBookingDto(
                date,
                bookings.Count(b => b.CheckInDate.Date == date && b.Status == BookingStatus.CheckedIn),
                bookings.Count(b => b.CheckOutDate.Date == date && b.Status == BookingStatus.CheckedOut),
                bookings.Count(b => b.CreatedAt.Date == date)))
            .ToList();

        return Result<BookingReportDto>.Success(new BookingReportDto(
            From: request.From,
            To: request.To,
            TotalBookings: bookings.Count,
            Arrivals: bookings.Count(b => b.Status == BookingStatus.CheckedIn),
            Departures: bookings.Count(b => b.Status == BookingStatus.CheckedOut),
            Cancellations: bookings.Count(b => b.Status == BookingStatus.Cancelled),
            NoShows: bookings.Count(b => b.Status == BookingStatus.NoShow),
            TotalRevenue: bookings.Sum(b => b.TotalAmount),
            ByDay: days
        ));
    }
}
