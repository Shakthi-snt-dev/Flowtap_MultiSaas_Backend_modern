using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

namespace Flowtap_Food.Industry;

public static class FoodRestaurantSeeder
{
    public static Task SeedAsync(IApplicationDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedProductCategories(context, companyId);
        return Task.CompletedTask;
    }

    private static void SeedProductCategories(IApplicationDbContext context, Guid companyId)
    {
        // --- Food ---
        var food = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Food",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(food);

        var startersCat = new ProductCategory { CompanyId = companyId, Name = "Starters & Appetizers", ParentCategoryId = food.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var soupsCat = new ProductCategory { CompanyId = companyId, Name = "Soups & Salads",        ParentCategoryId = food.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };

        // Main Course (sub-category with its own children)
        var mainCourse = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Main Course",
            ParentCategoryId = food.Id,
            SortOrder = 3,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(mainCourse);

        var vegDishesCat = new ProductCategory { CompanyId = companyId, Name = "Veg Dishes",     ParentCategoryId = mainCourse.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var nonVegDishesCat = new ProductCategory { CompanyId = companyId, Name = "Non-Veg Dishes", ParentCategoryId = mainCourse.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var seafoodCat = new ProductCategory { CompanyId = companyId, Name = "Seafood",        ParentCategoryId = mainCourse.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(vegDishesCat, nonVegDishesCat, seafoodCat);

        var noodlesCat = new ProductCategory { CompanyId = companyId, Name = "Rice, Bread & Noodles", ParentCategoryId = food.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var bbqCat = new ProductCategory { CompanyId = companyId, Name = "Grills & BBQ",          ParentCategoryId = food.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        var dessertsCat = new ProductCategory { CompanyId = companyId, Name = "Desserts & Sweets",     ParentCategoryId = food.Id, SortOrder = 6, IsDirectProductExist = true, IsActive = true };
        var snacksCat = new ProductCategory { CompanyId = companyId, Name = "Snacks & Fast Food",    ParentCategoryId = food.Id, SortOrder = 7, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(startersCat, soupsCat, noodlesCat, bbqCat, dessertsCat, snacksCat);

        // --- Beverages ---
        var beverages = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Beverages",
            SortOrder = 2,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(beverages);

        var hotBevCat = new ProductCategory { CompanyId = companyId, Name = "Hot Beverages",    ParentCategoryId = beverages.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var coldBevCat = new ProductCategory { CompanyId = companyId, Name = "Cold Beverages",   ParentCategoryId = beverages.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var juicesCat = new ProductCategory { CompanyId = companyId, Name = "Juices & Shakes",  ParentCategoryId = beverages.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var mocktailsCat = new ProductCategory { CompanyId = companyId, Name = "Mocktails",        ParentCategoryId = beverages.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(hotBevCat, coldBevCat, juicesCat, mocktailsCat);

        // --- Add-ons & Extras ---
        var addonsCat = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Add-ons & Extras",
            SortOrder = 3,
            IsDirectProductExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(addonsCat);

        // --- Seed Restaurant Products ---
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId,
                CategoryId = startersCat.Id,
                Name = "Crispy Garlic Bread with Cheese",
                Kind = ProductKind.Accessory,
                SKU = "RST-ST-GBR",
                DefaultCostPrice = 1.20m,
                DefaultSalePrice = 5.99m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = nonVegDishesCat.Id,
                Name = "Classic Butter Chicken (Murgh Makhani)",
                Kind = ProductKind.Accessory,
                SKU = "RST-MC-BCH",
                DefaultCostPrice = 4.50m,
                DefaultSalePrice = 16.99m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = juicesCat.Id,
                Name = "Fresh Squeezed Orange Juice",
                Kind = ProductKind.Accessory,
                SKU = "RST-BV-ORJ",
                DefaultCostPrice = 0.80m,
                DefaultSalePrice = 4.50m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );
    }
}
