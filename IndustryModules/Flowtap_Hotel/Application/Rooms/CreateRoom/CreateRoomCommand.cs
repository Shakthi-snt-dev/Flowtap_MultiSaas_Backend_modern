using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Hotel.Application.Rooms.CreateRoom;

public record CreateRoomCommand(
    Guid CompanyId, Guid LocationId, string RoomNumber, int Floor,
    string Category, int Capacity, decimal BaseRate,
    string? Description, string? Amenities) : IRequest<Result<Guid>>;
