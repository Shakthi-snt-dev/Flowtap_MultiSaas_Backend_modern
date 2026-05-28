using Flowtap_Domain.SharedKernel;
using Flowtap_Hotel.Domain.Enums;

namespace Flowtap_Hotel.Domain.Entities;

public class HotelRoom : TenantEntity
{
    public Guid LocationId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int Floor { get; set; }
    public RoomCategory Category { get; set; } = RoomCategory.Standard;
    public RoomStatus Status { get; set; } = RoomStatus.Available;
    public int Capacity { get; set; } = 2;
    public decimal BaseRate { get; set; }               // per night
    public string? Description { get; set; }
    public string? Amenities { get; set; }              // JSON or comma-separated
    public ICollection<HotelBooking> Bookings { get; set; } = [];
}
