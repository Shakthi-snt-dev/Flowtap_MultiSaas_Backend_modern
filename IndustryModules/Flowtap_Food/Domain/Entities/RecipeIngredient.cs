using Flowtap_Domain.SharedKernel;

namespace Flowtap_Food.Domain.Entities;

public class RecipeIngredient : AuditableEntity
{
    public Guid RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
    public Guid RawMaterialProductId { get; set; }        // Product with Kind=RawMaterial
    public string RawMaterialName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;      // e.g. "grams", "ml", "pcs"
}
