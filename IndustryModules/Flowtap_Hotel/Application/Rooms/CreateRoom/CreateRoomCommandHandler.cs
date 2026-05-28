using Flowtap_Application.Common.DTOs;
using Flowtap_Hotel.DbContext;
using Flowtap_Hotel.Domain.Entities;
using Flowtap_Hotel.Domain.Enums;
using MediatR;

namespace Flowtap_Hotel.Application.Rooms.CreateRoom;

public class CreateRoomCommandHandler(IHotelDbContext db)
    : IRequestHandler<CreateRoomCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRoomCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<RoomCategory>(request.Category, true, out var category))
            category = RoomCategory.Standard;

        var room = new HotelRoom
        {
            CompanyId   = request.CompanyId,
            LocationId  = request.LocationId,
            RoomNumber  = request.RoomNumber,
            Floor       = request.Floor,
            Category    = category,
            Capacity    = request.Capacity,
            BaseRate    = request.BaseRate,
            Description = request.Description,
            Amenities   = request.Amenities,
            Status      = RoomStatus.Available
        };

        db.HotelRooms.Add(room);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(room.Id);
    }
}
