namespace Flowtap_Medical.Application.Reports.DTOs;

public record MedicalDashboardDto(
    int TotalPatients,
    int TodayAppointments,
    int PendingAppointments,
    int CompletedAppointments,
    decimal TodayRevenue,
    decimal MonthRevenue
);

public record PatientVisitReportDto(
    DateTime From,
    DateTime To,
    int TotalVisits,
    int NewPatients,
    int RepeatPatients,
    List<DailyVisitDto> ByDay
);

public record DailyVisitDto(DateTime Date, int Visits, int NewPatients);

public record MedicineConsumptionDto(
    DateTime From,
    DateTime To,
    List<MedicineUsageDto> Medicines
);

public record MedicineUsageDto(
    Guid ProductId,
    string MedicineName,
    string? SKU,
    decimal QuantityDispensed,
    decimal Revenue
);
