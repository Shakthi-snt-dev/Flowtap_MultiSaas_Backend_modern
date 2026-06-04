using Flowtap_Application.Common.DTOs;
using Flowtap_Hotel.Application.Reports.DTOs;
using Flowtap_Hotel.DbContext;
using Flowtap_Hotel.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Hotel.Application.Reports;

public class GetRevParQueryHandler(IHotelDbContext db)
    : IRequestHandler<GetRevParQuery, Result<RevParDto>>
{
    public async Task<Result<RevParDto>> Handle(GetRevParQuery request, CancellationToken ct)
    {
        var roomQuery = db.HotelRooms.Where(r => r.CompanyId == request.CompanyId && r.IsActive);
        if (request.LocationId.HasValue)
            roomQuery = roomQuery.Where(r => r.LocationId == request.LocationId.Value);
        var totalRooms = await roomQuery.CountAsync(ct);

        var salesQuery = db.Sales
            .Where(s => s.CompanyId == request.CompanyId
                     && s.CreatedAt >= request.From
                     && s.CreatedAt < request.To.AddDays(1));
        if (request.LocationId.HasValue)
            salesQuery = salesQuery.Where(s => s.LocationId == request.LocationId.Value);
        var totalRevenue = await salesQuery.SumAsync(s => s.TotalAmount, ct);

        var bookingQuery = db.HotelBookings
            .Where(b => b.CompanyId == request.CompanyId
                     && b.Status == BookingStatus.CheckedOut
                     && b.CheckInDate >= request.From
                     && b.CheckOutDate <= request.To.AddDays(1));
        if (request.LocationId.HasValue)
            bookingQuery = bookingQuery.Where(b => b.LocationId == request.LocationId.Value);
        var checkedOutCount = await bookingQuery.CountAsync(ct);

        var totalNights = (int)(request.To.Date - request.From.Date).TotalDays + 1;
        var availableRoomNights = totalRooms * totalNights;
        var revPAR = availableRoomNights > 0 ? Math.Round(totalRevenue / availableRoomNights, 2) : 0;
        var adr = checkedOutCount > 0 ? Math.Round(totalRevenue / checkedOutCount, 2) : 0;

        return Result<RevParDto>.Success(new RevParDto(
            request.From, request.To, totalRooms, availableRoomNights, totalRevenue, revPAR, adr));
    }
}
