using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Food.Application.KitchenOrders.GetKitchenOrders;

public record GetKitchenOrdersQuery(
    Guid CompanyId,
    Guid? LocationId,
    string? Status) : IRequest<Result<List<KitchenOrderDto>>>;

public record KitchenOrderDto(
    Guid Id,
    Guid LocationId,
    Guid? SaleId,
    Guid? TableId,
    string? TableName,
    string OrderType,
    string Status,
    string? KotNumber,
    string? Notes,
    DateTime? PreparedAt,
    DateTime? ServedAt,
    DateTime CreatedAt,
    List<KitchenOrderItemDto> Items);

public record KitchenOrderItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal Quantity,
    string? Notes);
