using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.CreateInventorySerial;

public record CreateInventorySerialCommand(
    Guid CompanyId,
    Guid ProductId,
    Guid WarehouseId,
    string? ManufacturerSerial,
    string? CompanySerial = null,
    string? DisplayName = null,
    DateTime? WarrantyStartDate = null,
    DateTime? WarrantyEndDate = null) : IRequest<Result<Guid>>;
