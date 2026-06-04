namespace Flowtap_Food.Application.Reports.DTOs;

public record FoodDashboardDto(
    int ActiveTables,
    int AvailableTables,
    int TotalTables,
    int PendingKOTs,
    int ReadyKOTs,
    int TodayOrders,
    decimal TodayRevenue,
    List<HourlyOrderCountDto> PeakHours
);

public record HourlyOrderCountDto(int Hour, int OrderCount, decimal Revenue);

public record KitchenPerformanceDto(
    DateTime From,
    DateTime To,
    double AvgPrepTimeMinutes,
    int TotalKOTs,
    int CompletedKOTs,
    int CancelledKOTs,
    List<KOTByHourDto> ByHour
);

public record KOTByHourDto(int Hour, int KOTCount, double AvgPrepMinutes);

public record TableTurnoverDto(
    DateTime From,
    DateTime To,
    List<TableSummaryDto> Tables
);

public record TableSummaryDto(
    Guid TableId,
    string TableName,
    string? Section,
    int TotalCovers,
    decimal TotalRevenue,
    int OrderCount
);

public record RecipeProfitabilityDto(List<RecipeMarginDto> Recipes);

public record RecipeMarginDto(
    Guid RecipeId,
    string RecipeName,
    string ProductName,
    decimal EstimatedCostPrice,
    decimal DefaultSalePrice,
    decimal MarginPercent,
    int TotalIngredients
);
