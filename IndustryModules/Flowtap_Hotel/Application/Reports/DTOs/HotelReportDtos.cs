namespace Flowtap_Hotel.Application.Reports.DTOs;

public record HotelDashboardDto(
    int TotalRooms,
    int OccupiedRooms,
    int AvailableRooms,
    int ReservedRooms,
    decimal OccupancyPercent,
    int TodayCheckIns,
    int TodayCheckOuts,
    decimal TodayRevenue
);

public record OccupancyReportDto(
    DateTime From,
    DateTime To,
    decimal OverallOccupancyPercent,
    List<RoomTypeOccupancyDto> ByRoomCategory
);

public record RoomTypeOccupancyDto(
    string Category,
    int TotalRooms,
    int OccupiedNights,
    int AvailableNights,
    decimal OccupancyPercent
);

public record BookingReportDto(
    DateTime From,
    DateTime To,
    int TotalBookings,
    int Arrivals,
    int Departures,
    int Cancellations,
    int NoShows,
    decimal TotalRevenue,
    List<DailyBookingDto> ByDay
);

public record DailyBookingDto(DateTime Date, int CheckIns, int CheckOuts, int NewBookings);

public record RevParDto(
    DateTime From,
    DateTime To,
    int TotalRooms,
    int AvailableRoomNights,
    decimal TotalRoomRevenue,
    decimal RevPAR,
    decimal AverageDailyRate
);
