using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

namespace Flowtap_Application.Features.Organization.Tenant.Commands.SeedIndustryData.Seeders;

public static class RetailSeeder
{
    public static Task SeedAsync(IApplicationDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedProductCategories(context, companyId);
        return Task.CompletedTask;
    }

    private static void SeedProductCategories(IApplicationDbContext context, Guid companyId)
    {
        // --- General Products ---
        var general = new ProductCategory
        {
            CompanyId = companyId,
            Name = "General Products",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(general);

        var electronicsCat = new ProductCategory { CompanyId = companyId, Name = "Electronics",            ParentCategoryId = general.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var clothingCat = new ProductCategory { CompanyId = companyId, Name = "Clothing & Apparel",     ParentCategoryId = general.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var groceriesCat = new ProductCategory { CompanyId = companyId, Name = "Food & Groceries",       ParentCategoryId = general.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var kitchenCat = new ProductCategory { CompanyId = companyId, Name = "Home & Kitchen",         ParentCategoryId = general.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var beautyCat = new ProductCategory { CompanyId = companyId, Name = "Beauty & Personal Care", ParentCategoryId = general.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        var sportsCat = new ProductCategory { CompanyId = companyId, Name = "Sports & Outdoors",      ParentCategoryId = general.Id, SortOrder = 6, IsDirectProductExist = true, IsActive = true };
        var toysCat = new ProductCategory { CompanyId = companyId, Name = "Toys & Games",          ParentCategoryId = general.Id, SortOrder = 7, IsDirectProductExist = true, IsActive = true };
        var booksCat = new ProductCategory { CompanyId = companyId, Name = "Books & Stationery",    ParentCategoryId = general.Id, SortOrder = 8, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(electronicsCat, clothingCat, groceriesCat, kitchenCat, beautyCat, sportsCat, toysCat, booksCat);

        // --- Seed Retail Products ---
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId,
                CategoryId = electronicsCat.Id,
                Name = "Wireless Bluetooth Earbuds",
                Kind = ProductKind.Accessory,
                SKU = "RET-EL-EAR",
                DefaultCostPrice = 15.00m,
                DefaultSalePrice = 49.99m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = clothingCat.Id,
                Name = "Classic Cotton Crewneck T-Shirt",
                Kind = ProductKind.Accessory,
                SKU = "RET-CL-TSH",
                DefaultCostPrice = 4.50m,
                DefaultSalePrice = 19.99m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = kitchenCat.Id,
                Name = "Stainless Steel Vacuum Insulated Water Bottle 750ml",
                Kind = ProductKind.Accessory,
                SKU = "RET-HK-BOT",
                DefaultCostPrice = 5.00m,
                DefaultSalePrice = 24.99m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );
    }
}
