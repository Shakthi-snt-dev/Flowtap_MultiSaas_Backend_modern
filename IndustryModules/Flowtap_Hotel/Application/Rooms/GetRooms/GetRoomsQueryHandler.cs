using Flowtap_Application.Common.DTOs;
using Flowtap_Hotel.DbContext;
using Flowtap_Hotel.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Hotel.Application.Rooms.GetRooms;

public class GetRoomsQueryHandler(IHotelDbContext db)
    : IRequestHandler<GetRoomsQuery, Result<List<HotelRoomDto>>>
{
    public async Task<Result<List<HotelRoomDto>>> Handle(GetRoomsQuery request, CancellationToken ct)
    {
        var query = db.HotelRooms.Where(r => r.CompanyId == request.CompanyId && r.IsActive);

        if (request.LocationId.HasValue)
            query = query.Where(r => r.LocationId == request.LocationId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<RoomStatus>(request.Status, true, out var status))
            query = query.Where(r => r.Status == status);

        var rooms = await query
            .OrderBy(r => r.Floor).ThenBy(r => r.RoomNumber)
            .Select(r => new HotelRoomDto(r.Id, r.LocationId, r.RoomNumber, r.Floor,
                r.Category.ToString(), r.Status.ToString(), r.Capacity, r.BaseRate,
                r.Description, r.Amenities, r.IsActive))
            .ToListAsync(ct);

        return Result<List<HotelRoomDto>>.Success(rooms);
    }
}
