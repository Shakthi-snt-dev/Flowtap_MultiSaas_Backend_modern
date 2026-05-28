using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateInventorySerial;

public record UpdateInventorySerialCommand(
    Guid Id,
    Guid CompanyId,
    bool? IsSold = null,
    bool? IsReturned = null,
    bool? IsActive = null,
    DateTime? WarrantyEndDate = null) : IRequest<Result<bool>>;
