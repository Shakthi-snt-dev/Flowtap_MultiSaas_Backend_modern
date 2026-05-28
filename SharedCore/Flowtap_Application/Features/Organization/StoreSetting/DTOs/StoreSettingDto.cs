namespace Flowtap_Application.Features.Organization.StoreSetting.DTOs;

public record StoreSettingDto(
    Guid Id,
    Guid CompanyId,
    Guid LocationId,

    // Appearance
    string ThemeMode,
    string ColorTheme,
    string AccentColor,
    string FontFamily,
    string BorderRadius,
    string SidebarDensity,

    // Operations
    bool RequireClientOnSale,
    bool AllowDiscount,
    decimal MaxDiscountPercent,
    bool AllowVoid,
    bool RequireManagerPinForVoid,
    bool AutoPrintReceipt,
    string? ReceiptFooterText,
    string? OpeningTime,
    string? ClosingTime
);
