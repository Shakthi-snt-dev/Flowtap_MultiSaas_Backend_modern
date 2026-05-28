using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

namespace Flowtap_Application.Features.Organization.Tenant.Commands.SeedIndustryData.Seeders;

public static class HotelSeeder
{
    public static Task SeedAsync(IApplicationDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedProductCategories(context, companyId);
        return Task.CompletedTask;
    }

    private static void SeedProductCategories(IApplicationDbContext context, Guid companyId)
    {
        // --- Room Service ---
        var roomService = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Room Service",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(roomService);

        var foodSnacksCat = new ProductCategory { CompanyId = companyId, Name = "Food & Snacks", ParentCategoryId = roomService.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var roomBeveragesCat = new ProductCategory { CompanyId = companyId, Name = "Beverages",     ParentCategoryId = roomService.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(foodSnacksCat, roomBeveragesCat);

        // --- Minibar Items ---
        var minibarCat = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Minibar Items",
            SortOrder = 2,
            IsDirectProductExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(minibarCat);

        // --- Laundry & Pressing ---
        var laundryCat = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Laundry & Pressing",
            SortOrder = 3,
            IsDirectProductExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(laundryCat);

        // --- Spa & Wellness ---
        var spaCat = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Spa & Wellness",
            SortOrder = 4,
            IsDirectProductExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(spaCat);

        // --- Seed Hotel Products ---
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId,
                CategoryId = foodSnacksCat.Id,
                Name = "Club Sandwich with Fries",
                Kind = ProductKind.Accessory,
                SKU = "HTL-RS-CSW",
                DefaultCostPrice = 3.00m,
                DefaultSalePrice = 14.99m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = minibarCat.Id,
                Name = "Roasted Salted Cashews 50g",
                Kind = ProductKind.Accessory,
                SKU = "HTL-MB-CAS",
                DefaultCostPrice = 1.00m,
                DefaultSalePrice = 6.00m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = spaCat.Id,
                Name = "Lavender Essential Massage Oil 100ml",
                Kind = ProductKind.Accessory,
                SKU = "HTL-SP-OIL",
                DefaultCostPrice = 4.50m,
                DefaultSalePrice = 29.99m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );
    }
}
