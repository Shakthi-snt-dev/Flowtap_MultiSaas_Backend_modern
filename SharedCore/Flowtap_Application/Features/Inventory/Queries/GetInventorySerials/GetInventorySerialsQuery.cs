using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetInventorySerials;

public record InventorySerialDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    Guid WarehouseId,
    string WarehouseName,
    string? ManufacturerSerial,
    string CompanySerial,
    string DisplayName,
    bool IsSold,
    bool IsReturned,
    bool IsActive,
    DateTime? WarrantyStartDate,
    DateTime? WarrantyEndDate);

public record GetInventorySerialsQuery(
    Guid CompanyId,
    Guid? ProductId = null,
    Guid? WarehouseId = null,
    bool? IsSold = null,
    string? SerialNumber = null,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<InventorySerialDto>>>;
