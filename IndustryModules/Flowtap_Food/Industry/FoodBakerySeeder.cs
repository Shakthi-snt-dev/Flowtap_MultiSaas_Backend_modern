using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

namespace Flowtap_Food.Industry;

public static class FoodBakerySeeder
{
    public static async Task SeedAsync(IApplicationDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedCategories(context, companyId);
        await context.SaveChangesAsync(ct);
    }

    private static void SeedCategories(IApplicationDbContext context, Guid companyId)
    {
        // ── Baked Goods ──────────────────────────────────────────────────────────
        var bakedGoods = new ProductCategory { CompanyId = companyId, Name = "Baked Goods", SortOrder = 1, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(bakedGoods);

        var breadsCat    = new ProductCategory { CompanyId = companyId, Name = "Breads & Loaves",       ParentCategoryId = bakedGoods.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var cakesCat     = new ProductCategory { CompanyId = companyId, Name = "Cakes & Pastries",      ParentCategoryId = bakedGoods.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var cookiesCat   = new ProductCategory { CompanyId = companyId, Name = "Cookies & Biscuits",    ParentCategoryId = bakedGoods.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var muffinsCat   = new ProductCategory { CompanyId = companyId, Name = "Muffins & Cupcakes",    ParentCategoryId = bakedGoods.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var croissantCat = new ProductCategory { CompanyId = companyId, Name = "Croissants & Danishes", ParentCategoryId = bakedGoods.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(breadsCat, cakesCat, cookiesCat, muffinsCat, croissantCat);

        // ── Beverages ────────────────────────────────────────────────────────────
        var beverages = new ProductCategory { CompanyId = companyId, Name = "Beverages", SortOrder = 2, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(beverages);
        var hotBevCat  = new ProductCategory { CompanyId = companyId, Name = "Hot Beverages",  ParentCategoryId = beverages.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var coldBevCat = new ProductCategory { CompanyId = companyId, Name = "Cold Beverages", ParentCategoryId = beverages.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(hotBevCat, coldBevCat);

        // ── Raw Materials ────────────────────────────────────────────────────────
        var rawMaterials = new ProductCategory { CompanyId = companyId, Name = "Raw Materials", SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.Add(rawMaterials);

        // ── Final Products ───────────────────────────────────────────────────────
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = breadsCat.Id,    Name = "Classic White Bread Loaf",      Kind = ProductKind.FinalProduct, SKU = "BKY-BR-WHL", DefaultCostPrice = 25m,  DefaultSalePrice = 65m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = breadsCat.Id,    Name = "Whole Wheat Bread Loaf",        Kind = ProductKind.FinalProduct, SKU = "BKY-BR-WWT", DefaultCostPrice = 28m,  DefaultSalePrice = 75m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = cakesCat.Id,     Name = "Chocolate Truffle Cake (500g)", Kind = ProductKind.FinalProduct, SKU = "BKY-CK-CHT", DefaultCostPrice = 150m, DefaultSalePrice = 380m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = cakesCat.Id,     Name = "Vanilla Sponge Cake (500g)",    Kind = ProductKind.FinalProduct, SKU = "BKY-CK-VNS", DefaultCostPrice = 120m, DefaultSalePrice = 299m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = cookiesCat.Id,   Name = "Choco Chip Cookies (6 pcs)",    Kind = ProductKind.FinalProduct, SKU = "BKY-CK-CCK", DefaultCostPrice = 30m,  DefaultSalePrice = 79m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = muffinsCat.Id,   Name = "Blueberry Muffin",              Kind = ProductKind.FinalProduct, SKU = "BKY-MF-BBY", DefaultCostPrice = 20m,  DefaultSalePrice = 55m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = muffinsCat.Id,   Name = "Chocolate Cupcake",             Kind = ProductKind.FinalProduct, SKU = "BKY-MF-CCK", DefaultCostPrice = 18m,  DefaultSalePrice = 49m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = croissantCat.Id, Name = "Butter Croissant",              Kind = ProductKind.FinalProduct, SKU = "BKY-CR-BTR", DefaultCostPrice = 15m,  DefaultSalePrice = 45m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = croissantCat.Id, Name = "Almond Danish Pastry",          Kind = ProductKind.FinalProduct, SKU = "BKY-CR-ALM", DefaultCostPrice = 20m,  DefaultSalePrice = 59m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = hotBevCat.Id,    Name = "Freshly Brewed Coffee",         Kind = ProductKind.FinalProduct, SKU = "BKY-BV-COF", DefaultCostPrice = 15m,  DefaultSalePrice = 59m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = coldBevCat.Id,   Name = "Cold Brew Coffee",              Kind = ProductKind.FinalProduct, SKU = "BKY-BV-CBR", DefaultCostPrice = 20m,  DefaultSalePrice = 79m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );

        // ── Raw Materials ────────────────────────────────────────────────────────
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = rawMaterials.Id, Name = "All Purpose Flour (per kg)",  Kind = ProductKind.RawMaterial, SKU = "BKY-RM-FLR", DefaultCostPrice = 25m,  DefaultSalePrice = 25m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMaterials.Id, Name = "Granulated Sugar (per kg)",  Kind = ProductKind.RawMaterial, SKU = "BKY-RM-SGR", DefaultCostPrice = 40m,  DefaultSalePrice = 40m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMaterials.Id, Name = "Unsalted Butter (per kg)",   Kind = ProductKind.RawMaterial, SKU = "BKY-RM-BTR", DefaultCostPrice = 280m, DefaultSalePrice = 280m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMaterials.Id, Name = "Whole Eggs (per dozen)",     Kind = ProductKind.RawMaterial, SKU = "BKY-RM-EGG", DefaultCostPrice = 72m,  DefaultSalePrice = 72m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMaterials.Id, Name = "Cocoa Powder (per kg)",      Kind = ProductKind.RawMaterial, SKU = "BKY-RM-COC", DefaultCostPrice = 180m, DefaultSalePrice = 180m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMaterials.Id, Name = "Baking Soda (per kg)",       Kind = ProductKind.RawMaterial, SKU = "BKY-RM-BKS", DefaultCostPrice = 30m,  DefaultSalePrice = 30m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft }
        );
    }
}
