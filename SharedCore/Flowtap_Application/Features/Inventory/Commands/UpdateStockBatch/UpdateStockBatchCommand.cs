using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateStockBatch;

public record UpdateStockBatchCommand(
    Guid Id,
    Guid CompanyId,
    int? Quantity = null,
    DateTime? ExpiryDate = null) : IRequest<Result<bool>>;
