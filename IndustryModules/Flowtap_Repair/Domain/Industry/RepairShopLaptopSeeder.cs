using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Domain.Industry;

public static class RepairShopLaptopSeeder
{
    public static async Task SeedAsync(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedProductCategories(context, companyId);
        SeedServiceCategories(context, companyId);
        await SeedDeviceBrandsAndModelsAsync(context, ct);
    }

    private static void SeedProductCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // --- Laptop Parts ---
        var laptopParts = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Laptop Parts",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsBrandExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(laptopParts);

        var screensCat = new ProductCategory { CompanyId = companyId, Name = "Screens & Display",  ParentCategoryId = laptopParts.Id, SortOrder = 1, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var batteriesCat = new ProductCategory { CompanyId = companyId, Name = "Batteries",          ParentCategoryId = laptopParts.Id, SortOrder = 2, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var keyboardsCat = new ProductCategory { CompanyId = companyId, Name = "Keyboards",          ParentCategoryId = laptopParts.Id, SortOrder = 3, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var hingesCat = new ProductCategory { CompanyId = companyId, Name = "Hinges",             ParentCategoryId = laptopParts.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var motherboardsCat = new ProductCategory { CompanyId = companyId, Name = "Motherboards",       ParentCategoryId = laptopParts.Id, SortOrder = 5, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var fansCat = new ProductCategory { CompanyId = companyId, Name = "Cooling Fans",       ParentCategoryId = laptopParts.Id, SortOrder = 6, IsDirectProductExist = true, IsActive = true };
        var storageCat = new ProductCategory { CompanyId = companyId, Name = "RAM & Storage",      ParentCategoryId = laptopParts.Id, SortOrder = 7, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(screensCat, batteriesCat, keyboardsCat, hingesCat, motherboardsCat, fansCat, storageCat);

        // --- Accessories ---
        var accessories = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Accessories",
            SortOrder = 2,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(accessories);

        var bagsCat = new ProductCategory { CompanyId = companyId, Name = "Laptop Bags",          ParentCategoryId = accessories.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var mouseCat = new ProductCategory { CompanyId = companyId, Name = "Mouse & Keyboards",    ParentCategoryId = accessories.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var hubsCat = new ProductCategory { CompanyId = companyId, Name = "USB Hubs",             ParentCategoryId = accessories.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var adaptersCat = new ProductCategory { CompanyId = companyId, Name = "Adapters & Chargers",  ParentCategoryId = accessories.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var padsCat = new ProductCategory { CompanyId = companyId, Name = "Cooling Pads",         ParentCategoryId = accessories.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(bagsCat, mouseCat, hubsCat, adaptersCat, padsCat);

        // --- Services ---
        var services = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Services",
            SortOrder = 3,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(services);

        var screenRep = new ProductCategory { CompanyId = companyId, Name = "Screen Replacement",  ParentCategoryId = services.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var batteryRep = new ProductCategory { CompanyId = companyId, Name = "Battery Replacement", ParentCategoryId = services.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var keyboardRep = new ProductCategory { CompanyId = companyId, Name = "Keyboard Repair",     ParentCategoryId = services.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var motherboardRep = new ProductCategory { CompanyId = companyId, Name = "Motherboard Repair",  ParentCategoryId = services.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var osRep = new ProductCategory { CompanyId = companyId, Name = "OS Installation",     ParentCategoryId = services.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        var dataRep = new ProductCategory { CompanyId = companyId, Name = "Data Recovery",       ParentCategoryId = services.Id, SortOrder = 6, IsDirectProductExist = true, IsActive = true };
        var virusRep = new ProductCategory { CompanyId = companyId, Name = "Virus Removal",       ParentCategoryId = services.Id, SortOrder = 7, IsDirectProductExist = true, IsActive = true };
        var ramRep = new ProductCategory { CompanyId = companyId, Name = "RAM Upgrade",         ParentCategoryId = services.Id, SortOrder = 8, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(screenRep, batteryRep, keyboardRep, motherboardRep, osRep, dataRep, virusRep, ramRep);

        // --- Seed Laptop Products ---
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId,
                CategoryId = screensCat.Id,
                Name = "Dell XPS 13 9300 Replacement Screen Assembly",
                Kind = ProductKind.SparePart,
                SKU = "LAP-SCR-DELL-XPS",
                DefaultCostPrice = 75.00m,
                DefaultSalePrice = 179.99m,
                IsSerialized = true,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = storageCat.Id,
                Name = "Crucial 16GB DDR4 3200MHz SODIMM RAM",
                Kind = ProductKind.SparePart,
                SKU = "LAP-RAM-CR-16G",
                DefaultCostPrice = 18.00m,
                DefaultSalePrice = 45.00m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = adaptersCat.Id,
                Name = "Universal 65W USB-C Laptop Charger Adapter",
                Kind = ProductKind.Accessory,
                SKU = "LAP-AC-CH-65W",
                DefaultCostPrice = 8.50m,
                DefaultSalePrice = 29.99m,
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
        var hardware = new ServiceCategory
        {
            CompanyId = companyId,
            Name = "Hardware Issues",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ServiceCategories.Add(hardware);

        var displayIssues = new ServiceCategory { CompanyId = companyId, Name = "Display Issues",    ParentCategoryId = hardware.Id, SortOrder = 1, IsActive = true };
        var batteryIssues = new ServiceCategory { CompanyId = companyId, Name = "Battery Problems",  ParentCategoryId = hardware.Id, SortOrder = 2, IsActive = true };
        var keyboardIssues = new ServiceCategory { CompanyId = companyId, Name = "Keyboard Issues",   ParentCategoryId = hardware.Id, SortOrder = 3, IsActive = true };
        var overheatIssues = new ServiceCategory { CompanyId = companyId, Name = "Overheating",       ParentCategoryId = hardware.Id, SortOrder = 4, IsActive = true };
        var bootIssues = new ServiceCategory { CompanyId = companyId, Name = "Boot Issues",       ParentCategoryId = hardware.Id, SortOrder = 5, IsActive = true };

        context.ServiceCategories.AddRange(displayIssues, batteryIssues, keyboardIssues, overheatIssues, bootIssues);

        // --- Software Issues ---
        var software = new ServiceCategory
        {
            CompanyId = companyId,
            Name = "Software Issues",
            SortOrder = 2,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ServiceCategories.Add(software);

        var osIssues = new ServiceCategory { CompanyId = companyId, Name = "OS Install/Reinstall", ParentCategoryId = software.Id, SortOrder = 1, IsActive = true };
        var dataIssues = new ServiceCategory { CompanyId = companyId, Name = "Data Recovery",        ParentCategoryId = software.Id, SortOrder = 2, IsActive = true };
        var virusIssues = new ServiceCategory { CompanyId = companyId, Name = "Virus Removal",        ParentCategoryId = software.Id, SortOrder = 3, IsActive = true };
        var softIssues = new ServiceCategory { CompanyId = companyId, Name = "Software Errors",      ParentCategoryId = software.Id, SortOrder = 4, IsActive = true };

        context.ServiceCategories.AddRange(osIssues, dataIssues, virusIssues, softIssues);

        // --- Seed Laptop Services ---
        context.Services.AddRange(
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = displayIssues.Id,
                Name = "Dell XPS 13 Screen Replacement Service",
                Description = "Professional replacement of Dell XPS 13 screen assembly with OEM quality screen.",
                EstimatedDuration = "45 mins",
                BasePrice = 199.99m,
                RequiresInventory = true,
                IsActive = true,
                IsUniversal = false
            },
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = osIssues.Id,
                Name = "OS Clean Installation & Driver Updates",
                Description = "Fresh clean install of Windows 10/11 or macOS, including full licensing, hardware driver configuration, and utility setup.",
                EstimatedDuration = "60 mins",
                BasePrice = 49.99m,
                RequiresInventory = false,
                IsActive = true,
                IsUniversal = true
            },
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = dataIssues.Id,
                Name = "Advanced Storage Data Recovery",
                Description = "Restoration of lost or corrupted files from HDD, SSD, or external flash storage media.",
                EstimatedDuration = "90 mins",
                BasePrice = 120.00m,
                RequiresInventory = false,
                IsActive = true,
                IsUniversal = true
            }
        );
    }

    private static async Task SeedDeviceBrandsAndModelsAsync(Flowtap_Repair.DbContext.IRepairDbContext context, CancellationToken ct)
    {
        // Dell
        var dell = await GetOrCreateBrandAsync(context, "Dell", ct);
        await AddModelsAsync(context, dell, new[] { "XPS 13", "XPS 15", "Inspiron 15", "Latitude 5420", "Vostro 3400" }, ct);

        // HP
        var hp = await GetOrCreateBrandAsync(context, "HP", ct);
        await AddModelsAsync(context, hp, new[] { "Pavilion 15", "EliteBook 840", "ProBook 440", "Envy x360", "Spectre x360" }, ct);

        // Lenovo
        var lenovo = await GetOrCreateBrandAsync(context, "Lenovo", ct);
        await AddModelsAsync(context, lenovo, new[] { "ThinkPad X1", "IdeaPad 3", "Legion 5", "Yoga 7", "ThinkBook 15" }, ct);

        // Apple
        var apple = await GetOrCreateBrandAsync(context, "Apple", ct);
        await AddModelsAsync(context, apple, new[] { "MacBook Air M1", "MacBook Air M2", "MacBook Pro 13", "MacBook Pro 14", "MacBook Pro 16" }, ct);

        // Other brands — no models
        await GetOrCreateBrandAsync(context, "Asus", ct);
        await GetOrCreateBrandAsync(context, "Acer", ct);
        await GetOrCreateBrandAsync(context, "MSI", ct);
        await GetOrCreateBrandAsync(context, "Samsung", ct);
        await GetOrCreateBrandAsync(context, "Toshiba", ct);
        await GetOrCreateBrandAsync(context, "Sony", ct);
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

