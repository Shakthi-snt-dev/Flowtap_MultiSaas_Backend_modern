namespace Flowtap_Jewelry.Application.Reports.DTOs;

public record JewelryDashboardDto(
    int TodayOrders,
    decimal TodayRevenue,
    decimal MonthRevenue,
    int TodayExchanges,
    decimal TodayExchangeValue,
    List<MetalStockSummaryDto> MetalStockSummary
);

public record MetalStockSummaryDto(string MetalType, decimal TotalWeightGrams, decimal TotalValue);

public record MetalStockReportDto(List<MetalStockDetailDto> Items);

public record MetalStockDetailDto(
    Guid ProductId,
    string ProductName,
    string? SKU,
    string MetalType,
    decimal StockQuantity,
    decimal DefaultCostPrice,
    decimal TotalValue
);

public record ExchangeReportDto(
    DateTime From,
    DateTime To,
    int TotalExchanges,
    decimal TotalWeightGrams,
    decimal TotalExchangeValue,
    List<ExchangeItemDto> Items
);

public record ExchangeItemDto(
    Guid TransactionId,
    string? ClientName,
    string MetalType,
    string Purity,
    decimal WeightGrams,
    decimal RatePerGram,
    decimal TotalValue,
    DateTime CreatedAt
);
