using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Food.Application.KitchenOrders.CreateKitchenOrder;

public record CreateKitchenOrderCommand(
    Guid CompanyId,
    Guid LocationId,
    Guid? SaleId,
    Guid? TableId,
    string OrderType,
    string? Notes,
    List<CreateKOTItemDto> Items) : IRequest<Result<Guid>>;

public record CreateKOTItemDto(
    Guid ProductId,
    string ProductName,
    decimal Quantity,
    string? Notes);
