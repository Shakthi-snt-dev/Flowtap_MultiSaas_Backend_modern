using Flowtap_Repair.Domain.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Domain.Industry;

/// <summary>
/// Seeds data for printer and peripheral repair shops:
/// inkjet, laser, scanners, copiers, plotters, etc.
/// </summary>
public static class RepairShopPrinterSeeder
{
    public static async Task SeedAsync(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedProductCategories(context, companyId);
        SeedServiceCategories(context, companyId);
        await SeedDeviceBrandsAndModelsAsync(context, ct);
    }

    private static void SeedProductCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // --- Printer Parts (root) ---
        var parts = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Printer Parts",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsBrandExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(parts);

        var printHeadsCat   = new ProductCategory { CompanyId = companyId, Name = "Print Heads",                  ParentCategoryId = parts.Id, SortOrder = 1, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var drumUnitsCat    = new ProductCategory { CompanyId = companyId, Name = "Drum Units & OPC Drums",       ParentCategoryId = parts.Id, SortOrder = 2, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var fusersCat       = new ProductCategory { CompanyId = companyId, Name = "Fuser Assemblies",             ParentCategoryId = parts.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var rollersCat      = new ProductCategory { CompanyId = companyId, Name = "Pickup Rollers & Separation Pads", ParentCategoryId = parts.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var mainBoardsCat   = new ProductCategory { CompanyId = companyId, Name = "Formatter Boards & Main PCBs", ParentCategoryId = parts.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        var powerBoardsCat  = new ProductCategory { CompanyId = companyId, Name = "Power Supply Boards",          ParentCategoryId = parts.Id, SortOrder = 6, IsDirectProductExist = true, IsActive = true };
        var encodersCat     = new ProductCategory { CompanyId = companyId, Name = "Encoder Strips & Sensors",     ParentCategoryId = parts.Id, SortOrder = 7, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(printHeadsCat, drumUnitsCat, fusersCat, rollersCat, mainBoardsCat, powerBoardsCat, encodersCat);

        // --- Ink & Toner ---
        var inkToner = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Ink & Toner",
            SortOrder = 2,
            IsSubCategoryExist = true,
            IsBrandExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(inkToner);

        var originalInkCat  = new ProductCategory { CompanyId = companyId, Name = "Original Ink Cartridges",     ParentCategoryId = inkToner.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var compatInkCat    = new ProductCategory { CompanyId = companyId, Name = "Compatible Ink Cartridges",   ParentCategoryId = inkToner.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var tonerCat        = new ProductCategory { CompanyId = companyId, Name = "Toner Cartridges",            ParentCategoryId = inkToner.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var refillInkCat    = new ProductCategory { CompanyId = companyId, Name = "Bulk Ink & Refill Kits",      ParentCategoryId = inkToner.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(originalInkCat, compatInkCat, tonerCat, refillInkCat);

        // --- Printer Accessories ---
        var accessories = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Printer Accessories",
            SortOrder = 3,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(accessories);

        var cablesCat       = new ProductCategory { CompanyId = companyId, Name = "USB & Parallel Cables",       ParentCategoryId = accessories.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var paperCat        = new ProductCategory { CompanyId = companyId, Name = "Specialty Paper & Labels",    ParentCategoryId = accessories.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var cleaningKitsCat = new ProductCategory { CompanyId = companyId, Name = "Cleaning Kits & Swabs",       ParentCategoryId = accessories.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(cablesCat, paperCat, cleaningKitsCat);

        // --- Sample products ---
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId, CategoryId = printHeadsCat.Id,
                Name = "HP 932 / 933 Print Head Assembly",
                Kind = ProductKind.SparePart, SKU = "PR-PH-HP932",
                DefaultCostPrice = 18.00m, DefaultSalePrice = 54.99m,
                IsSerialized = false, IsUniversal = false, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId, CategoryId = drumUnitsCat.Id,
                Name = "Brother DR-2300 Drum Unit",
                Kind = ProductKind.SparePart, SKU = "PR-DRM-BR2300",
                DefaultCostPrice = 14.00m, DefaultSalePrice = 39.99m,
                IsSerialized = false, IsUniversal = false, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId, CategoryId = rollersCat.Id,
                Name = "Universal Pickup Roller Kit (HP LaserJet)",
                Kind = ProductKind.SparePart, SKU = "PR-RLR-HP-KIT",
                DefaultCostPrice = 5.00m, DefaultSalePrice = 19.99m,
                IsSerialized = false, IsUniversal = true, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId, CategoryId = refillInkCat.Id,
                Name = "Universal Dye Ink Refill Kit (100ml x4 CMYK)",
                Kind = ProductKind.SparePart, SKU = "PR-INK-CMYK-100",
                DefaultCostPrice = 3.50m, DefaultSalePrice = 12.99m,
                IsSerialized = false, IsUniversal = true, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );
    }

    private static void SeedServiceCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // --- Print Quality Issues ---
        var printQuality = new ServiceCategory { CompanyId = companyId, Name = "Print Quality Issues", SortOrder = 1, IsSubCategoryExist = true, IsActive = true };
        context.ServiceCategories.Add(printQuality);

        var blurryIssues    = new ServiceCategory { CompanyId = companyId, Name = "Blurry / Faded Prints",        ParentCategoryId = printQuality.Id, SortOrder = 1, IsActive = true };
        var streakIssues    = new ServiceCategory { CompanyId = companyId, Name = "Streaks & Lines on Prints",    ParentCategoryId = printQuality.Id, SortOrder = 2, IsActive = true };
        var colourIssues    = new ServiceCategory { CompanyId = companyId, Name = "Colour Mismatch / Missing Ink", ParentCategoryId = printQuality.Id, SortOrder = 3, IsActive = true };
        var headCleanIssues = new ServiceCategory { CompanyId = companyId, Name = "Print Head Clogging",          ParentCategoryId = printQuality.Id, SortOrder = 4, IsActive = true };

        context.ServiceCategories.AddRange(blurryIssues, streakIssues, colourIssues, headCleanIssues);

        // --- Paper & Mechanical Issues ---
        var mechanical = new ServiceCategory { CompanyId = companyId, Name = "Paper & Mechanical Issues", SortOrder = 2, IsSubCategoryExist = true, IsActive = true };
        context.ServiceCategories.Add(mechanical);

        var paperJamIssues  = new ServiceCategory { CompanyId = companyId, Name = "Paper Jam",                    ParentCategoryId = mechanical.Id, SortOrder = 1, IsActive = true };
        var paperFeedIssues = new ServiceCategory { CompanyId = companyId, Name = "Paper Not Feeding",            ParentCategoryId = mechanical.Id, SortOrder = 2, IsActive = true };
        var rollerIssues    = new ServiceCategory { CompanyId = companyId, Name = "Roller Wear / Slipping",       ParentCategoryId = mechanical.Id, SortOrder = 3, IsActive = true };

        context.ServiceCategories.AddRange(paperJamIssues, paperFeedIssues, rollerIssues);

        // --- Power & Connectivity Issues ---
        var connectivity = new ServiceCategory { CompanyId = companyId, Name = "Power & Connectivity Issues", SortOrder = 3, IsSubCategoryExist = true, IsActive = true };
        context.ServiceCategories.Add(connectivity);

        var noPowerIssues   = new ServiceCategory { CompanyId = companyId, Name = "Printer Not Turning On",       ParentCategoryId = connectivity.Id, SortOrder = 1, IsActive = true };
        var wifiIssues      = new ServiceCategory { CompanyId = companyId, Name = "Wi-Fi / Network Issues",       ParentCategoryId = connectivity.Id, SortOrder = 2, IsActive = true };
        var usbIssues       = new ServiceCategory { CompanyId = companyId, Name = "USB Not Detected",             ParentCategoryId = connectivity.Id, SortOrder = 3, IsActive = true };
        var errorCodeIssues = new ServiceCategory { CompanyId = companyId, Name = "Error Codes & Firmware",       ParentCategoryId = connectivity.Id, SortOrder = 4, IsActive = true };

        context.ServiceCategories.AddRange(noPowerIssues, wifiIssues, usbIssues, errorCodeIssues);

        // --- Sample services ---
        context.Services.AddRange(
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = headCleanIssues.Id,
                Name = "Print Head Deep Clean & Alignment",
                Description = "Professional ultrasonic or manual print head clean with nozzle check and alignment calibration.",
                EstimatedDuration = "30 mins", BasePrice = 24.99m,
                RequiresInventory = false, IsActive = true, IsUniversal = true
            },
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = paperJamIssues.Id,
                Name = "Paper Jam Clearance & Roller Service",
                Description = "Full disassembly to remove stuck paper, inspect and clean all rollers, test multiple page feeds.",
                EstimatedDuration = "30 mins", BasePrice = 29.99m,
                RequiresInventory = false, IsActive = true, IsUniversal = true
            },
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = blurryIssues.Id,
                Name = "Print Head Replacement",
                Description = "Replace damaged or permanently clogged print head assembly. OEM quality replacement installed.",
                EstimatedDuration = "45 mins", BasePrice = 69.99m,
                RequiresInventory = true, IsActive = true, IsUniversal = false
            },
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = noPowerIssues.Id,
                Name = "Printer Power Board Repair",
                Description = "Diagnose and repair or replace faulty power supply board on any desktop printer model.",
                EstimatedDuration = "45 mins", BasePrice = 49.99m,
                RequiresInventory = true, IsActive = true, IsUniversal = true
            }
        );
    }

    private static async Task SeedDeviceBrandsAndModelsAsync(Flowtap_Repair.DbContext.IRepairDbContext context, CancellationToken ct)
    {
        // HP
        var hp = await GetOrCreateBrandAsync(context, "HP", ct);
        await AddModelsAsync(context, hp, new[]
        {
            "HP DeskJet 2700", "HP DeskJet 3700", "HP OfficeJet Pro 9010",
            "HP LaserJet Pro M15a", "HP LaserJet Pro M404n", "HP LaserJet MFP M428"
        }, ct);

        // Canon
        var canon = await GetOrCreateBrandAsync(context, "Canon", ct);
        await AddModelsAsync(context, canon, new[]
        {
            "Canon PIXMA G3010", "Canon PIXMA G6010", "Canon PIXMA TR4520",
            "Canon imageCLASS MF3010", "Canon imageCLASS LBP6030"
        }, ct);

        // Epson
        var epson = await GetOrCreateBrandAsync(context, "Epson", ct);
        await AddModelsAsync(context, epson, new[]
        {
            "Epson L3110", "Epson L3150", "Epson L6190",
            "Epson EcoTank ET-2760", "Epson WorkForce WF-3720"
        }, ct);

        // Brother
        var brother = await GetOrCreateBrandAsync(context, "Brother", ct);
        await AddModelsAsync(context, brother, new[]
        {
            "Brother HL-1200", "Brother HL-L2350DW", "Brother DCP-L2540DW",
            "Brother MFC-J995DW"
        }, ct);

        // Other brands — no models
        await GetOrCreateBrandAsync(context, "Xerox", ct);
        await GetOrCreateBrandAsync(context, "Lexmark", ct);
        await GetOrCreateBrandAsync(context, "Ricoh", ct);
        await GetOrCreateBrandAsync(context, "Kyocera", ct);
        await GetOrCreateBrandAsync(context, "Samsung", ct);  // Samsung printers (now sold to HP)
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
