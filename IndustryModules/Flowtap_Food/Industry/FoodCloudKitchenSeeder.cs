using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

namespace Flowtap_Food.Industry;

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
        var deliveryMenu = new ProductCategory { CompanyId = companyId, Name = "Delivery Menu", SortOrder = 1, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(deliveryMenu);

        var mealsCat    = new ProductCategory { CompanyId = companyId, Name = "Meals & Combos",    ParentCategoryId = deliveryMenu.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var snacksCat   = new ProductCategory { CompanyId = companyId, Name = "Snacks & Starters", ParentCategoryId = deliveryMenu.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var dessertsCat = new ProductCategory { CompanyId = companyId, Name = "Desserts",          ParentCategoryId = deliveryMenu.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var drinksCat   = new ProductCategory { CompanyId = companyId, Name = "Drinks",            ParentCategoryId = deliveryMenu.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(mealsCat, snacksCat, dessertsCat, drinksCat);

        // Packaging
        var packagingCat = new ProductCategory { CompanyId = companyId, Name = "Packaging Materials", SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.Add(packagingCat);

        // Raw Materials
        var rawMaterialsCat = new ProductCategory { CompanyId = companyId, Name = "Raw Materials", SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.Add(rawMaterialsCat);

        // Sample Products
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = mealsCat.Id,      Name = "Grilled Chicken Rice Box",      Kind = ProductKind.Accessory, SKU = "CKD-ML-GCR", DefaultCostPrice = 3.50m, DefaultSalePrice = 12.99m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = mealsCat.Id,      Name = "Veg Biryani Box",               Kind = ProductKind.Accessory, SKU = "CKD-ML-VBR", DefaultCostPrice = 2.00m, DefaultSalePrice = 9.99m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = snacksCat.Id,     Name = "Crispy Chicken Wings (6 pcs)",  Kind = ProductKind.Accessory, SKU = "CKD-SN-CCW", DefaultCostPrice = 2.50m, DefaultSalePrice = 8.99m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = packagingCat.Id,  Name = "Disposable Food Box (Pack 50)", Kind = ProductKind.RawMaterial, SKU = "CKD-PK-FBX", DefaultCostPrice = 3.00m, DefaultSalePrice = 3.00m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Draft }
        );
    }
}
