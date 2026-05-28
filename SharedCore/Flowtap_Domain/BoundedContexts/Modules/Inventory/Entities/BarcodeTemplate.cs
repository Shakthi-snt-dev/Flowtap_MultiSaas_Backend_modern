using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class BarcodeTemplate : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string CodeType { get; set; } = "Code128";
    public bool IsDefault { get; set; }
    public int LabelsPerRow { get; set; } = 3;
    public decimal LabelWidthMm { get; set; } = 60;
    public decimal LabelHeightMm { get; set; } = 40;
    public decimal MarginMm { get; set; } = 2;
    public decimal PaddingMm { get; set; } = 2;
    public bool ShowProductName { get; set; } = true;
    public bool ShowSKU { get; set; } = true;
}
