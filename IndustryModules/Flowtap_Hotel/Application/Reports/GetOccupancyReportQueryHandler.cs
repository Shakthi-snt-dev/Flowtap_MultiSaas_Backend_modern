using Flowtap_Application.Common.DTOs;
using Flowtap_Hotel.Application.Reports.DTOs;
using Flowtap_Hotel.DbContext;
using Flowtap_Hotel.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Hotel.Application.Reports;

public class GetOccupancyReportQueryHandler(IHotelDbContext db)
    : IRequestHandler<GetOccupancyReportQuery, Result<OccupancyReportDto>>
{
    public async Task<Result<OccupancyReportDto>> Handle(GetOccupancyReportQuery request, CancellationToken ct)
    {
        var roomQuery = db.HotelRooms.Where(r => r.CompanyId == request.CompanyId && r.IsActive);
        if (request.LocationId.HasValue)
            roomQuery = roomQuery.Where(r => r.LocationId == request.LocationId.Value);

        var rooms = await roomQuery.Select(r => new { r.Id, r.Category }).ToListAsync(ct);

        var bookingQuery = db.HotelBookings
            .Where(b => b.CompanyId == request.CompanyId
                     && b.Status != BookingStatus.Cancelled
                     && b.CheckInDate.Date <= request.To.Date
                     && b.CheckOutDate.Date >= request.From.Date);
        if (request.LocationId.HasValue)
            bookingQuery = bookingQuery.Where(b => b.LocationId == request.LocationId.Value);

        var bookings = await bookingQuery
            .Select(b => new { b.RoomId, b.CheckInDate, b.CheckOutDate })
            .ToListAsync(ct);

        var totalNights = (int)(request.To.Date - request.From.Date).TotalDays + 1;

        var byCategory = rooms
            .GroupBy(r => r.Category)
            .Select(g =>
            {
                var roomIds = g.Select(r => r.Id).ToHashSet();
                var occupiedNights = bookings
                    .Where(b => roomIds.Contains(b.RoomId))
                    .Sum(b =>
                    {
                        var overlapStart = b.CheckInDate.Date < request.From.Date ? request.From.Date : b.CheckInDate.Date;
                        var overlapEnd   = b.CheckOutDate.Date > request.To.Date  ? request.To.Date  : b.CheckOutDate.Date;
                        return Math.Max(0, (int)(overlapEnd - overlapStart).TotalDays);
                    });
                var availNights = g.Count() * totalNights;
                var pct = availNights > 0 ? Math.Round((decimal)occupiedNights / availNights * 100, 1) : 0;
                return new RoomTypeOccupancyDto(g.Key.ToString(), g.Count(), occupiedNights, availNights, pct);
            })
            .ToList();

        var totalAvail = rooms.Count * totalNights;
        var totalOccupied = byCategory.Sum(c => c.OccupiedNights);
        var overallPct = totalAvail > 0 ? Math.Round((decimal)totalOccupied / totalAvail * 100, 1) : 0;

        return Result<OccupancyReportDto>.Success(new OccupancyReportDto(request.From, request.To, overallPct, byCategory));
    }
}
