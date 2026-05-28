using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Hotel.Application.Rooms.GetRooms;

public record GetRoomsQuery(Guid CompanyId, Guid? LocationId, string? Status) : IRequest<Result<List<HotelRoomDto>>>;

public record HotelRoomDto(
    Guid Id, Guid LocationId, string RoomNumber, int Floor,
    string Category, string Status, int Capacity, decimal BaseRate,
    string? Description, string? Amenities, bool IsActive);
