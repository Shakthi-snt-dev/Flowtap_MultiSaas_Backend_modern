using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.StoreSetting.Commands.UpsertStoreSetting;

public record UpsertStoreSettingCommand(
    Guid CompanyId,
    Guid LocationId,

    // Appearance
    string? ThemeMode = null,
    string? ColorTheme = null,
    string? AccentColor = null,
    string? FontFamily = null,
    string? BorderRadius = null,
    string? SidebarDensity = null,

    // Operations
    bool? RequireClientOnSale = null,
    bool? AllowDiscount = null,
    decimal? MaxDiscountPercent = null,
    bool? AllowVoid = null,
    bool? RequireManagerPinForVoid = null,
    bool? AutoPrintReceipt = null,
    string? ReceiptFooterText = null,
    string? OpeningTime = null,
    string? ClosingTime = null
) : IRequest<Result<bool>>;
