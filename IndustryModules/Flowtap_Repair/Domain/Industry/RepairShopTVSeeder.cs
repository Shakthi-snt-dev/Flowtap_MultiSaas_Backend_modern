using Flowtap_Repair.Domain.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Domain.Industry;

/// <summary>
/// Seeds data for TV and display repair shops:
/// LED/OLED TVs, monitors, projectors, etc.
/// </summary>
public static class RepairShopTVSeeder
{
    public static async Task SeedAsync(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedProductCategories(context, companyId);
        SeedServiceCategories(context, companyId);
        await SeedDeviceBrandsAndModelsAsync(context, ct);
    }

    private static void SeedProductCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // --- TV & Display Parts (root) ---
        var parts = new ProductCategory
        {
            CompanyId = companyId,
            Name = "TV & Display Parts",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsBrandExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(parts);

        var screenPanelsCat = new ProductCategory { CompanyId = companyId, Name = "Screen Panels (LED/OLED/QLED)", ParentCategoryId = parts.Id, SortOrder = 1, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var powerBoardsCat  = new ProductCategory { CompanyId = companyId, Name = "Power Boards & PSUs",           ParentCategoryId = parts.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var mainBoardsCat   = new ProductCategory { CompanyId = companyId, Name = "Main Boards & T-Con Boards",    ParentCategoryId = parts.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var backlightsCat   = new ProductCategory { CompanyId = companyId, Name = "Backlight Strips & Lamps",      ParentCategoryId = parts.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var hdmiPortsCat    = new ProductCategory { CompanyId = companyId, Name = "HDMI & AV Port Boards",         ParentCategoryId = parts.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        var invertorsCat    = new ProductCategory { CompanyId = companyId, Name = "Inverter Boards",               ParentCategoryId = parts.Id, SortOrder = 6, IsDirectProductExist = true, IsActive = true };
        var speakersCat     = new ProductCategory { CompanyId = companyId, Name = "Built-in Speakers",             ParentCategoryId = parts.Id, SortOrder = 7, IsDirectProductExist = true, IsActive = true };
        var remotesCat      = new ProductCategory { CompanyId = companyId, Name = "Remote Controls",               ParentCategoryId = parts.Id, SortOrder = 8, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(screenPanelsCat, powerBoardsCat, mainBoardsCat, backlightsCat, hdmiPortsCat, invertorsCat, speakersCat, remotesCat);

        // --- TV Accessories ---
        var accessories = new ProductCategory
        {
            CompanyId = companyId,
            Name = "TV & Display Accessories",
            SortOrder = 2,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(accessories);

        var standsCat       = new ProductCategory { CompanyId = companyId, Name = "TV Stands & Wall Mounts",  ParentCategoryId = accessories.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var cablesCat       = new ProductCategory { CompanyId = companyId, Name = "HDMI & AV Cables",         ParentCategoryId = accessories.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var screenCleanCat  = new ProductCategory { CompanyId = companyId, Name = "Screen Cleaning Kits",     ParentCategoryId = accessories.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(standsCat, cablesCat, screenCleanCat);

        // --- TV Services (as product categories for service linking) ---
        var services = new ProductCategory
        {
            CompanyId = companyId,
            Name = "TV & Display Services",
            SortOrder = 3,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(services);

        var screenRepCat  = new ProductCategory { CompanyId = companyId, Name = "Screen Replacement",     ParentCategoryId = services.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var backlightRepCat = new ProductCategory { CompanyId = companyId, Name = "Backlight Repair",     ParentCategoryId = services.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var boardRepCat   = new ProductCategory { CompanyId = companyId, Name = "Board-Level Repair",     ParentCategoryId = services.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var hdmiRepCat    = new ProductCategory { CompanyId = companyId, Name = "HDMI Port Repair",        ParentCategoryId = services.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var diagCat       = new ProductCategory { CompanyId = companyId, Name = "Diagnostics & Cleaning",  ParentCategoryId = services.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(screenRepCat, backlightRepCat, boardRepCat, hdmiRepCat, diagCat);

        // --- Sample products ---
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId, CategoryId = backlightsCat.Id,
                Name = "Universal LED Backlight Strip 32\" (Set of 4)",
                Kind = ProductKind.SparePart, SKU = "TV-BL-32-4S",
                DefaultCostPrice = 8.00m, DefaultSalePrice = 24.99m,
                IsSerialized = false, IsUniversal = true, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId, CategoryId = powerBoardsCat.Id,
                Name = "Samsung 55\" Power Supply Board BN44 Series",
                Kind = ProductKind.SparePart, SKU = "TV-PSB-SAM55",
                DefaultCostPrice = 22.00m, DefaultSalePrice = 64.99m,
                IsSerialized = false, IsUniversal = false, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId, CategoryId = remotesCat.Id,
                Name = "Universal TV Remote Control (All Brands)",
                Kind = ProductKind.Accessory, SKU = "TV-RMT-UNIV",
                DefaultCostPrice = 2.50m, DefaultSalePrice = 9.99m,
                IsSerialized = false, IsUniversal = true, IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );
    }

    private static void SeedServiceCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // --- Display Issues ---
        var displayIssues = new ServiceCategory { CompanyId = companyId, Name = "Display Issues", SortOrder = 1, IsSubCategoryExist = true, IsActive = true };
        context.ServiceCategories.Add(displayIssues);

        var noImageIssues   = new ServiceCategory { CompanyId = companyId, Name = "No Image / Black Screen",    ParentCategoryId = displayIssues.Id, SortOrder = 1, IsActive = true };
        var crackedScreen    = new ServiceCategory { CompanyId = companyId, Name = "Cracked / Damaged Screen",  ParentCategoryId = displayIssues.Id, SortOrder = 2, IsActive = true };
        var backlightIssues  = new ServiceCategory { CompanyId = companyId, Name = "Backlight / Dim Screen",    ParentCategoryId = displayIssues.Id, SortOrder = 3, IsActive = true };
        var lineIssues       = new ServiceCategory { CompanyId = companyId, Name = "Lines / Dead Pixels",       ParentCategoryId = displayIssues.Id, SortOrder = 4, IsActive = true };
        var colourIssues     = new ServiceCategory { CompanyId = companyId, Name = "Colour / Distortion Issues", ParentCategoryId = displayIssues.Id, SortOrder = 5, IsActive = true };

        context.ServiceCategories.AddRange(noImageIssues, crackedScreen, backlightIssues, lineIssues, colourIssues);

        // --- Power & Board Issues ---
        var boardIssues = new ServiceCategory { CompanyId = companyId, Name = "Power & Board Issues", SortOrder = 2, IsSubCategoryExist = true, IsActive = true };
        context.ServiceCategories.Add(boardIssues);

        var noPowerIssues    = new ServiceCategory { CompanyId = companyId, Name = "No Power / Dead TV",          ParentCategoryId = boardIssues.Id, SortOrder = 1, IsActive = true };
        var powerBoardFault  = new ServiceCategory { CompanyId = companyId, Name = "Power Board Failure",          ParentCategoryId = boardIssues.Id, SortOrder = 2, IsActive = true };
        var mainBoardFault   = new ServiceCategory { CompanyId = companyId, Name = "Main Board / Software Issues", ParentCategoryId = boardIssues.Id, SortOrder = 3, IsActive = true };
        var hdmiIssues       = new ServiceCategory { CompanyId = companyId, Name = "HDMI / Port Issues",           ParentCategoryId = boardIssues.Id, SortOrder = 4, IsActive = true };

        context.ServiceCategories.AddRange(noPowerIssues, powerBoardFault, mainBoardFault, hdmiIssues);

        // --- Audio Issues ---
        var audioIssues = new ServiceCategory { CompanyId = companyId, Name = "Audio Issues", SortOrder = 3, IsSubCategoryExist = true, IsActive = true };
        context.ServiceCategories.Add(audioIssues);

        var noSoundIssues   = new ServiceCategory { CompanyId = companyId, Name = "No Sound / Distorted Audio",  ParentCategoryId = audioIssues.Id, SortOrder = 1, IsActive = true };
        var speakerReplCat  = new ServiceCategory { CompanyId = companyId, Name = "Speaker Replacement",         ParentCategoryId = audioIssues.Id, SortOrder = 2, IsActive = true };

        context.ServiceCategories.AddRange(noSoundIssues, speakerReplCat);

        // --- Sample services ---
        context.Services.AddRange(
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = backlightIssues.Id,
                Name = "TV Backlight Strip Replacement",
                Description = "Replace burned-out LED backlight strips to restore full brightness. All screen sizes supported.",
                EstimatedDuration = "60 mins", BasePrice = 59.99m,
                RequiresInventory = true, IsActive = true, IsUniversal = true
            },
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = noPowerIssues.Id,
                Name = "TV Power Board Repair / Replacement",
                Description = "Diagnose and repair or replace faulty power supply board causing dead TV or standby issues.",
                EstimatedDuration = "45 mins", BasePrice = 69.99m,
                RequiresInventory = true, IsActive = true, IsUniversal = true
            },
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = hdmiIssues.Id,
                Name = "HDMI Port Repair",
                Description = "Micro-solder replacement of broken or loose HDMI port on TV main board.",
                EstimatedDuration = "40 mins", BasePrice = 44.99m,
                RequiresInventory = true, IsActive = true, IsUniversal = true
            },
            new Service
            {
                CompanyId = companyId, ServiceCategoryId = noImageIssues.Id,
                Name = "TV Diagnostics & Assessment",
                Description = "Full diagnostic check including board tests, panel inspection, and backlight test. Fee deducted from repair cost.",
                EstimatedDuration = "30 mins", BasePrice = 19.99m,
                RequiresInventory = false, IsActive = true, IsUniversal = true
            }
        );
    }

    private static async Task SeedDeviceBrandsAndModelsAsync(Flowtap_Repair.DbContext.IRepairDbContext context, CancellationToken ct)
    {
        // Samsung
        var samsung = await GetOrCreateBrandAsync(context, "Samsung", ct);
        await AddModelsAsync(context, samsung, new[]
        {
            "Samsung 32\" LED", "Samsung 43\" LED", "Samsung 55\" QLED",
            "Samsung 65\" QLED", "Samsung 75\" Neo QLED"
        }, ct);

        // LG
        var lg = await GetOrCreateBrandAsync(context, "LG", ct);
        await AddModelsAsync(context, lg, new[]
        {
            "LG 32\" LED", "LG 43\" LED", "LG 55\" OLED",
            "LG 65\" OLED evo", "LG 77\" OLED"
        }, ct);

        // Sony
        var sony = await GetOrCreateBrandAsync(context, "Sony", ct);
        await AddModelsAsync(context, sony, new[]
        {
            "Sony Bravia 43\" LED", "Sony Bravia 55\" OLED", "Sony Bravia 65\" 4K"
        }, ct);

        // TCL
        var tcl = await GetOrCreateBrandAsync(context, "TCL", ct);
        await AddModelsAsync(context, tcl, new[]
        {
            "TCL 32\" LED", "TCL 43\" 4K", "TCL 55\" QLED"
        }, ct);

        // Other brands — no models
        await GetOrCreateBrandAsync(context, "Hisense", ct);
        await GetOrCreateBrandAsync(context, "Philips", ct);
        await GetOrCreateBrandAsync(context, "Panasonic", ct);
        await GetOrCreateBrandAsync(context, "Toshiba", ct);
        await GetOrCreateBrandAsync(context, "Haier", ct);
        await GetOrCreateBrandAsync(context, "Skyworth", ct);
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
