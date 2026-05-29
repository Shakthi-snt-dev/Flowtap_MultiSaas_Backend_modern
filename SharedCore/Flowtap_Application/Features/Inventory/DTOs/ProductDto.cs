namespace Flowtap_Application.Features.Inventory.DTOs;

public record ProductDto(
    Guid Id, Guid CompanyId, Guid CategoryId, string Name, string Kind, string SKU,
    decimal DefaultCostPrice, decimal DefaultSalePrice, bool IsSerialized,
    bool IsUniversal, bool IsActive, string PublishStatus, string? Tag, string? HsnCode,
    List<ProductVariantDto> Variants, List<string> MediaUrls);

public record ProductListItemDto(
    Guid Id, string Name, string SKU, string Kind, decimal DefaultSalePrice,
    bool IsActive, string PublishStatus, string? PrimaryImageUrl,
    Guid? CategoryId, string? CategoryName, decimal StockQuantity,
    Guid? TaxSlabId = null,
    decimal? LocationSalePrice = null,      // store-specific price (null = use default)
    bool? LocationIsTaxIncluded = null,     // store-specific tax inclusion type
    Guid? LocationTaxSlabId = null);        // store-specific tax slab (null = use product default)

public record ProductVariantDto(Guid Id, string Name, string SKU, bool IsActive, decimal? AdditionalPrice);

public record WarehouseDto(
    Guid Id, Guid CompanyId, string Code, string Name, string Type,
    string Status, string City, string Country, bool IsActive, bool HasRackSystem,
    Guid? ManagerEmployeeId = null,
    Guid? LocationId = null);

public record StockLevelDto(
    Guid ProductId, string ProductName, string SKU, Guid WarehouseId, string WarehouseName,
    decimal Quantity, decimal InTransitQuantity, decimal ReservedQuantity, decimal ReorderLevel);

public record TransferDto(
    Guid Id, Guid CompanyId, string? TransferNumber, string Status,
    Guid FromWarehouseId, string FromWarehouseName,
    Guid ToWarehouseId, string ToWarehouseName,
    DateTime CreatedAt, DateTime? ShippedAt);

public record WriteOffDto(
    Guid Id, Guid ProductId, string ProductName, Guid WarehouseId, string WarehouseName,
    string WriteOffNumber, decimal Quantity, string WriteOffType, string Reason, string Status, DateTime CreatedAt);

public record ReorderAlertDto(
    Guid Id, Guid ProductId, string ProductName, Guid WarehouseId,
    int CurrentQuantity, int ReorderLevel, string Severity, bool IsHandled);

public record ReorderRuleDto(
    Guid   Id,
    Guid   ProductId,
    string ProductName,
    Guid   WarehouseId,
    string WarehouseName,
    decimal MinimumQuantity,
    decimal ReorderQuantity,
    int?   LeadTimeDays,
    bool   IsActive);

public record ProductLocationPriceDto(
    Guid Id, Guid ProductId, Guid LocationId, decimal CostPrice,
    decimal SalePrice, decimal? MRP, bool IsTaxIncluded, string Status, bool IsActive,
    Guid? TaxSlabId = null);    // store-specific tax slab
