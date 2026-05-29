using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

namespace Flowtap_Food.Industry;

public static class FoodIceCreamSeeder
{
    public static async Task SeedAsync(IApplicationDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedCategories(context, companyId);
        await context.SaveChangesAsync(ct);
    }

    private static void SeedCategories(IApplicationDbContext context, Guid companyId)
    {
        // ── Ice Cream ────────────────────────────────────────────────────────────
        var iceCream = new ProductCategory { CompanyId = companyId, Name = "Ice Cream", SortOrder = 1, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(iceCream);

        var scoopsCat     = new ProductCategory { CompanyId = companyId, Name = "Scoops",       ParentCategoryId = iceCream.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var sundaesCat    = new ProductCategory { CompanyId = companyId, Name = "Sundaes",       ParentCategoryId = iceCream.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var conesCat      = new ProductCategory { CompanyId = companyId, Name = "Cones & Cups",  ParentCategoryId = iceCream.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var milkshakesCat = new ProductCategory { CompanyId = companyId, Name = "Milkshakes",    ParentCategoryId = iceCream.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(scoopsCat, sundaesCat, conesCat, milkshakesCat);

        // ── Toppings & Add-ons ───────────────────────────────────────────────────
        var toppings = new ProductCategory { CompanyId = companyId, Name = "Toppings & Add-ons", SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.Add(toppings);

        // ── Combos ───────────────────────────────────────────────────────────────
        var combosCat = new ProductCategory { CompanyId = companyId, Name = "Combo Deals", SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.Add(combosCat);

        // ── Raw Materials ────────────────────────────────────────────────────────
        var rawMat = new ProductCategory { CompanyId = companyId, Name = "Raw Materials", SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.Add(rawMat);

        // ── Final Products ───────────────────────────────────────────────────────
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = scoopsCat.Id,     Name = "Belgian Chocolate Scoop",      Kind = ProductKind.FinalProduct, SKU = "ICE-SC-BCH", DefaultCostPrice = 25m,  DefaultSalePrice = 79m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = scoopsCat.Id,     Name = "Vanilla Bean Scoop",           Kind = ProductKind.FinalProduct, SKU = "ICE-SC-VNL", DefaultCostPrice = 20m,  DefaultSalePrice = 69m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = scoopsCat.Id,     Name = "Strawberry Scoop",             Kind = ProductKind.FinalProduct, SKU = "ICE-SC-STR", DefaultCostPrice = 22m,  DefaultSalePrice = 69m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = scoopsCat.Id,     Name = "Mango Sorbet Scoop",           Kind = ProductKind.FinalProduct, SKU = "ICE-SC-MNG", DefaultCostPrice = 28m,  DefaultSalePrice = 89m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = sundaesCat.Id,    Name = "Hot Fudge Brownie Sundae",     Kind = ProductKind.FinalProduct, SKU = "ICE-SN-HFB", DefaultCostPrice = 60m,  DefaultSalePrice = 179m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = sundaesCat.Id,    Name = "Banana Split Sundae",          Kind = ProductKind.FinalProduct, SKU = "ICE-SN-BNS", DefaultCostPrice = 55m,  DefaultSalePrice = 169m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = conesCat.Id,      Name = "Waffle Cone (Double Scoop)",   Kind = ProductKind.FinalProduct, SKU = "ICE-CN-WFC", DefaultCostPrice = 40m,  DefaultSalePrice = 119m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = conesCat.Id,      Name = "Sugar Cone (Single Scoop)",    Kind = ProductKind.FinalProduct, SKU = "ICE-CN-SGR", DefaultCostPrice = 25m,  DefaultSalePrice = 79m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = milkshakesCat.Id, Name = "Strawberry Milkshake (Large)", Kind = ProductKind.FinalProduct, SKU = "ICE-MK-STR", DefaultCostPrice = 50m,  DefaultSalePrice = 149m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = milkshakesCat.Id, Name = "Chocolate Milkshake (Large)",  Kind = ProductKind.FinalProduct, SKU = "ICE-MK-CHC", DefaultCostPrice = 55m,  DefaultSalePrice = 159m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = combosCat.Id,     Name = "Family Combo (4 Scoops)",      Kind = ProductKind.FinalProduct, SKU = "ICE-CM-FM4", DefaultCostPrice = 80m,  DefaultSalePrice = 249m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );

        // ── Toppings (FinalProduct — sold as add-ons) ────────────────────────────
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = toppings.Id, Name = "Chocolate Sauce",    Kind = ProductKind.FinalProduct, SKU = "ICE-TP-CHS", DefaultCostPrice = 5m, DefaultSalePrice = 19m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = toppings.Id, Name = "Rainbow Sprinkles",  Kind = ProductKind.FinalProduct, SKU = "ICE-TP-SPR", DefaultCostPrice = 3m, DefaultSalePrice = 15m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );

        // ── Raw Materials ────────────────────────────────────────────────────────
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = rawMat.Id, Name = "Full Cream Milk (per litre)",     Kind = ProductKind.RawMaterial, SKU = "ICE-RM-MLK", DefaultCostPrice = 24m,  DefaultSalePrice = 24m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMat.Id, Name = "Heavy Cream (per litre)",         Kind = ProductKind.RawMaterial, SKU = "ICE-RM-CRM", DefaultCostPrice = 80m,  DefaultSalePrice = 80m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMat.Id, Name = "Caster Sugar (per kg)",           Kind = ProductKind.RawMaterial, SKU = "ICE-RM-SGR", DefaultCostPrice = 45m,  DefaultSalePrice = 45m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMat.Id, Name = "Vanilla Extract (per 100ml)",     Kind = ProductKind.RawMaterial, SKU = "ICE-RM-VNL", DefaultCostPrice = 90m,  DefaultSalePrice = 90m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMat.Id, Name = "Cocoa Powder (per kg)",           Kind = ProductKind.RawMaterial, SKU = "ICE-RM-COC", DefaultCostPrice = 180m, DefaultSalePrice = 180m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMat.Id, Name = "Waffle Cones (pack of 50)",       Kind = ProductKind.RawMaterial, SKU = "ICE-RM-WFC", DefaultCostPrice = 150m, DefaultSalePrice = 150m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft }
        );
    }
}
