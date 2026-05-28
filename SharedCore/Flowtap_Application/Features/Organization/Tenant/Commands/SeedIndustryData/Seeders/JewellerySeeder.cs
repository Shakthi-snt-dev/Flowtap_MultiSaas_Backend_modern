using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

namespace Flowtap_Application.Features.Organization.Tenant.Commands.SeedIndustryData.Seeders;

public static class JewellerySeeder
{
    public static Task SeedAsync(IApplicationDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedProductCategories(context, companyId);
        return Task.CompletedTask;
    }

    private static void SeedProductCategories(IApplicationDbContext context, Guid companyId)
    {
        // --- Gold Jewellery ---
        var gold = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Gold Jewellery",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(gold);

        var goldRings = new ProductCategory { CompanyId = companyId, Name = "Gold Rings", ParentCategoryId = gold.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var goldNecklaces = new ProductCategory { CompanyId = companyId, Name = "Gold Necklaces & Chains", ParentCategoryId = gold.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(goldRings, goldNecklaces);

        // --- Silver Jewellery ---
        var silver = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Silver Jewellery",
            SortOrder = 2,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(silver);

        var silverRings = new ProductCategory { CompanyId = companyId, Name = "Silver Rings", ParentCategoryId = silver.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var silverNecklaces = new ProductCategory { CompanyId = companyId, Name = "Silver Necklaces & Chains", ParentCategoryId = silver.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(silverRings, silverNecklaces);

        // --- Diamond Jewellery ---
        var diamond = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Diamond Jewellery",
            SortOrder = 3,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(diamond);

        var diamondRings = new ProductCategory { CompanyId = companyId, Name = "Diamond Rings",     ParentCategoryId = diamond.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var diamondNecklaces = new ProductCategory { CompanyId = companyId, Name = "Diamond Necklaces", ParentCategoryId = diamond.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var diamondEarrings = new ProductCategory { CompanyId = companyId, Name = "Diamond Earrings",  ParentCategoryId = diamond.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(diamondRings, diamondNecklaces, diamondEarrings);

        // --- Gemstone Jewellery ---
        var gemstone = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Gemstone Jewellery",
            SortOrder = 4,
            IsDirectProductExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(gemstone);

        // --- Antique & Kundan ---
        var antique = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Antique & Kundan",
            SortOrder = 5,
            IsDirectProductExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(antique);

        // --- Seed Jewellery Products ---
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId,
                CategoryId = diamondRings.Id,
                Name = "Solitaire Diamond Engagement Ring (1 Carat)",
                Kind = ProductKind.Accessory,
                SKU = "JWL-DM-RNG-01",
                DefaultCostPrice = 1200.00m,
                DefaultSalePrice = 2999.00m,
                IsSerialized = true,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = goldNecklaces.Id,
                Name = "22K Yellow Gold Rope Chain (24 inch)",
                Kind = ProductKind.Accessory,
                SKU = "JWL-GD-CHN-22K",
                DefaultCostPrice = 450.00m,
                DefaultSalePrice = 850.00m,
                IsSerialized = true,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = silverRings.Id,
                Name = "Sterling Silver Infinity Ring",
                Kind = ProductKind.Accessory,
                SKU = "JWL-SV-RNG-INF",
                DefaultCostPrice = 12.00m,
                DefaultSalePrice = 49.00m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );
    }
}
