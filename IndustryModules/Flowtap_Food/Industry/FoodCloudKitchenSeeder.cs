using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

namespace Flowtap_Food.Industry;

public static class FoodCloudKitchenSeeder
{
    public static async Task SeedAsync(IApplicationDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedCategories(context, companyId);
        await context.SaveChangesAsync(ct);
    }

    private static void SeedCategories(IApplicationDbContext context, Guid companyId)
    {
        // ── Delivery Menu ────────────────────────────────────────────────────────
        var deliveryMenu = new ProductCategory { CompanyId = companyId, Name = "Delivery Menu", SortOrder = 1, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(deliveryMenu);

        var mealsCat    = new ProductCategory { CompanyId = companyId, Name = "Meals & Combos",    ParentCategoryId = deliveryMenu.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var snacksCat   = new ProductCategory { CompanyId = companyId, Name = "Snacks & Starters", ParentCategoryId = deliveryMenu.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var dessertsCat = new ProductCategory { CompanyId = companyId, Name = "Desserts",           ParentCategoryId = deliveryMenu.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var drinksCat   = new ProductCategory { CompanyId = companyId, Name = "Drinks",             ParentCategoryId = deliveryMenu.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(mealsCat, snacksCat, dessertsCat, drinksCat);

        // ── Packaging ────────────────────────────────────────────────────────────
        var packagingCat = new ProductCategory { CompanyId = companyId, Name = "Packaging Materials", SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.Add(packagingCat);

        // ── Raw Materials ────────────────────────────────────────────────────────
        var rawMaterialsCat = new ProductCategory { CompanyId = companyId, Name = "Raw Materials", SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.Add(rawMaterialsCat);

        // ── Final Products ───────────────────────────────────────────────────────
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = mealsCat.Id,    Name = "Grilled Chicken Rice Box",     Kind = ProductKind.FinalProduct, SKU = "CKD-ML-GCR", DefaultCostPrice = 120m, DefaultSalePrice = 299m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = mealsCat.Id,    Name = "Veg Biryani Box",              Kind = ProductKind.FinalProduct, SKU = "CKD-ML-VBR", DefaultCostPrice = 80m,  DefaultSalePrice = 199m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = mealsCat.Id,    Name = "Paneer Butter Masala Meal",    Kind = ProductKind.FinalProduct, SKU = "CKD-ML-PBM", DefaultCostPrice = 110m, DefaultSalePrice = 279m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = mealsCat.Id,    Name = "Butter Chicken Combo Box",     Kind = ProductKind.FinalProduct, SKU = "CKD-ML-BCH", DefaultCostPrice = 130m, DefaultSalePrice = 329m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = snacksCat.Id,   Name = "Crispy Chicken Wings (6 pcs)", Kind = ProductKind.FinalProduct, SKU = "CKD-SN-CCW", DefaultCostPrice = 90m,  DefaultSalePrice = 229m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = snacksCat.Id,   Name = "Veg Spring Rolls (6 pcs)",     Kind = ProductKind.FinalProduct, SKU = "CKD-SN-VSR", DefaultCostPrice = 50m,  DefaultSalePrice = 149m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = snacksCat.Id,   Name = "Paneer Tikka (6 pcs)",         Kind = ProductKind.FinalProduct, SKU = "CKD-SN-PTK", DefaultCostPrice = 70m,  DefaultSalePrice = 179m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = dessertsCat.Id, Name = "Gulab Jamun (4 pcs)",          Kind = ProductKind.FinalProduct, SKU = "CKD-DS-GJM", DefaultCostPrice = 30m,  DefaultSalePrice = 89m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = dessertsCat.Id, Name = "Rasmalai (2 pcs)",             Kind = ProductKind.FinalProduct, SKU = "CKD-DS-RSM", DefaultCostPrice = 35m,  DefaultSalePrice = 99m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = drinksCat.Id,   Name = "Mango Lassi",                  Kind = ProductKind.FinalProduct, SKU = "CKD-DK-MLS", DefaultCostPrice = 20m,  DefaultSalePrice = 69m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = drinksCat.Id,   Name = "Masala Buttermilk",            Kind = ProductKind.FinalProduct, SKU = "CKD-DK-MBM", DefaultCostPrice = 10m,  DefaultSalePrice = 39m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );

        // ── Packaging (RawMaterial) ──────────────────────────────────────────────
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = packagingCat.Id, Name = "Disposable Food Box (Pack 50)",     Kind = ProductKind.RawMaterial, SKU = "CKD-PK-FBX", DefaultCostPrice = 90m,  DefaultSalePrice = 90m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = packagingCat.Id, Name = "Paper Bags (Pack 100)",             Kind = ProductKind.RawMaterial, SKU = "CKD-PK-PBG", DefaultCostPrice = 60m,  DefaultSalePrice = 60m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = packagingCat.Id, Name = "Sauce Cups (Pack 100)",             Kind = ProductKind.RawMaterial, SKU = "CKD-PK-SCC", DefaultCostPrice = 30m,  DefaultSalePrice = 30m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft }
        );

        // ── Raw Materials ────────────────────────────────────────────────────────
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = rawMaterialsCat.Id, Name = "Chicken Breast (per kg)",  Kind = ProductKind.RawMaterial, SKU = "CKD-RM-CHB", DefaultCostPrice = 200m, DefaultSalePrice = 200m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMaterialsCat.Id, Name = "Basmati Rice (per kg)",   Kind = ProductKind.RawMaterial, SKU = "CKD-RM-BSR", DefaultCostPrice = 60m,  DefaultSalePrice = 60m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMaterialsCat.Id, Name = "Paneer (per kg)",         Kind = ProductKind.RawMaterial, SKU = "CKD-RM-PNR", DefaultCostPrice = 320m, DefaultSalePrice = 320m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMaterialsCat.Id, Name = "Cooking Oil (per litre)", Kind = ProductKind.RawMaterial, SKU = "CKD-RM-OIL", DefaultCostPrice = 120m, DefaultSalePrice = 120m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft }
        );
    }
}
