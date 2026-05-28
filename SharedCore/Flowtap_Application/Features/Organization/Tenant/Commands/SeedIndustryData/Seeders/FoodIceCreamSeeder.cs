using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

namespace Flowtap_Application.Features.Organization.Tenant.Commands.SeedIndustryData.Seeders;

public static class FoodIceCreamSeeder
{
    public static Task SeedAsync(IApplicationDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedCategories(context, companyId);
        return Task.CompletedTask;
    }

    private static void SeedCategories(IApplicationDbContext context, Guid companyId)
    {
        // Ice Creams
        var iceCream = new ProductCategory { CompanyId = companyId, Name = "Ice Creams", SortOrder = 1, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(iceCream);
        var singleScoopCat = new ProductCategory { CompanyId = companyId, Name = "Single Scoop",   ParentCategoryId = iceCream.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var doubleScoopCat = new ProductCategory { CompanyId = companyId, Name = "Double Scoop",   ParentCategoryId = iceCream.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var sundaesCat     = new ProductCategory { CompanyId = companyId, Name = "Sundaes",        ParentCategoryId = iceCream.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(singleScoopCat, doubleScoopCat, sundaesCat);

        // Shakes & Floats
        var shakes = new ProductCategory { CompanyId = companyId, Name = "Shakes & Floats", SortOrder = 2, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(shakes);
        var milkshakesCat = new ProductCategory { CompanyId = companyId, Name = "Milkshakes", ParentCategoryId = shakes.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var floatsCat     = new ProductCategory { CompanyId = companyId, Name = "Ice Cream Floats", ParentCategoryId = shakes.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(milkshakesCat, floatsCat);

        // Toppings & Combos
        var toppingsCat = new ProductCategory { CompanyId = companyId, Name = "Toppings & Extras", SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.Add(toppingsCat);

        // Sample products
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = singleScoopCat.Id, Name = "Vanilla Single Scoop",         Kind = ProductKind.Accessory, SKU = "ICE-SC-VNL", DefaultCostPrice = 0.40m, DefaultSalePrice = 1.80m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = singleScoopCat.Id, Name = "Chocolate Single Scoop",       Kind = ProductKind.Accessory, SKU = "ICE-SC-CHC", DefaultCostPrice = 0.45m, DefaultSalePrice = 1.80m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = singleScoopCat.Id, Name = "Strawberry Single Scoop",      Kind = ProductKind.Accessory, SKU = "ICE-SC-STR", DefaultCostPrice = 0.45m, DefaultSalePrice = 1.80m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = sundaesCat.Id,     Name = "Hot Fudge Brownie Sundae",     Kind = ProductKind.Accessory, SKU = "ICE-SN-HFB", DefaultCostPrice = 1.20m, DefaultSalePrice = 6.50m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = milkshakesCat.Id,  Name = "Classic Chocolate Milkshake",  Kind = ProductKind.Accessory, SKU = "ICE-SH-CHM", DefaultCostPrice = 0.80m, DefaultSalePrice = 4.50m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = toppingsCat.Id,    Name = "Extra Scoop (Add-on)",         Kind = ProductKind.Accessory, SKU = "ICE-TP-EXT", DefaultCostPrice = 0.40m, DefaultSalePrice = 1.00m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );
    }
}
