using Flowtap_Application.Features.Sales.Commands.CreateSale;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;

namespace Flowtap_Food.Application.Sales;

/// <summary>
/// Food-industry sale request — only exposes fields relevant to food POS.
/// Table assignment and order type are food-specific; all other industries never see these.
/// Mapped to CreateSaleCommand in FoodController.
/// </summary>
public record FoodCreateSaleRequest(
    Guid LocationId,
    Guid? ClientId,
    string Source,
    string? Notes,
    string? IdempotencyKey,
    List<CreateSaleItemDto> Items,
    List<CreateSalePaymentDto>? Payments = null,
    Guid? EmployeeId = null,
    // Food-specific fields
    Guid? TableId = null,
    FoodOrderType? FoodOrderType = null
);
