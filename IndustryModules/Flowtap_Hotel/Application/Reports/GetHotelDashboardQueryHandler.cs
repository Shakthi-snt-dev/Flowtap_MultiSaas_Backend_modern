using Flowtap_Application.Common.DTOs;
using Flowtap_Hotel.Application.Reports.DTOs;
using Flowtap_Hotel.DbContext;
using Flowtap_Hotel.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Hotel.Application.Reports;

public class GetHotelDashboardQueryHandler(IHotelDbContext db)
    : IRequestHandler<GetHotelDashboardQuery, Result<HotelDashboardDto>>
{
    public async Task<Result<HotelDashboardDto>> Handle(GetHotelDashboardQuery request, CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var roomQuery = db.HotelRooms.Where(r => r.CompanyId == request.CompanyId && r.IsActive);
        if (request.LocationId.HasValue)
            roomQuery = roomQuery.Where(r => r.LocationId == request.LocationId.Value);

        var rooms = await roomQuery.Select(r => new { r.Status }).ToListAsync(ct);

        var bookingQuery = db.HotelBookings.Where(b => b.CompanyId == request.CompanyId);
        if (request.LocationId.HasValue)
            bookingQuery = bookingQuery.Where(b => b.LocationId == request.LocationId.Value);

        var todayCheckIns  = await bookingQuery.CountAsync(b => b.CheckInDate.Date == today && b.Status == BookingStatus.Confirmed, ct);
        var todayCheckOuts = await bookingQuery.CountAsync(b => b.CheckOutDate.Date == today && b.Status == BookingStatus.CheckedIn, ct);

        var salesQuery = db.Sales.Where(s => s.CompanyId == request.CompanyId
                                          && s.CreatedAt >= today
                                          && s.CreatedAt < tomorrow);
        if (request.LocationId.HasValue)
            salesQuery = salesQuery.Where(s => s.LocationId == request.LocationId.Value);
        var todayRevenue = await salesQuery.SumAsync(s => s.TotalAmount, ct);

        var total = rooms.Count;
        var occupied = rooms.Count(r => r.Status == RoomStatus.Occupied);
        var available = rooms.Count(r => r.Status == RoomStatus.Available);
        var reserved = rooms.Count(r => r.Status == RoomStatus.Reserved);
        var occupancyPct = total > 0 ? Math.Round((decimal)occupied / total * 100, 1) : 0;

        return Result<HotelDashboardDto>.Success(new HotelDashboardDto(
            total, occupied, available, reserved, occupancyPct,
            todayCheckIns, todayCheckOuts, todayRevenue));
    }
}
