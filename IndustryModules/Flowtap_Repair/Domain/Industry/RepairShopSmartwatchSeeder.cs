using Flowtap_Repair.Domain.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Domain.Industry;

/// <summary>
/// Seeds data for smartwatch and wearable repair shops:
/// Apple Watch, Galaxy Watch, Garmin, Fitbit, etc.
/// </summary>
public static class RepairShopSmartwatchSeeder
{
    public static async Task SeedAsync(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedProductCategories(context, companyId);
        SeedServiceCategories(context, companyId);
        await SeedDeviceBrandsAndModelsAsync(context, ct);
    }

    private static void SeedProductCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // --- Smartwatch Parts (root) ---
        var parts = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Smartwatch Parts",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsBrandExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(parts);

        var screensCat    = new ProductCategory { CompanyId = companyId, Name = "Screens & OLED Panels",    ParentCategoryId = parts.Id, SortOrder = 1, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var batteriesCat  = new ProductCategory { CompanyId = companyId, Name = "Batteries",                ParentCategoryId = parts.Id, SortOrder = 2, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var crownsCat     = new ProductCategory { CompanyId = companyId, Name = "Crowns & Buttons",         ParentCategoryId = parts.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var housingCat    = new ProductCategory { CompanyId = companyId, Name = "Housing & Back Covers",    ParentCategoryId = parts.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var chargingCat   = new ProductCategory { CompanyId = companyId, Name = "Charging Docks & Cables",  ParentCategoryId = parts.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        var sensorsCat    = new ProductCategory { CompanyId = companyId, Name = "Sensors & Vibration Motors", ParentCategoryId = parts.Id, SortOrder = 6, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(screensCat, batteriesCat, crownsCat, housingCat, chargingCat, sensorsCat);

        // --- Watch Accessories ---
        var accessories = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Smartwatch Accessories",
            SortOrder = 2,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(accessories);

        var bandsCat       = new ProductCategory { CompanyId = companyId, Name = "Watch Bands & Straps",       ParentCategoryId = accessories.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var screenProtCat  = new ProductCategory { CompanyId = companyId, Name = "Screen Protectors",          ParentCategoryId = accessories.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var casesCat       = new ProductCategory { CompanyId = companyId, Name = "Cases & Bumpers",            ParentCategoryId = accessories.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(bandsCat, screenProtCat, casesCat);

        // --- Watch Services ---
        var services = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Smartwatch Services",
            SortOrder = 3,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(services);

        var screenRepCat  = new ProductCategory { CompanyId = companyId, Name = "Screen Replacement", ParentCategoryId = services.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var battRepCat    = new ProductCategory { CompanyId = companyId, Name = "Battery Replacement", ParentCategoryId = services.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var bandRepCat    = new ProductCategory { CompanyId = companyId, Name = "Band Replacement",    ParentCategoryId = services.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var waterRepCat   = new ProductCategory { CompanyId = companyId, Name = "Water Damage Repair", ParentCategoryId = services.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var softRepCat    = new ProductCategory { CompanyId = companyId, Name = "Software & Reset",    ParentCategoryId = services.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(screenRepCat, battRepCat, bandRepCat, waterRepCat, softRepCat);

        // --- Sample products ---
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId, CategoryId = screensCat.Id,
                Name = "Apple Watch Series 8 OLED Screen Assembly (41mm)",
                Kind = ProductKind.SparePart, SKU = "SW-SCR-AW8-41",
                DefaultCostPrice = 55.00m, DefaultSalePrice = 139.00m,
                IsSerialized = false, IsUniversal = false, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId, CategoryId = batteriesCat.Id,
                Name = "Apple Watch Series 7 Battery 309mAh",
                Kind = ProductKind.SparePart, SKU = "SW-BAT-AW7",
                DefaultCostPrice = 12.00m, DefaultSalePrice = 39.00m,
                IsSerialized = false, IsUniversal = false, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId, CategoryId = bandsCat.Id,
                Name = "Silicone Sport Band 44mm (Pack of 2)",
                Kind = ProductKind.Accessory, SKU = "SW-BAND-SIL-44",
                DefaultCostPrice = 2.50m, DefaultSalePrice = 14.99m,
                IsSerialized = false, IsUniversal = true, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );
    }

    private static void SeedServiceCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // --- Hardware Issues ---
        var hardware = new ServiceCategory { CompanyId = companyId, Name = "Hardware Issues", SortOrder = 1, IsSubCategoryExist = true, IsActive = true };
        context.ServiceCategories.Add(hardware);

        var screenIssues  = new ServiceCategory { CompanyId = companyId, Name = "Screen & Touch Issues",     ParentCategoryId = hardware.Id, SortOrder = 1, IsActive = true };
        var batteryIssues = new ServiceCategory { CompanyId = companyId, Name = "Battery & Charging Issues", ParentCategoryId = hardware.Id, SortOrder = 2, IsActive = true };
        var waterIssues   = new ServiceCategory { CompanyId = companyId, Name = "Water Damage",              ParentCategoryId = hardware.Id, SortOrder = 3, IsActive = true };
        var crownIssues   = new ServiceCategory { CompanyId = companyId, Name = "Crown & Button Issues",     ParentCategoryId = hardware.Id, SortOrder = 4, IsActive = true };
        var sensorIssues  = new ServiceCategory { CompanyId = companyId, Name = "Heart Rate / GPS Issues",   ParentCategoryId = hardware.Id, SortOrder = 5, IsActive = true };

        context.ServiceCategories.AddRange(screenIssues, batteryIssues, waterIssues, crownIssues, sensorIssues);

        // --- Software Issues ---
        var software = new ServiceCategory { CompanyId = companyId, Name = "Software Issues", SortOrder = 2, IsSubCategoryExist = true, IsActive = true };
        context.ServiceCategories.Add(software);

        var syncIssues   = new ServiceCategory { CompanyId = companyId, Name = "Sync & Pairing Issues", ParentCategoryId = software.Id, SortOrder = 1, IsActive = true };
        var updateIssues = new ServiceCategory { CompanyId = companyId, Name = "Update & Reset Issues", ParentCategoryId = software.Id, SortOrder = 2, IsActive = true };

        context.ServiceCategories.AddRange(syncIssues, updateIssues);

        // --- Sample services ---
        context.Services.AddRange(
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = screenIssues.Id,
                Name = "Apple Watch Screen Replacement",
                Description = "Professional OLED screen replacement with original quality digitizer and frame.",
                EstimatedDuration = "45 mins", BasePrice = 149.00m,
                RequiresInventory = true, IsActive = true, IsUniversal = false
            },
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = batteryIssues.Id,
                Name = "Smartwatch Battery Replacement",
                Description = "Replace worn-out battery to restore full-day battery life.",
                EstimatedDuration = "30 mins", BasePrice = 49.00m,
                RequiresInventory = true, IsActive = true, IsUniversal = true
            },
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = waterIssues.Id,
                Name = "Water Damage Assessment & Repair",
                Description = "Ultrasonic clean, dry and repair of water-damaged smartwatch components.",
                EstimatedDuration = "60 mins", BasePrice = 69.00m,
                RequiresInventory = false, IsActive = true, IsUniversal = true
            }
        );
    }

    private static async Task SeedDeviceBrandsAndModelsAsync(Flowtap_Repair.DbContext.IRepairDbContext context, CancellationToken ct)
    {
        // Apple Watch
        var apple = await GetOrCreateBrandAsync(context, "Apple", ct);
        await AddModelsAsync(context, apple, new[]
        {
            "Apple Watch Series 4", "Apple Watch Series 5", "Apple Watch Series 6",
            "Apple Watch Series 7", "Apple Watch Series 8", "Apple Watch Series 9",
            "Apple Watch Ultra", "Apple Watch Ultra 2", "Apple Watch SE (1st Gen)", "Apple Watch SE (2nd Gen)"
        }, ct);

        // Samsung Galaxy Watch
        var samsung = await GetOrCreateBrandAsync(context, "Samsung", ct);
        await AddModelsAsync(context, samsung, new[]
        {
            "Galaxy Watch 4", "Galaxy Watch 4 Classic",
            "Galaxy Watch 5", "Galaxy Watch 5 Pro",
            "Galaxy Watch 6", "Galaxy Watch 6 Classic"
        }, ct);

        // Garmin
        var garmin = await GetOrCreateBrandAsync(context, "Garmin", ct);
        await AddModelsAsync(context, garmin, new[]
        {
            "Garmin Venu 2", "Garmin Fenix 7", "Garmin Forerunner 255"
        }, ct);

        // Other brands — no models
        await GetOrCreateBrandAsync(context, "Fitbit", ct);
        await GetOrCreateBrandAsync(context, "Huawei", ct);
        await GetOrCreateBrandAsync(context, "Xiaomi", ct);
        await GetOrCreateBrandAsync(context, "Amazfit", ct);
        await GetOrCreateBrandAsync(context, "Fossil", ct);
    }

    private static async Task<DeviceBrand> GetOrCreateBrandAsync(Flowtap_Repair.DbContext.IRepairDbContext context, string name, CancellationToken ct)
    {
        var existing = await context.DeviceBrands.FirstOrDefaultAsync(b => b.Name == name, ct);
        if (existing != null) return existing;
        var brand = new DeviceBrand { Name = name, IsActive = true };
        context.DeviceBrands.Add(brand);
        return brand;
    }

    private static async Task AddModelsAsync(Flowtap_Repair.DbContext.IRepairDbContext context, DeviceBrand brand, IEnumerable<string> modelNames, CancellationToken ct)
    {
        foreach (var name in modelNames)
        {
            var exists = await context.DeviceModels.AnyAsync(m => m.BrandId == brand.Id && m.Name == name, ct);
            if (!exists)
                context.DeviceModels.Add(new DeviceModel { BrandId = brand.Id, Name = name, IsActive = true });
        }
    }
}
