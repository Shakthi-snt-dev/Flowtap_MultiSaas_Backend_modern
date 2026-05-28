using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;

public class StoreSetting : BaseEntity
{
    public Guid CompanyId { get; set; }
    public Guid LocationId { get; set; }

    // Appearance
    public string ThemeMode      { get; set; } = "light";        // light | dark | system
    public string ColorTheme     { get; set; } = "default";      // default|ocean|forest|midnight|sunset|lavender|aurora|carbon|rose
    public string AccentColor    { get; set; } = "blue";         // blue|purple|green|orange|rose|teal|amber|slate
    public string FontFamily     { get; set; } = "inter";        // inter|poppins|roboto|dm-sans|nunito
    public string BorderRadius   { get; set; } = "normal";       // sharp|normal|rounded
    public string SidebarDensity { get; set; } = "comfortable";  // comfortable|compact

    // Operations
    public bool    RequireClientOnSale      { get; set; } = false;
    public bool    AllowDiscount            { get; set; } = true;
    public decimal MaxDiscountPercent       { get; set; } = 100;
    public bool    AllowVoid                { get; set; } = true;
    public bool    RequireManagerPinForVoid { get; set; } = false;
    public bool    AutoPrintReceipt         { get; set; } = false;
    public string? ReceiptFooterText        { get; set; }
    public string? OpeningTime              { get; set; }  // "09:00"
    public string? ClosingTime              { get; set; }  // "21:00"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
