using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

namespace Flowtap_Food.Industry;

public static class FoodBakerySeeder
{
    public static Task SeedAsync(IApplicationDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedCategories(context, companyId);
        return Task.CompletedTask;
    }

    private static void SeedCategories(IApplicationDbContext context, Guid companyId)
    {
        // Baked Goods
        var bakedGoods = new ProductCategory { CompanyId = companyId, Name = "Baked Goods", SortOrder = 1, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(bakedGoods);

        var breadsCat    = new ProductCategory { CompanyId = companyId, Name = "Breads & Loaves",       ParentCategoryId = bakedGoods.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var cakesCat     = new ProductCategory { CompanyId = companyId, Name = "Cakes & Pastries",      ParentCategoryId = bakedGoods.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var cookiesCat   = new ProductCategory { CompanyId = companyId, Name = "Cookies & Biscuits",    ParentCategoryId = bakedGoods.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var muffinsCat   = new ProductCategory { CompanyId = companyId, Name = "Muffins & Cupcakes",    ParentCategoryId = bakedGoods.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var croissantCat = new ProductCategory { CompanyId = companyId, Name = "Croissants & Danishes", ParentCategoryId = bakedGoods.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(breadsCat, cakesCat, cookiesCat, muffinsCat, croissantCat);

        // Beverages
        var beverages = new ProductCategory { CompanyId = companyId, Name = "Beverages", SortOrder = 2, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(beverages);
        var hotBevCat  = new ProductCategory { CompanyId = companyId, Name = "Hot Beverages",  ParentCategoryId = beverages.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var coldBevCat = new ProductCategory { CompanyId = companyId, Name = "Cold Beverages", ParentCategoryId = beverages.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(hotBevCat, coldBevCat);

        // Raw Materials
        var rawMaterials = new ProductCategory { CompanyId = companyId, Name = "Raw Materials", SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.Add(rawMaterials);

        // Sample Products
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = breadsCat.Id,    Name = "Classic White Bread Loaf",         Kind = ProductKind.Accessory, SKU = "BKY-BR-WHL", DefaultCostPrice = 0.80m, DefaultSalePrice = 3.50m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = cakesCat.Id,     Name = "Chocolate Truffle Cake (500g)",    Kind = ProductKind.Accessory, SKU = "BKY-CK-CHT", DefaultCostPrice = 4.00m, DefaultSalePrice = 18.00m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = muffinsCat.Id,   Name = "Blueberry Muffin",                 Kind = ProductKind.Accessory, SKU = "BKY-MF-BBY", DefaultCostPrice = 0.60m, DefaultSalePrice = 2.50m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = croissantCat.Id, Name = "Butter Croissant",                 Kind = ProductKind.Accessory, SKU = "BKY-CR-BTR", DefaultCostPrice = 0.50m, DefaultSalePrice = 2.00m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = rawMaterials.Id, Name = "All Purpose Flour (per kg)",       Kind = ProductKind.RawMaterial, SKU = "BKY-RM-FLR", DefaultCostPrice = 0.50m, DefaultSalePrice = 0.50m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMaterials.Id, Name = "Granulated Sugar (per kg)",        Kind = ProductKind.RawMaterial, SKU = "BKY-RM-SGR", DefaultCostPrice = 0.60m, DefaultSalePrice = 0.60m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMaterials.Id, Name = "Unsalted Butter (per kg)",         Kind = ProductKind.RawMaterial, SKU = "BKY-RM-BTR", DefaultCostPrice = 5.00m, DefaultSalePrice = 5.00m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = hotBevCat.Id,    Name = "Freshly Brewed Coffee",            Kind = ProductKind.Accessory, SKU = "BKY-BV-COF", DefaultCostPrice = 0.30m, DefaultSalePrice = 2.50m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );
    }
}
