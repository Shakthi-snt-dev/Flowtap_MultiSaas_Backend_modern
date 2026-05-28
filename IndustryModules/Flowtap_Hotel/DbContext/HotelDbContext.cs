using Flowtap_Application.Common.Interfaces;
using Flowtap_Hotel.Domain.Entities;
using Flowtap_Infrastructure.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Flowtap_Hotel.DbContext;

/// <summary>
/// Extends the shared ApplicationDbContext with Hotel-industry entities.
/// Registered as IHotelDbContext in the Hotel API DI container.
/// </summary>
public class HotelDbContext(
    DbContextOptions<HotelDbContext> options,
    ICurrentUserService currentUser,
    IPublisher publisher)
    : ApplicationDbContext(ChangeOptionsType<ApplicationDbContext>(options), currentUser, publisher),
      IHotelDbContext
{
    public DbSet<HotelRoom> HotelRooms => Set<HotelRoom>();
    public DbSet<HotelBooking> HotelBookings => Set<HotelBooking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<HotelRoom>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasMany(x => x.Bookings).WithOne(x => x.Room).HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HotelBooking>(b => b.HasKey(x => x.Id));
    }

    // Helper: convert DbContextOptions<TDerived> → DbContextOptions<TBase>
    private static DbContextOptions<TBase> ChangeOptionsType<TBase>(DbContextOptions options)
        where TBase : Microsoft.EntityFrameworkCore.DbContext
    {
        var builder = new DbContextOptionsBuilder<TBase>();
        foreach (var extension in options.Extensions)
            ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(extension);
        return builder.Options;
    }
}
