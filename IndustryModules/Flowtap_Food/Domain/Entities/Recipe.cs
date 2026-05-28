using Flowtap_Domain.SharedKernel;

namespace Flowtap_Food.Domain.Entities;

/// <summary>
/// Maps a menu item (Product) to the raw materials (Products with Kind=RawMaterial) needed to produce it.
/// </summary>
public class Recipe : TenantEntity
{
    public Guid ProductId { get; set; }                   // the finished menu item
    public string Name { get; set; } = string.Empty;
    public int YieldQuantity { get; set; } = 1;           // how many portions this recipe produces
    public string? Instructions { get; set; }
    public ICollection<RecipeIngredient> Ingredients { get; set; } = [];
}
