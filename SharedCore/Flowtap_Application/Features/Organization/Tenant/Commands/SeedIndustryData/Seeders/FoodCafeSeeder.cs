using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

namespace Flowtap_Application.Features.Organization.Tenant.Commands.SeedIndustryData.Seeders;

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
        var coffeesCat  = new ProductCategory { CompanyId = companyId, Name = "Coffees",           ParentCategoryId = hotDrinks.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var teasCat     = new ProductCategory { CompanyId = companyId, Name = "Teas & Infusions",  ParentCategoryId = hotDrinks.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var hotChocCat  = new ProductCategory { CompanyId = companyId, Name = "Hot Chocolate",     ParentCategoryId = hotDrinks.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(coffeesCat, teasCat, hotChocCat);

        // Cold Drinks
        var coldDrinks = new ProductCategory { CompanyId = companyId, Name = "Cold Drinks", SortOrder = 2, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(coldDrinks);
        var icedCoffeesCat = new ProductCategory { CompanyId = companyId, Name = "Iced Coffees",  ParentCategoryId = coldDrinks.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var smoothiesCat   = new ProductCategory { CompanyId = companyId, Name = "Smoothies",     ParentCategoryId = coldDrinks.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var coldBrewCat    = new ProductCategory { CompanyId = companyId, Name = "Cold Brew",     ParentCategoryId = coldDrinks.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(icedCoffeesCat, smoothiesCat, coldBrewCat);

        // Light Meals
        var lightMeals = new ProductCategory { CompanyId = companyId, Name = "Light Meals", SortOrder = 3, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(lightMeals);
        var sandwichesCat = new ProductCategory { CompanyId = companyId, Name = "Sandwiches & Wraps", ParentCategoryId = lightMeals.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var saladsCat     = new ProductCategory { CompanyId = companyId, Name = "Salads",             ParentCategoryId = lightMeals.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var snacksCat     = new ProductCategory { CompanyId = companyId, Name = "Snacks & Pastries",  ParentCategoryId = lightMeals.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(sandwichesCat, saladsCat, snacksCat);

        // Sample products
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = coffeesCat.Id,    Name = "Espresso",                      Kind = ProductKind.Accessory, SKU = "CAF-HT-ESP", DefaultCostPrice = 0.20m, DefaultSalePrice = 2.50m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = coffeesCat.Id,    Name = "Cappuccino",                    Kind = ProductKind.Accessory, SKU = "CAF-HT-CAP", DefaultCostPrice = 0.40m, DefaultSalePrice = 3.50m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = coffeesCat.Id,    Name = "Latte",                         Kind = ProductKind.Accessory, SKU = "CAF-HT-LAT", DefaultCostPrice = 0.45m, DefaultSalePrice = 3.80m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = icedCoffeesCat.Id, Name = "Iced Caramel Macchiato",         Kind = ProductKind.Accessory, SKU = "CAF-CL-ICM", DefaultCostPrice = 0.60m, DefaultSalePrice = 4.50m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = sandwichesCat.Id, Name = "Chicken Club Sandwich",         Kind = ProductKind.Accessory, SKU = "CAF-LM-CCS", DefaultCostPrice = 1.50m, DefaultSalePrice = 6.50m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = snacksCat.Id,     Name = "Butter Croissant",              Kind = ProductKind.Accessory, SKU = "CAF-LM-BCR", DefaultCostPrice = 0.50m, DefaultSalePrice = 2.00m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );
    }
}
