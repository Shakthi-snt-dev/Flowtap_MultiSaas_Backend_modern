namespace Flowtap_Repair.Application.Reports.DTOs;

public record RepairDashboardDto(
    int OpenTickets,
    int PendingPickup,
    int OverdueTickets,
    int ClosedThisMonth,
    decimal RevenueThisMonth,
    List<TicketsByStatusDto> StatusBreakdown
);

public record TicketsByStatusDto(string Status, int Count);

public record TicketStatusReportDto(
    DateTime From,
    DateTime To,
    int Total,
    int Open,
    int InProgress,
    int WaitingForParts,
    int Done,
    int Cancelled,
    double AvgResolutionDays,
    List<DailyTicketCountDto> ByDay
);

public record DailyTicketCountDto(DateTime Date, int Opened, int Closed);

public record TechnicianPerformanceDto(DateTime From, DateTime To, List<TechSummaryDto> Technicians);

public record TechSummaryDto(
    Guid EmployeeId,
    string EmployeeName,
    int TicketsClosed,
    double AvgResolutionDays,
    decimal RevenueGenerated
);

public record PartConsumptionDto(DateTime From, DateTime To, List<PartUsageDto> Parts);

public record PartUsageDto(
    Guid ProductId,
    string ProductName,
    string? SKU,
    decimal TotalUsed,
    decimal TotalCost
);
