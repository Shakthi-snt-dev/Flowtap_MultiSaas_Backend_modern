using Flowtap_Application.Common.Interfaces;
using Flowtap_Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Hotel.DbContext;

public interface IHotelDbContext : IApplicationDbContext
{
    DbSet<HotelRoom> HotelRooms { get; }
    DbSet<HotelBooking> HotelBookings { get; }
}
