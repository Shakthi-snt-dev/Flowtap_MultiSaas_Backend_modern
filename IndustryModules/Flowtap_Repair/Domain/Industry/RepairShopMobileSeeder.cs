using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Domain.Industry;

public static class RepairShopMobileSeeder
{
    public static async Task SeedAsync(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedProductCategories(context, companyId);
        SeedServiceCategories(context, companyId);
        await SeedDeviceBrandsAndModelsAsync(context, ct);
    }

    private static void SeedProductCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // --- Spare Parts (parent) ---
        var spareParts = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Spare Parts",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsBrandExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(spareParts);

        var screensCat = new ProductCategory { CompanyId = companyId, Name = "Screens & Displays", ParentCategoryId = spareParts.Id, SortOrder = 1, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var batteriesCat = new ProductCategory { CompanyId = companyId, Name = "Batteries", ParentCategoryId = spareParts.Id, SortOrder = 2, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var backCoversCat = new ProductCategory { CompanyId = companyId, Name = "Back Covers", ParentCategoryId = spareParts.Id, SortOrder = 3, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var camerasCat = new ProductCategory { CompanyId = companyId, Name = "Cameras & Lenses", ParentCategoryId = spareParts.Id, SortOrder = 4, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var portsCat = new ProductCategory { CompanyId = companyId, Name = "Charging Ports & Cables", ParentCategoryId = spareParts.Id, SortOrder = 5, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var audioCat = new ProductCategory { CompanyId = companyId, Name = "Speakers & Microphones", ParentCategoryId = spareParts.Id, SortOrder = 6, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var otherSpareCat = new ProductCategory { CompanyId = companyId, Name = "Other Spare Parts", ParentCategoryId = spareParts.Id, SortOrder = 7, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(screensCat, batteriesCat, backCoversCat, camerasCat, portsCat, audioCat, otherSpareCat);

        // --- Accessories (parent) ---
        var accessories = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Accessories",
            SortOrder = 2,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(accessories);

        var casesCat = new ProductCategory { CompanyId = companyId, Name = "Phone Cases & Covers", ParentCategoryId = accessories.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var glassCat = new ProductCategory { CompanyId = companyId, Name = "Tempered Glass & Screen Protectors", ParentCategoryId = accessories.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var chargersCat = new ProductCategory { CompanyId = companyId, Name = "Chargers & Cables", ParentCategoryId = accessories.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var audioAccCat = new ProductCategory { CompanyId = companyId, Name = "Earphones & Headphones", ParentCategoryId = accessories.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var powerCat = new ProductCategory { CompanyId = companyId, Name = "Power Banks", ParentCategoryId = accessories.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        var storageCat = new ProductCategory { CompanyId = companyId, Name = "Memory Cards & USB", ParentCategoryId = accessories.Id, SortOrder = 6, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(casesCat, glassCat, chargersCat, audioAccCat, powerCat, storageCat);

        // --- Services (parent) ---
        var services = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Services",
            SortOrder = 3,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(services);

        var screenRep = new ProductCategory { CompanyId = companyId, Name = "Screen Repair", ParentCategoryId = services.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var batteryRep = new ProductCategory { CompanyId = companyId, Name = "Battery Replacement", ParentCategoryId = services.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var waterRep = new ProductCategory { CompanyId = companyId, Name = "Water Damage Repair", ParentCategoryId = services.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var softwareRep = new ProductCategory { CompanyId = companyId, Name = "Software & Unlocking", ParentCategoryId = services.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var otherRep = new ProductCategory { CompanyId = companyId, Name = "Other Repairs", ParentCategoryId = services.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(screenRep, batteryRep, waterRep, softwareRep, otherRep);

        // --- Seed Products ---
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId,
                CategoryId = screensCat.Id,
                Name = "iPhone 13 OEM Screen Assembly",
                Kind = ProductKind.SparePart,
                SKU = "SCR-IP13-OEM",
                DefaultCostPrice = 65.00m,
                DefaultSalePrice = 139.00m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = screensCat.Id,
                Name = "Samsung Galaxy S22 Ultra Screen with Frame",
                Kind = ProductKind.SparePart,
                SKU = "SCR-S22U-FRM",
                DefaultCostPrice = 95.00m,
                DefaultSalePrice = 199.00m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = batteriesCat.Id,
                Name = "iPhone 11 Replacement Battery (3110 mAh)",
                Kind = ProductKind.SparePart,
                SKU = "BAT-IP11",
                DefaultCostPrice = 12.00m,
                DefaultSalePrice = 39.00m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = glassCat.Id,
                Name = "Premium 9H Tempered Glass - iPhone 14 Pro Max",
                Kind = ProductKind.Accessory,
                SKU = "ACC-TG-IP14PM",
                DefaultCostPrice = 1.50m,
                DefaultSalePrice = 14.99m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = chargersCat.Id,
                Name = "20W USB-C PD Fast Charging Adapter",
                Kind = ProductKind.Accessory,
                SKU = "ACC-CH-20W",
                DefaultCostPrice = 6.00m,
                DefaultSalePrice = 24.99m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );
    }

    private static void SeedServiceCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // --- Hardware Issues ---
        var hardwareIssues = new ServiceCategory
        {
            CompanyId = companyId,
            Name = "Hardware Issues",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ServiceCategories.Add(hardwareIssues);

        var screenDisplayIssues = new ServiceCategory { CompanyId = companyId, Name = "Screen & Display Issues", ParentCategoryId = hardwareIssues.Id, SortOrder = 1, IsActive = true };
        var batteryIssues = new ServiceCategory { CompanyId = companyId, Name = "Battery & Charging Issues", ParentCategoryId = hardwareIssues.Id, SortOrder = 2, IsActive = true };
        var cameraIssues = new ServiceCategory { CompanyId = companyId, Name = "Camera Issues", ParentCategoryId = hardwareIssues.Id, SortOrder = 3, IsActive = true };
        var waterDamageIssues = new ServiceCategory { CompanyId = companyId, Name = "Water Damage", ParentCategoryId = hardwareIssues.Id, SortOrder = 4, IsActive = true };
        var physicalDamageIssues = new ServiceCategory { CompanyId = companyId, Name = "Physical Damage", ParentCategoryId = hardwareIssues.Id, SortOrder = 5, IsActive = true };

        context.ServiceCategories.AddRange(screenDisplayIssues, batteryIssues, cameraIssues, waterDamageIssues, physicalDamageIssues);

        // --- Software Issues ---
        var softwareIssues = new ServiceCategory
        {
            CompanyId = companyId,
            Name = "Software Issues",
            SortOrder = 2,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ServiceCategories.Add(softwareIssues);

        var osUpdateIssues = new ServiceCategory { CompanyId = companyId, Name = "OS & Software Update", ParentCategoryId = softwareIssues.Id, SortOrder = 1, IsActive = true };
        var bypassIssues = new ServiceCategory { CompanyId = companyId, Name = "Unlocking & Bypass", ParentCategoryId = softwareIssues.Id, SortOrder = 2, IsActive = true };
        var recoveryIssues = new ServiceCategory { CompanyId = companyId, Name = "Data Recovery", ParentCategoryId = softwareIssues.Id, SortOrder = 3, IsActive = true };
        var malwareIssues = new ServiceCategory { CompanyId = companyId, Name = "Virus & Malware", ParentCategoryId = softwareIssues.Id, SortOrder = 4, IsActive = true };

        context.ServiceCategories.AddRange(osUpdateIssues, bypassIssues, recoveryIssues, malwareIssues);

        // --- Other Issues ---
        var otherIssues = new ServiceCategory
        {
            CompanyId = companyId,
            Name = "Other Issues",
            SortOrder = 3,
            IsDirectProductExist = true,
            IsActive = true
        };
        context.ServiceCategories.Add(otherIssues);

        // --- Seed Services ---
        context.Services.AddRange(
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = screenDisplayIssues.Id,
                Name = "iPhone 13 Screen Replacement Service",
                Description = "Professional installation of a high-quality OEM/Premium screen assembly.",
                EstimatedDuration = "45 mins",
                BasePrice = 149.00m,
                RequiresInventory = true,
                IsActive = true,
                IsUniversal = false
            },
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = screenDisplayIssues.Id,
                Name = "Galaxy S22 Ultra Screen Repair Service",
                Description = "Replacement of broken display with complete frame and original glass.",
                EstimatedDuration = "60 mins",
                BasePrice = 229.00m,
                RequiresInventory = true,
                IsActive = true,
                IsUniversal = false
            },
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = batteryIssues.Id,
                Name = "iPhone Battery Replacement Service",
                Description = "Replace old battery with a brand new premium zero-cycle battery.",
                EstimatedDuration = "30 mins",
                BasePrice = 59.00m,
                RequiresInventory = true,
                IsActive = true,
                IsUniversal = false
            },
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = osUpdateIssues.Id,
                Name = "OS Flash & Software Recovery",
                Description = "Resolving boot loops, stuck logo issues, and complete OS reinstalls.",
                EstimatedDuration = "45 mins",
                BasePrice = 45.00m,
                RequiresInventory = false,
                IsActive = true,
                IsUniversal = true
            },
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = bypassIssues.Id,
                Name = "Device Diagnostics & Troubleshooting",
                Description = "Complete checkup to identify hardware or motherboard faults.",
                EstimatedDuration = "30 mins",
                BasePrice = 29.00m,
                RequiresInventory = false,
                IsActive = true,
                IsUniversal = true
            }
        );
    }

    private static async Task SeedDeviceBrandsAndModelsAsync(Flowtap_Repair.DbContext.IRepairDbContext context, CancellationToken ct)
    {
        // Apple
        var apple = await GetOrCreateBrandAsync(context, "Apple", ct);
        await AddModelsAsync(context, apple, new[]
        {
            "iPhone 7", "iPhone 7 Plus", "iPhone 8", "iPhone 8 Plus",
            "iPhone X", "iPhone XR", "iPhone XS", "iPhone XS Max",
            "iPhone 11", "iPhone 11 Pro", "iPhone 11 Pro Max",
            "iPhone 12", "iPhone 12 Mini", "iPhone 12 Pro", "iPhone 12 Pro Max",
            "iPhone 13", "iPhone 13 Mini", "iPhone 13 Pro", "iPhone 13 Pro Max",
            "iPhone 14", "iPhone 14 Plus", "iPhone 14 Pro", "iPhone 14 Pro Max",
            "iPhone 15", "iPhone 15 Plus", "iPhone 15 Pro", "iPhone 15 Pro Max"
        }, ct);

        // Samsung
        var samsung = await GetOrCreateBrandAsync(context, "Samsung", ct);
        await AddModelsAsync(context, samsung, new[]
        {
            "Galaxy S21", "Galaxy S21+", "Galaxy S21 Ultra",
            "Galaxy S22", "Galaxy S22+", "Galaxy S22 Ultra",
            "Galaxy S23", "Galaxy S23+", "Galaxy S23 Ultra",
            "Galaxy S24", "Galaxy S24+", "Galaxy S24 Ultra",
            "Galaxy A32", "Galaxy A52", "Galaxy A72",
            "Galaxy A53", "Galaxy A73", "Galaxy A54",
            "Galaxy Note 20", "Galaxy Note 20 Ultra"
        }, ct);

        // Xiaomi
        var xiaomi = await GetOrCreateBrandAsync(context, "Xiaomi", ct);
        await AddModelsAsync(context, xiaomi, new[]
        {
            "Redmi Note 10", "Redmi Note 11", "Redmi Note 12", "Redmi Note 13",
            "Redmi 9", "Redmi 10", "Redmi 12",
            "Xiaomi 11", "Xiaomi 12", "Xiaomi 13", "Mi 11 Lite"
        }, ct);

        // Redmi (brand)
        await GetOrCreateBrandAsync(context, "Redmi", ct);

        // OnePlus
        var oneplus = await GetOrCreateBrandAsync(context, "OnePlus", ct);
        await AddModelsAsync(context, oneplus, new[]
        {
            "OnePlus 8", "OnePlus 9", "OnePlus 10", "OnePlus 11",
            "OnePlus Nord", "OnePlus Nord CE 2", "OnePlus Nord CE 3"
        }, ct);

        // Oppo
        var oppo = await GetOrCreateBrandAsync(context, "Oppo", ct);
        await AddModelsAsync(context, oppo, new[]
        {
            "Oppo A57", "Oppo A77", "Oppo Reno8", "Oppo F21 Pro", "Oppo A96"
        }, ct);

        // Vivo
        var vivo = await GetOrCreateBrandAsync(context, "Vivo", ct);
        await AddModelsAsync(context, vivo, new[]
        {
            "Vivo Y75", "Vivo Y33s", "Vivo V23", "Vivo X80", "Vivo T1"
        }, ct);

        // Realme
        var realme = await GetOrCreateBrandAsync(context, "Realme", ct);
        await AddModelsAsync(context, realme, new[]
        {
            "Realme 10", "Realme 9 Pro", "Realme C35", "Realme 8", "Realme Narzo 50"
        }, ct);

        // Other brands â€” no models defined, just create the brand
        await GetOrCreateBrandAsync(context, "Huawei", ct);
        await GetOrCreateBrandAsync(context, "Nokia", ct);
        await GetOrCreateBrandAsync(context, "Motorola", ct);
        await GetOrCreateBrandAsync(context, "Sony", ct);
        await GetOrCreateBrandAsync(context, "LG", ct);
        await GetOrCreateBrandAsync(context, "Google", ct);
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
            {
                context.DeviceModels.Add(new DeviceModel
                {
                    BrandId = brand.Id,
                    Name = name,
                    IsActive = true
                });
            }
        }
    }
}

