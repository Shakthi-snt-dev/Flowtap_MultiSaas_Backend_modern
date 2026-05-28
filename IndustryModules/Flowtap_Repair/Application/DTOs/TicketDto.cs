namespace Flowtap_Repair.Application.DTOs;

public record TicketDto(
    Guid Id, Guid CompanyId, Guid LocationId, Guid ClientId,
    string? TicketNumber, string Type, string Status, string Priority,
    Guid? PrimaryServiceId,
    Guid? ExecutorEmployeeId, Guid? ManagerEmployeeId,
    DateTime CreatedAt, DateTime? Deadline, DateTime? ClosedAt,
    // Device info
    string? DeviceType, string? DeviceBrand, string? DeviceModel, string? DeviceSerial,
    string? DeviceModification, string? Appearance, string? Password, string? Equipment,
    // Notes & checklists
    string? Reason, string? MastersNotes, string? PreRepairChecklist, string? AccessoryList,
    // Financials
    decimal EstimatedCost, decimal Prepayment, decimal TotalCost, bool IsPaid,
    string? PrepaymentMethod, DateTime? PrepaymentPaidAt,
    // Related sale
    Guid? SaleId,
    // Timer
    bool IsTimerRunning, long TotalTimeSpentSeconds,
    // Line items
    List<TicketItemDto> Items,
    // Payments (advance payments linked to this ticket)
    List<TicketPaymentDto> Payments,
    string? ClientName = null,
    string? ClientPhone = null,
    string? ClientEmail = null,
    string? TechnicianName = null,
    string? ManagerName = null);

public record TicketListDto(
    Guid Id, Guid ClientId, string? TicketNumber,
    string Type, string Status, string Priority,
    string? DeviceBrand, string? DeviceModel, string? DeviceSerial,
    Guid? ExecutorEmployeeId, Guid? ManagerEmployeeId,
    DateTime CreatedAt, DateTime? Deadline,
    decimal EstimatedCost, decimal TotalCost, bool IsPaid,
    bool IsTimerRunning, long TotalTimeSpentSeconds,
    string? ClientName = null,
    string? TechnicianName = null,
    string? ManagerName = null);

public record TicketItemDto(
    Guid Id, Guid ItemReferenceId, string Name, string Type,
    decimal Quantity, decimal Price, decimal Cost, decimal DiscountAmount, decimal TaxPercent);

public record TicketPaymentDto(
    Guid Id, decimal Amount, string Method, string Purpose,
    string? ExternalReference, string? Comment, DateTime PaidAt,
    Guid? SaleId);   // null = advance not yet linked to a sale

public record ServiceDto(
    Guid Id, Guid CompanyId, string Name, string? Description,
    decimal BasePrice, bool IsActive, bool IsUniversal, Guid? ServiceCategoryId);

public record TaskDto(
    Guid Id, Guid CompanyId, Guid LocationId,
    Guid AuthorEmployeeId, Guid AssigneeEmployeeId,
    Guid? TicketId,
    string Title, string Description, string Status, string Priority,
    DateTime CreatedAt, DateTime? Deadline, DateTime? CompletedAt,
    List<string> Tags,
    bool IsTimerRunning, long TotalTimeSpentSeconds,
    string AuthorName = "",
    string AssigneeName = "");

public record TimeLogDto(Guid Id, DateTime StartedAt, DateTime? StoppedAt, Guid EmployeeId);

