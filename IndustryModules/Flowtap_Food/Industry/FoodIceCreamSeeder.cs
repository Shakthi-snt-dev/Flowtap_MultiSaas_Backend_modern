using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

namespace Flowtap_Food.Industry;

public static class FoodIceCreamSeeder
{
    public static Task SeedAsync(IApplicationDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedCategories(context, companyId);
        return Task.CompletedTask;
    }

    private static void SeedCategories(IApplicationDbContext context, Guid companyId)
    {
        // Ice Cream
        var iceCream = new ProductCategory { CompanyId = companyId, Name = "Ice Cream", SortOrder = 1, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(iceCream);

        var scoopsCat   = new ProductCategory { CompanyId = companyId, Name = "Scoops",            ParentCategoryId = iceCream.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var sundaesCat  = new ProductCategory { CompanyId = companyId, Name = "Sundaes",            ParentCategoryId = iceCream.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var conesCat    = new ProductCategory { CompanyId = companyId, Name = "Cones & Cups",       ParentCategoryId = iceCream.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var milkshakesCat = new ProductCategory { CompanyId = companyId, Name = "Milkshakes",       ParentCategoryId = iceCream.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(scoopsCat, sundaesCat, conesCat, milkshakesCat);

        // Toppings & Add-ons
        var toppings = new ProductCategory { CompanyId = companyId, Name = "Toppings & Add-ons", SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.Add(toppings);

        // Combos
        var combosCat = new ProductCategory { CompanyId = companyId, Name = "Combo Deals", SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.Add(combosCat);

        // Sample Products
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = scoopsCat.Id,    Name = "Belgian Chocolate Scoop (Single)",    Kind = ProductKind.Accessory, SKU = "ICE-SC-BCH", DefaultCostPrice = 0.50m, DefaultSalePrice = 2.50m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = scoopsCat.Id,    Name = "Vanilla Bean Scoop (Single)",         Kind = ProductKind.Accessory, SKU = "ICE-SC-VNL", DefaultCostPrice = 0.40m, DefaultSalePrice = 2.20m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = sundaesCat.Id,   Name = "Hot Fudge Brownie Sundae",            Kind = ProductKind.Accessory, SKU = "ICE-SN-HFB", DefaultCostPrice = 1.50m, DefaultSalePrice = 7.50m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = milkshakesCat.Id, Name = "Strawberry Milkshake (Large)",          Kind = ProductKind.Accessory, SKU = "ICE-MK-STR", DefaultCostPrice = 1.20m, DefaultSalePrice = 5.99m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );
    }
}
