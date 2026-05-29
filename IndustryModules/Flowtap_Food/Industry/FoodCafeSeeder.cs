using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

namespace Flowtap_Food.Industry;

public static class FoodCafeSeeder
{
    public static async Task SeedAsync(IApplicationDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedCategories(context, companyId);
        await context.SaveChangesAsync(ct);
    }

    private static void SeedCategories(IApplicationDbContext context, Guid companyId)
    {
        // ── Hot Drinks ───────────────────────────────────────────────────────────
        var hotDrinks = new ProductCategory { CompanyId = companyId, Name = "Hot Drinks", SortOrder = 1, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(hotDrinks);

        var coffeeCat  = new ProductCategory { CompanyId = companyId, Name = "Coffee",       ParentCategoryId = hotDrinks.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var teaCat     = new ProductCategory { CompanyId = companyId, Name = "Tea",           ParentCategoryId = hotDrinks.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var hotChocCat = new ProductCategory { CompanyId = companyId, Name = "Hot Chocolate", ParentCategoryId = hotDrinks.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(coffeeCat, teaCat, hotChocCat);

        // ── Cold Drinks ──────────────────────────────────────────────────────────
        var coldDrinks = new ProductCategory { CompanyId = companyId, Name = "Cold Drinks", SortOrder = 2, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(coldDrinks);

        var icedCoffeeCat = new ProductCategory { CompanyId = companyId, Name = "Iced Coffee",     ParentCategoryId = coldDrinks.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var smoothiesCat  = new ProductCategory { CompanyId = companyId, Name = "Smoothies",       ParentCategoryId = coldDrinks.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var frappesCat    = new ProductCategory { CompanyId = companyId, Name = "Frappes & Shakes", ParentCategoryId = coldDrinks.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(icedCoffeeCat, smoothiesCat, frappesCat);

        // ── Food ─────────────────────────────────────────────────────────────────
        var food = new ProductCategory { CompanyId = companyId, Name = "Food", SortOrder = 3, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(food);

        var sandwichesCat = new ProductCategory { CompanyId = companyId, Name = "Sandwiches & Wraps", ParentCategoryId = food.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var pastriesCat   = new ProductCategory { CompanyId = companyId, Name = "Pastries & Muffins", ParentCategoryId = food.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var saladsCat     = new ProductCategory { CompanyId = companyId, Name = "Salads & Bowls",     ParentCategoryId = food.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(sandwichesCat, pastriesCat, saladsCat);

        // ── Raw Materials ────────────────────────────────────────────────────────
        var rawMat = new ProductCategory { CompanyId = companyId, Name = "Raw Materials", SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.Add(rawMat);

        // ── Final Products ───────────────────────────────────────────────────────
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = coffeeCat.Id,      Name = "Espresso (Single Shot)",   Kind = ProductKind.FinalProduct, SKU = "CAF-HT-ESP", DefaultCostPrice = 15m, DefaultSalePrice = 79m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = coffeeCat.Id,      Name = "Cappuccino (Regular)",     Kind = ProductKind.FinalProduct, SKU = "CAF-HT-CAP", DefaultCostPrice = 20m, DefaultSalePrice = 99m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = coffeeCat.Id,      Name = "Flat White",               Kind = ProductKind.FinalProduct, SKU = "CAF-HT-FLW", DefaultCostPrice = 22m, DefaultSalePrice = 109m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = teaCat.Id,         Name = "Masala Chai",              Kind = ProductKind.FinalProduct, SKU = "CAF-HT-MCH", DefaultCostPrice = 10m, DefaultSalePrice = 59m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = teaCat.Id,         Name = "Green Tea",                Kind = ProductKind.FinalProduct, SKU = "CAF-HT-GNT", DefaultCostPrice = 8m,  DefaultSalePrice = 49m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = hotChocCat.Id,     Name = "Classic Hot Chocolate",    Kind = ProductKind.FinalProduct, SKU = "CAF-HT-CHC", DefaultCostPrice = 18m, DefaultSalePrice = 89m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = icedCoffeeCat.Id,  Name = "Iced Caramel Latte",       Kind = ProductKind.FinalProduct, SKU = "CAF-IC-ICL", DefaultCostPrice = 25m, DefaultSalePrice = 129m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = icedCoffeeCat.Id,  Name = "Cold Brew Coffee",         Kind = ProductKind.FinalProduct, SKU = "CAF-IC-CBR", DefaultCostPrice = 20m, DefaultSalePrice = 109m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = smoothiesCat.Id,   Name = "Mixed Berry Smoothie",     Kind = ProductKind.FinalProduct, SKU = "CAF-SM-MBR", DefaultCostPrice = 30m, DefaultSalePrice = 139m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = frappesCat.Id,     Name = "Caramel Frappe",           Kind = ProductKind.FinalProduct, SKU = "CAF-FR-CAR", DefaultCostPrice = 28m, DefaultSalePrice = 149m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = sandwichesCat.Id,  Name = "Grilled Chicken Panini",   Kind = ProductKind.FinalProduct, SKU = "CAF-FD-GCP", DefaultCostPrice = 60m, DefaultSalePrice = 179m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = sandwichesCat.Id,  Name = "Veggie Club Sandwich",     Kind = ProductKind.FinalProduct, SKU = "CAF-FD-VCS", DefaultCostPrice = 45m, DefaultSalePrice = 139m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = pastriesCat.Id,    Name = "Almond Croissant",         Kind = ProductKind.FinalProduct, SKU = "CAF-FD-ACR", DefaultCostPrice = 25m, DefaultSalePrice = 79m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = saladsCat.Id,      Name = "Caesar Salad Bowl",        Kind = ProductKind.FinalProduct, SKU = "CAF-FD-CSB", DefaultCostPrice = 55m, DefaultSalePrice = 169m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );

        // ── Raw Materials ────────────────────────────────────────────────────────
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = rawMat.Id, Name = "Coffee Beans (per kg)",     Kind = ProductKind.RawMaterial, SKU = "CAF-RM-CFB", DefaultCostPrice = 600m, DefaultSalePrice = 600m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMat.Id, Name = "Full Fat Milk (per litre)", Kind = ProductKind.RawMaterial, SKU = "CAF-RM-MLK", DefaultCostPrice = 24m,  DefaultSalePrice = 24m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMat.Id, Name = "Tea Leaves (per kg)",       Kind = ProductKind.RawMaterial, SKU = "CAF-RM-TEA", DefaultCostPrice = 200m, DefaultSalePrice = 200m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMat.Id, Name = "Sugar (per kg)",            Kind = ProductKind.RawMaterial, SKU = "CAF-RM-SGR", DefaultCostPrice = 40m,  DefaultSalePrice = 40m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft }
        );
    }
}
