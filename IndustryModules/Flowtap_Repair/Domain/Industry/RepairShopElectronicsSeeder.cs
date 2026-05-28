using Flowtap_Repair.Domain.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Domain.Industry;

/// <summary>
/// Seeds data for general electronics repair shops:
/// home appliances, audio equipment, AC units, washing machines, etc.
/// </summary>
public static class RepairShopElectronicsSeeder
{
    public static async Task SeedAsync(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedProductCategories(context, companyId);
        SeedServiceCategories(context, companyId);
        await SeedDeviceBrandsAndModelsAsync(context, ct);
    }

    private static void SeedProductCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // --- Electronics Parts (root) ---
        var electronicsParts = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Electronics Parts",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsBrandExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(electronicsParts);

        var powerBoardsCat    = new ProductCategory { CompanyId = companyId, Name = "Power Boards & Supplies",    ParentCategoryId = electronicsParts.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var controlBoardsCat  = new ProductCategory { CompanyId = companyId, Name = "Control Boards & PCBs",      ParentCategoryId = electronicsParts.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var motorsCat         = new ProductCategory { CompanyId = companyId, Name = "Motors & Pumps",             ParentCategoryId = electronicsParts.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var compressorsCat    = new ProductCategory { CompanyId = companyId, Name = "Compressors & Coils",        ParentCategoryId = electronicsParts.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var heatingCat        = new ProductCategory { CompanyId = companyId, Name = "Heating Elements",           ParentCategoryId = electronicsParts.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        var remoteCat         = new ProductCategory { CompanyId = companyId, Name = "Remote Controls",            ParentCategoryId = electronicsParts.Id, SortOrder = 6, IsDirectProductExist = true, IsActive = true };
        var capacitorsCat     = new ProductCategory { CompanyId = companyId, Name = "Capacitors & Resistors",     ParentCategoryId = electronicsParts.Id, SortOrder = 7, IsDirectProductExist = true, IsActive = true };
        var otherPartsCat     = new ProductCategory { CompanyId = companyId, Name = "Other Electronic Parts",     ParentCategoryId = electronicsParts.Id, SortOrder = 8, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(powerBoardsCat, controlBoardsCat, motorsCat, compressorsCat, heatingCat, remoteCat, capacitorsCat, otherPartsCat);

        // --- Electronics Accessories ---
        var accessories = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Electronics Accessories",
            SortOrder = 2,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(accessories);

        var cablesCat    = new ProductCategory { CompanyId = companyId, Name = "Cables & Connectors",    ParentCategoryId = accessories.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var filtersCat   = new ProductCategory { CompanyId = companyId, Name = "Filters & Belts",        ParentCategoryId = accessories.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var lubricantsCat= new ProductCategory { CompanyId = companyId, Name = "Lubricants & Cleaners",  ParentCategoryId = accessories.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(cablesCat, filtersCat, lubricantsCat);

        // --- Electronics Services ---
        var services = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Electronics Services",
            SortOrder = 3,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(services);

        var acRepCat     = new ProductCategory { CompanyId = companyId, Name = "AC & Cooling Repair",      ParentCategoryId = services.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var washRepCat   = new ProductCategory { CompanyId = companyId, Name = "Washing Machine Repair",   ParentCategoryId = services.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var fridgeRepCat = new ProductCategory { CompanyId = companyId, Name = "Refrigerator Repair",      ParentCategoryId = services.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var microRepCat  = new ProductCategory { CompanyId = companyId, Name = "Microwave Repair",         ParentCategoryId = services.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var audioRepCat  = new ProductCategory { CompanyId = companyId, Name = "Audio & Speaker Repair",   ParentCategoryId = services.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        var genRepCat    = new ProductCategory { CompanyId = companyId, Name = "General Electronics Repair", ParentCategoryId = services.Id, SortOrder = 6, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(acRepCat, washRepCat, fridgeRepCat, microRepCat, audioRepCat, genRepCat);

        // --- Sample products ---
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId, CategoryId = powerBoardsCat.Id,
                Name = "Universal AC Power Board 5A 220V",
                Kind = ProductKind.SparePart, SKU = "EL-PB-AC-5A",
                DefaultCostPrice = 8.00m, DefaultSalePrice = 29.99m,
                IsSerialized = false, IsUniversal = true, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId, CategoryId = compressorsCat.Id,
                Name = "Rotary Compressor 1.5 Ton R22",
                Kind = ProductKind.SparePart, SKU = "EL-CP-15T",
                DefaultCostPrice = 85.00m, DefaultSalePrice = 219.99m,
                IsSerialized = true, IsUniversal = false, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId, CategoryId = filtersCat.Id,
                Name = "Washing Machine Drive Belt (Universal)",
                Kind = ProductKind.SparePart, SKU = "EL-WM-BELT",
                DefaultCostPrice = 2.50m, DefaultSalePrice = 12.99m,
                IsSerialized = false, IsUniversal = true, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );
    }

    private static void SeedServiceCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // --- Cooling & Climate ---
        var cooling = new ServiceCategory { CompanyId = companyId, Name = "Cooling & Climate Control", SortOrder = 1, IsSubCategoryExist = true, IsActive = true };
        context.ServiceCategories.Add(cooling);

        var acGasIssues   = new ServiceCategory { CompanyId = companyId, Name = "AC Gas Refill",          ParentCategoryId = cooling.Id, SortOrder = 1, IsActive = true };
        var acCompIssues  = new ServiceCategory { CompanyId = companyId, Name = "AC Compressor Fault",    ParentCategoryId = cooling.Id, SortOrder = 2, IsActive = true };
        var acCleanIssues = new ServiceCategory { CompanyId = companyId, Name = "AC Cleaning & Service",  ParentCategoryId = cooling.Id, SortOrder = 3, IsActive = true };
        var fridgeIssues  = new ServiceCategory { CompanyId = companyId, Name = "Refrigerator Not Cooling", ParentCategoryId = cooling.Id, SortOrder = 4, IsActive = true };
        context.ServiceCategories.AddRange(acGasIssues, acCompIssues, acCleanIssues, fridgeIssues);

        // --- Home Appliances ---
        var appliances = new ServiceCategory { CompanyId = companyId, Name = "Home Appliances", SortOrder = 2, IsSubCategoryExist = true, IsActive = true };
        context.ServiceCategories.Add(appliances);

        var washIssues   = new ServiceCategory { CompanyId = companyId, Name = "Washing Machine Not Spinning",  ParentCategoryId = appliances.Id, SortOrder = 1, IsActive = true };
        var drainIssues  = new ServiceCategory { CompanyId = companyId, Name = "Drainage & Pump Issues",        ParentCategoryId = appliances.Id, SortOrder = 2, IsActive = true };
        var microIssues  = new ServiceCategory { CompanyId = companyId, Name = "Microwave Not Heating",         ParentCategoryId = appliances.Id, SortOrder = 3, IsActive = true };
        var powerIssues  = new ServiceCategory { CompanyId = companyId, Name = "No Power / Dead Appliance",     ParentCategoryId = appliances.Id, SortOrder = 4, IsActive = true };
        context.ServiceCategories.AddRange(washIssues, drainIssues, microIssues, powerIssues);

        // --- Audio & AV ---
        var audio = new ServiceCategory { CompanyId = companyId, Name = "Audio & AV Equipment", SortOrder = 3, IsSubCategoryExist = true, IsActive = true };
        context.ServiceCategories.Add(audio);

        var speakerIssues = new ServiceCategory { CompanyId = companyId, Name = "Speaker Distortion", ParentCategoryId = audio.Id, SortOrder = 1, IsActive = true };
        var ampIssues     = new ServiceCategory { CompanyId = companyId, Name = "Amplifier Repair",   ParentCategoryId = audio.Id, SortOrder = 2, IsActive = true };
        context.ServiceCategories.AddRange(speakerIssues, ampIssues);

        // --- Sample services ---
        context.Services.AddRange(
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = acGasIssues.Id,
                Name = "AC Gas Refill & Performance Check",
                Description = "Recharge AC with correct refrigerant, check for leaks, test cooling performance.",
                EstimatedDuration = "45 mins", BasePrice = 59.99m,
                RequiresInventory = true, IsActive = true, IsUniversal = true
            },
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = washIssues.Id,
                Name = "Washing Machine Motor Replacement",
                Description = "Diagnose and replace faulty motor or capacitor on front or top-load machines.",
                EstimatedDuration = "60 mins", BasePrice = 89.99m,
                RequiresInventory = true, IsActive = true, IsUniversal = true
            },
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = powerIssues.Id,
                Name = "Appliance Power Board Repair",
                Description = "Diagnose and replace blown fuses, capacitors, or power supply boards.",
                EstimatedDuration = "45 mins", BasePrice = 49.99m,
                RequiresInventory = true, IsActive = true, IsUniversal = true
            }
        );
    }

    private static async Task SeedDeviceBrandsAndModelsAsync(Flowtap_Repair.DbContext.IRepairDbContext context, CancellationToken ct)
    {
        // LG
        var lg = await GetOrCreateBrandAsync(context, "LG", ct);
        await AddModelsAsync(context, lg, new[] { "LG Split AC 1.5 Ton", "LG Front Load 7kg", "LG French Door Fridge" }, ct);

        // Samsung
        var samsung = await GetOrCreateBrandAsync(context, "Samsung", ct);
        await AddModelsAsync(context, samsung, new[] { "Samsung Split AC 1 Ton", "Samsung Top Load 8kg", "Samsung Side-by-Side Fridge" }, ct);

        // Panasonic
        var panasonic = await GetOrCreateBrandAsync(context, "Panasonic", ct);
        await AddModelsAsync(context, panasonic, new[] { "Panasonic Split AC 2 Ton", "Panasonic Microwave 23L" }, ct);

        // Other brands — no models
        await GetOrCreateBrandAsync(context, "Bosch", ct);
        await GetOrCreateBrandAsync(context, "Whirlpool", ct);
        await GetOrCreateBrandAsync(context, "Haier", ct);
        await GetOrCreateBrandAsync(context, "Dawlance", ct);
        await GetOrCreateBrandAsync(context, "Gree", ct);
        await GetOrCreateBrandAsync(context, "Daikin", ct);
        await GetOrCreateBrandAsync(context, "Mitsubishi Electric", ct);
        await GetOrCreateBrandAsync(context, "Sony", ct);
        await GetOrCreateBrandAsync(context, "JBL", ct);
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
