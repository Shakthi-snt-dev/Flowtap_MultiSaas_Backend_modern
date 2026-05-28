using Flowtap_Domain.SharedKernel;
using Flowtap_Hotel.Domain.Enums;

namespace Flowtap_Hotel.Domain.Entities;

public class HotelBooking : TenantEntity
{
    public Guid LocationId { get; set; }
    public Guid RoomId { get; set; }
    public HotelRoom Room { get; set; } = null!;
    public Guid? ClientId { get; set; }             // links to shared Sales.Client
    public string GuestName { get; set; } = string.Empty;
    public string? GuestPhone { get; set; }
    public string? GuestEmail { get; set; }
    public string? GuestIdNumber { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public DateTime? ActualCheckIn { get; set; }
    public DateTime? ActualCheckOut { get; set; }
    public int GuestCount { get; set; } = 1;
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string? Notes { get; set; }
    public string? BookingReference { get; set; }
}
