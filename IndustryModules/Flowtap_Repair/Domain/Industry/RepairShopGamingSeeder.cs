using Flowtap_Repair.Domain.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Domain.Industry;

/// <summary>
/// Seeds data for gaming console repair shops:
/// PlayStation, Xbox, Nintendo Switch, etc.
/// </summary>
public static class RepairShopGamingSeeder
{
    public static async Task SeedAsync(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedProductCategories(context, companyId);
        SeedServiceCategories(context, companyId);
        await SeedDeviceBrandsAndModelsAsync(context, ct);
    }

    private static void SeedProductCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // --- Gaming Console Parts (root) ---
        var parts = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Gaming Console Parts",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsBrandExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(parts);

        var discDrivesCat   = new ProductCategory { CompanyId = companyId, Name = "Disc Drives & Lasers",      ParentCategoryId = parts.Id, SortOrder = 1, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var hdmiCat         = new ProductCategory { CompanyId = companyId, Name = "HDMI Ports & IC Chips",     ParentCategoryId = parts.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var fansCat         = new ProductCategory { CompanyId = companyId, Name = "Fans & Cooling Systems",    ParentCategoryId = parts.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var powerSupplyCat  = new ProductCategory { CompanyId = companyId, Name = "Power Supplies & AC Adapters", ParentCategoryId = parts.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var motherboardsCat = new ProductCategory { CompanyId = companyId, Name = "Motherboards & APUs",       ParentCategoryId = parts.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        var shellsCat       = new ProductCategory { CompanyId = companyId, Name = "Shells & Housing",          ParentCategoryId = parts.Id, SortOrder = 6, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(discDrivesCat, hdmiCat, fansCat, powerSupplyCat, motherboardsCat, shellsCat);

        // --- Controller Parts ---
        var controllerParts = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Controller Parts",
            SortOrder = 2,
            IsSubCategoryExist = true,
            IsBrandExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(controllerParts);

        var thumbsticksCat  = new ProductCategory { CompanyId = companyId, Name = "Thumbsticks & Joysticks",   ParentCategoryId = controllerParts.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var triggersCat     = new ProductCategory { CompanyId = companyId, Name = "Triggers & Bumpers",        ParentCategoryId = controllerParts.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var controlBattCat  = new ProductCategory { CompanyId = companyId, Name = "Controller Batteries",      ParentCategoryId = controllerParts.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var buttonsCat      = new ProductCategory { CompanyId = companyId, Name = "Buttons & D-Pads",          ParentCategoryId = controllerParts.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(thumbsticksCat, triggersCat, controlBattCat, buttonsCat);

        // --- Gaming Accessories ---
        var accessories = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Gaming Accessories",
            SortOrder = 3,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(accessories);

        var hdmiCablesCat   = new ProductCategory { CompanyId = companyId, Name = "HDMI Cables",               ParentCategoryId = accessories.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var chargingDockCat = new ProductCategory { CompanyId = companyId, Name = "Charging Docks & Stands",   ParentCategoryId = accessories.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var headsetsPartsCat= new ProductCategory { CompanyId = companyId, Name = "Headset Parts & Cables",    ParentCategoryId = accessories.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var thermalCat      = new ProductCategory { CompanyId = companyId, Name = "Thermal Paste & Pads",      ParentCategoryId = accessories.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(hdmiCablesCat, chargingDockCat, headsetsPartsCat, thermalCat);

        // --- Sample products ---
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId, CategoryId = discDrivesCat.Id,
                Name = "PS5 Disc Drive Replacement (CFI-1xxx)",
                Kind = ProductKind.SparePart, SKU = "GC-DD-PS5",
                DefaultCostPrice = 45.00m, DefaultSalePrice = 119.00m,
                IsSerialized = false, IsUniversal = false, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId, CategoryId = thumbsticksCat.Id,
                Name = "PS4 / PS5 DualShock Thumbstick Replacement (Pair)",
                Kind = ProductKind.SparePart, SKU = "GC-TS-DS-PR",
                DefaultCostPrice = 3.00m, DefaultSalePrice = 14.99m,
                IsSerialized = false, IsUniversal = false, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId, CategoryId = fansCat.Id,
                Name = "Xbox Series X Replacement Cooling Fan",
                Kind = ProductKind.SparePart, SKU = "GC-FAN-XSX",
                DefaultCostPrice = 12.00m, DefaultSalePrice = 34.99m,
                IsSerialized = false, IsUniversal = false, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId, CategoryId = thermalCat.Id,
                Name = "Thermal Paste (2g Syringe — Universal)",
                Kind = ProductKind.SparePart, SKU = "GC-THRM-2G",
                DefaultCostPrice = 1.00m, DefaultSalePrice = 5.99m,
                IsSerialized = false, IsUniversal = true, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );
    }

    private static void SeedServiceCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // --- Console Hardware Issues ---
        var hardware = new ServiceCategory { CompanyId = companyId, Name = "Console Hardware Issues", SortOrder = 1, IsSubCategoryExist = true, IsActive = true };
        context.ServiceCategories.Add(hardware);

        var hdmiIssues      = new ServiceCategory { CompanyId = companyId, Name = "No Display / HDMI Issues",    ParentCategoryId = hardware.Id, SortOrder = 1, IsActive = true };
        var overheatIssues  = new ServiceCategory { CompanyId = companyId, Name = "Overheating & Fan Issues",    ParentCategoryId = hardware.Id, SortOrder = 2, IsActive = true };
        var discIssues      = new ServiceCategory { CompanyId = companyId, Name = "Disc Drive Issues",           ParentCategoryId = hardware.Id, SortOrder = 3, IsActive = true };
        var powerIssues     = new ServiceCategory { CompanyId = companyId, Name = "Power / Won't Turn On",       ParentCategoryId = hardware.Id, SortOrder = 4, IsActive = true };
        var usbIssues       = new ServiceCategory { CompanyId = companyId, Name = "USB Port Issues",             ParentCategoryId = hardware.Id, SortOrder = 5, IsActive = true };

        context.ServiceCategories.AddRange(hdmiIssues, overheatIssues, discIssues, powerIssues, usbIssues);

        // --- Controller Issues ---
        var controller = new ServiceCategory { CompanyId = companyId, Name = "Controller Issues", SortOrder = 2, IsSubCategoryExist = true, IsActive = true };
        context.ServiceCategories.Add(controller);

        var stickDrift      = new ServiceCategory { CompanyId = companyId, Name = "Stick Drift",                 ParentCategoryId = controller.Id, SortOrder = 1, IsActive = true };
        var triggerIssues   = new ServiceCategory { CompanyId = companyId, Name = "Trigger / Bumper Issues",     ParentCategoryId = controller.Id, SortOrder = 2, IsActive = true };
        var chargingIssues  = new ServiceCategory { CompanyId = companyId, Name = "Not Charging / Pairing",      ParentCategoryId = controller.Id, SortOrder = 3, IsActive = true };
        var buttonIssues    = new ServiceCategory { CompanyId = companyId, Name = "Button / D-Pad Issues",       ParentCategoryId = controller.Id, SortOrder = 4, IsActive = true };

        context.ServiceCategories.AddRange(stickDrift, triggerIssues, chargingIssues, buttonIssues);

        // --- Software & Storage ---
        var software = new ServiceCategory { CompanyId = companyId, Name = "Software & Storage", SortOrder = 3, IsSubCategoryExist = true, IsActive = true };
        context.ServiceCategories.Add(software);

        var softwareIssues  = new ServiceCategory { CompanyId = companyId, Name = "Error Codes & Crashes",       ParentCategoryId = software.Id, SortOrder = 1, IsActive = true };
        var storageIssues   = new ServiceCategory { CompanyId = companyId, Name = "SSD / HDD Upgrade",           ParentCategoryId = software.Id, SortOrder = 2, IsActive = true };

        context.ServiceCategories.AddRange(softwareIssues, storageIssues);

        // --- Sample services ---
        context.Services.AddRange(
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = stickDrift.Id,
                Name = "Controller Stick Drift Repair",
                Description = "Replace worn thumbstick modules causing unintended movement. Compatible with DualShock 4, DualSense, Xbox controllers.",
                EstimatedDuration = "30 mins", BasePrice = 29.99m,
                RequiresInventory = true, IsActive = true, IsUniversal = false
            },
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = hdmiIssues.Id,
                Name = "HDMI Port Replacement",
                Description = "Micro-solder replacement of damaged or bent HDMI port on PS4/PS5/Xbox consoles.",
                EstimatedDuration = "45 mins", BasePrice = 49.99m,
                RequiresInventory = true, IsActive = true, IsUniversal = false
            },
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = overheatIssues.Id,
                Name = "Console Thermal Service (Deep Clean + Repaste)",
                Description = "Full disassembly, dust removal, fan inspection, and thermal paste replacement to resolve overheating.",
                EstimatedDuration = "60 mins", BasePrice = 44.99m,
                RequiresInventory = true, IsActive = true, IsUniversal = true
            },
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = discIssues.Id,
                Name = "Disc Drive Replacement",
                Description = "Replace faulty optical drive that fails to read or eject discs.",
                EstimatedDuration = "60 mins", BasePrice = 89.99m,
                RequiresInventory = true, IsActive = true, IsUniversal = false
            }
        );
    }

    private static async Task SeedDeviceBrandsAndModelsAsync(Flowtap_Repair.DbContext.IRepairDbContext context, CancellationToken ct)
    {
        // Sony PlayStation
        var sony = await GetOrCreateBrandAsync(context, "Sony", ct);
        await AddModelsAsync(context, sony, new[]
        {
            "PlayStation 3", "PlayStation 3 Slim",
            "PlayStation 4", "PlayStation 4 Slim", "PlayStation 4 Pro",
            "PlayStation 5", "PlayStation 5 Digital Edition"
        }, ct);

        // Microsoft Xbox
        var microsoft = await GetOrCreateBrandAsync(context, "Microsoft", ct);
        await AddModelsAsync(context, microsoft, new[]
        {
            "Xbox 360", "Xbox One", "Xbox One S", "Xbox One X",
            "Xbox Series S", "Xbox Series X"
        }, ct);

        // Nintendo
        var nintendo = await GetOrCreateBrandAsync(context, "Nintendo", ct);
        await AddModelsAsync(context, nintendo, new[]
        {
            "Nintendo Switch", "Nintendo Switch Lite", "Nintendo Switch OLED",
            "Nintendo 3DS", "Nintendo 3DS XL"
        }, ct);

        // Other brands — no models
        await GetOrCreateBrandAsync(context, "Valve", ct);   // Steam Deck
        await GetOrCreateBrandAsync(context, "Sega", ct);
        await GetOrCreateBrandAsync(context, "Atari", ct);
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
