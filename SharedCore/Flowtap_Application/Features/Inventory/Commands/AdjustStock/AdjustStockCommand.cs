using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.AdjustStock;

public record AdjustStockCommand(
    Guid CompanyId, Guid WarehouseId, Guid ProductId,
    string AdjustmentType, decimal Quantity, string Reason,
    string ReasonCode, string? Notes, Guid? AdjustedByEmployeeId) : IRequest<Result<Guid>>;
