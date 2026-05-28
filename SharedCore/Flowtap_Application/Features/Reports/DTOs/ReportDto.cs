namespace Flowtap_Application.Features.Reports.DTOs;

public record SalesReportDto(
    DateTime From, DateTime To, decimal TotalRevenue, decimal TotalTax,
    int TotalTransactions, List<SalesSummaryDto> DailySummary);

public record SalesSummaryDto(DateOnly Date, decimal Revenue, int Transactions);

public record InventoryReportDto(
    int TotalProducts, int LowStockItems, int OutOfStockItems,
    decimal TotalInventoryValue, List<StockSummaryDto> Items);

public record StockSummaryDto(Guid ProductId, string ProductName, string SKU, decimal Quantity, decimal Value);

public record EmployeeReportDto(
    Guid EmployeeId, string Name, int TicketsHandled, int TasksCompleted,
    decimal TotalSalesAmount, decimal SalaryEarned);

public record DashboardStatsDto(
    decimal TotalRevenueTodayCount, int TransactionsToday, int NewClientsThisMonth,
    int OpenTickets, int LowStockAlerts, decimal TotalRevenueThisMonth);

public record TopProductDto(
    Guid ProductId, string ProductName, string SKU, int QuantitySold, decimal Revenue);

public record TopProductsReportDto(DateTime From, DateTime To, List<TopProductDto> Products);

public record AdminOverviewDto(
    // Business identity
    string TenantName,
    string BusinessType,
    string Country,
    string Currency,
    List<string> ActiveModules,

    // Counts
    int TotalStores,
    int TotalEmployees,
    int TotalProducts,
    decimal TotalStockQuantity,
    int TotalClients,
    int OpenTickets,

    // Revenue
    decimal RevenueToday,
    decimal RevenueThisMonth,
    decimal RevenueAllTime,
    int TotalSalesCount,

    // Per-store breakdown
    List<StoreOverviewDto> Stores,

    // Subscription
    SubscriptionSummaryDto? Subscription
);

public record StoreOverviewDto(
    Guid Id, string Name, string CountryCode, string CurrencyCode,
    int EmployeeCount, int StockItemCount, bool IsActive,
    decimal RevenueToday, decimal RevenueThisMonth, decimal RevenueAllTime, int SalesCount);

public record SubscriptionSummaryDto(
    string PlanName, string Status,
    DateTime StartDate, DateTime EndDate,
    DateTime NextBillingDate, int TotalLocations);
