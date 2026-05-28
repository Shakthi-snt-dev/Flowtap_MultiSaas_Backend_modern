using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Purchase.Commands.CreatePurchaseOrder;

public record CreatePurchaseOrderCommand(
    Guid CompanyId, Guid SupplierId, Guid WarehouseId, Guid? LocationId,
    DateTime? ExpectedDeliveryDate, string Currency, Guid CreatedByEmployeeId,
    List<PurchaseOrderItemDto> Items) : IRequest<Result<Guid>>;

public record PurchaseOrderItemDto(Guid ProductId, int Quantity, decimal UnitCost, decimal TaxPercent);
