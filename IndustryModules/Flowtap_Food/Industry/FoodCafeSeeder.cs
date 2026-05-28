using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

namespace Flowtap_Food.Industry;

public static class FoodCafeSeeder
{
    public static Task SeedAsync(IApplicationDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedCategories(context, companyId);
        return Task.CompletedTask;
    }

    private static void SeedCategories(IApplicationDbContext context, Guid companyId)
    {
        // Hot Drinks
        var hotDrinks = new ProductCategory { CompanyId = companyId, Name = "Hot Drinks", SortOrder = 1, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(hotDrinks);

        var coffeeCat  = new ProductCategory { CompanyId = companyId, Name = "Coffee",    ParentCategoryId = hotDrinks.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var teaCat     = new ProductCategory { CompanyId = companyId, Name = "Tea",       ParentCategoryId = hotDrinks.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var hotChocCat = new ProductCategory { CompanyId = companyId, Name = "Hot Chocolate", ParentCategoryId = hotDrinks.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(coffeeCat, teaCat, hotChocCat);

        // Cold Drinks
        var coldDrinks = new ProductCategory { CompanyId = companyId, Name = "Cold Drinks", SortOrder = 2, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(coldDrinks);

        var icedCoffeeCat  = new ProductCategory { CompanyId = companyId, Name = "Iced Coffee",     ParentCategoryId = coldDrinks.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var smoothiesCat   = new ProductCategory { CompanyId = companyId, Name = "Smoothies",       ParentCategoryId = coldDrinks.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var frappesCat     = new ProductCategory { CompanyId = companyId, Name = "Frappes & Shakes", ParentCategoryId = coldDrinks.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(icedCoffeeCat, smoothiesCat, frappesCat);

        // Food
        var food = new ProductCategory { CompanyId = companyId, Name = "Food", SortOrder = 3, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(food);

        var sandwichesCat = new ProductCategory { CompanyId = companyId, Name = "Sandwiches & Wraps", ParentCategoryId = food.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var pastriesCat   = new ProductCategory { CompanyId = companyId, Name = "Pastries & Muffins", ParentCategoryId = food.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var saladsCat     = new ProductCategory { CompanyId = companyId, Name = "Salads & Bowls",     ParentCategoryId = food.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(sandwichesCat, pastriesCat, saladsCat);

        // Sample Products
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = coffeeCat.Id,      Name = "Flat White (Regular)",          Kind = ProductKind.Accessory, SKU = "CAF-HT-FLW", DefaultCostPrice = 0.40m, DefaultSalePrice = 4.20m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = icedCoffeeCat.Id,  Name = "Iced Caramel Latte",            Kind = ProductKind.Accessory, SKU = "CAF-IC-ICL", DefaultCostPrice = 0.60m, DefaultSalePrice = 5.50m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = sandwichesCat.Id,  Name = "Grilled Chicken Panini",        Kind = ProductKind.Accessory, SKU = "CAF-FD-GCP", DefaultCostPrice = 1.80m, DefaultSalePrice = 7.99m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = pastriesCat.Id,    Name = "Almond Croissant",              Kind = ProductKind.Accessory, SKU = "CAF-FD-ACR", DefaultCostPrice = 0.90m, DefaultSalePrice = 3.50m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );
    }
}
