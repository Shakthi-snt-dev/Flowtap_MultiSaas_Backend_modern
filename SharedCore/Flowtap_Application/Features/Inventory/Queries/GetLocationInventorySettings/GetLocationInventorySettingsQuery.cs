using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetLocationInventorySettings;

public record LocationInventorySettingsDto(
    Guid Id,
    Guid LocationId,
    bool EnableBinTracking,
    bool AllowNegativeStock,
    bool EnableAutoReorder,
    string? ReorderNotificationEmail);

public record GetLocationInventorySettingsQuery(Guid CompanyId, Guid LocationId) : IRequest<Result<LocationInventorySettingsDto?>>;
