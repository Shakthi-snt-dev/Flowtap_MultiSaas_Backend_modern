using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.UpsertLocationInventorySettings;

public record UpsertLocationInventorySettingsCommand(
    Guid CompanyId,
    Guid LocationId,
    bool? EnableBinTracking = null,
    bool? AllowNegativeStock = null,
    bool? EnableAutoReorder = null,
    string? ReorderNotificationEmail = null) : IRequest<Result<bool>>;
