using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

namespace Flowtap_Application.Features.Organization.Tenant.Commands.SeedIndustryData.Seeders;

public static class FoodCloudKitchenSeeder
{
    public static Task SeedAsync(IApplicationDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedCategories(context, companyId);
        return Task.CompletedTask;
    }

    private static void SeedCategories(IApplicationDbContext context, Guid companyId)
    {
        // Delivery Menu
        var menu = new ProductCategory { CompanyId = companyId, Name = "Delivery Menu", SortOrder = 1, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(menu);

        var mainsCat     = new ProductCategory { CompanyId = companyId, Name = "Mains",           ParentCategoryId = menu.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var sidesCat     = new ProductCategory { CompanyId = companyId, Name = "Sides & Extras",  ParentCategoryId = menu.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var combosCat    = new ProductCategory { CompanyId = companyId, Name = "Combo Meals",     ParentCategoryId = menu.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var beveragesCat = new ProductCategory { CompanyId = companyId, Name = "Beverages",       ParentCategoryId = menu.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var dessertsCat  = new ProductCategory { CompanyId = companyId, Name = "Desserts",        ParentCategoryId = menu.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(mainsCat, sidesCat, combosCat, beveragesCat, dessertsCat);

        // Packaging
        var packagingCat = new ProductCategory { CompanyId = companyId, Name = "Packaging Materials", SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.Add(packagingCat);

        // Raw Materials
        var rawMaterialsCat = new ProductCategory { CompanyId = companyId, Name = "Raw Materials", SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.Add(rawMaterialsCat);

        // Sample products
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = mainsCat.Id,     Name = "Grilled Chicken Bowl",           Kind = ProductKind.Accessory,   SKU = "CLD-MN-GCB", DefaultCostPrice = 2.50m, DefaultSalePrice = 9.99m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = mainsCat.Id,     Name = "Veggie Biryani",                 Kind = ProductKind.Accessory,   SKU = "CLD-MN-VBR", DefaultCostPrice = 1.80m, DefaultSalePrice = 7.99m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = combosCat.Id,    Name = "Meal Deal — Main + Side + Drink", Kind = ProductKind.Accessory,    SKU = "CLD-CB-MSD", DefaultCostPrice = 3.50m, DefaultSalePrice = 12.99m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = packagingCat.Id, Name = "Takeaway Box (1L)",              Kind = ProductKind.Accessory,   SKU = "CLD-PK-BX1", DefaultCostPrice = 0.15m, DefaultSalePrice = 0.15m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = rawMaterialsCat.Id, Name = "Basmati Rice (per kg)",         Kind = ProductKind.RawMaterial, SKU = "CLD-RM-BSM", DefaultCostPrice = 1.20m, DefaultSalePrice = 1.20m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft },
            new Product { CompanyId = companyId, CategoryId = beveragesCat.Id, Name = "Bottled Water (500ml)",         Kind = ProductKind.Accessory,   SKU = "CLD-BV-WTR", DefaultCostPrice = 0.15m, DefaultSalePrice = 0.99m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );
    }
}
