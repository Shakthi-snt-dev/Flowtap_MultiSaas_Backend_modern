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
        // Idempotency: skip if already seeded
        var alreadySeeded = await context.ProductCategories
            .AnyAsync(c => c.CompanyId == companyId && c.Name == "Laptop Parts" && c.ParentCategoryId == null, ct);
        if (alreadySeeded) return;

        var (laptopPartsCat, screensCat, batteriesCat, keyboardsCat, storageCat, adaptersCat) =
            SeedProductCategoriesAndGetRefs(context, companyId);
        var (hardwareCat, softwareCat, displayCat, batteryIssueCat) =
            SeedServiceCategoriesAndGetRefs(context, companyId);
        await SeedDeviceBrandsAndModelsAsync(context, ct);
        SeedLaptopProducts(context, companyId, screensCat, batteriesCat, keyboardsCat, storageCat, adaptersCat);
        SeedLaptopServices(context, companyId, hardwareCat, softwareCat, displayCat, batteryIssueCat, laptopPartsCat);
        await SeedLaptopModelSpecificProductsAsync(context, companyId, ct);
    }

    // ── Ref-returning wrapper used by new SeedAsync ─────────────────────────────
    private static (
        ProductCategory laptopPartsCat,
        ProductCategory screensCat, ProductCategory batteriesCat,
        ProductCategory keyboardsCat, ProductCategory storageCat, ProductCategory adaptersCat
    ) SeedProductCategoriesAndGetRefs(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        SeedProductCategories(context, companyId);
        var laptopParts = context.ProductCategories.Local.First(c => c.CompanyId == companyId && c.Name == "Laptop Parts");
        var screens    = context.ProductCategories.Local.First(c => c.CompanyId == companyId && c.Name == "Screens & Display");
        var batteries  = context.ProductCategories.Local.First(c => c.CompanyId == companyId && c.Name == "Batteries");
        var keyboards  = context.ProductCategories.Local.First(c => c.CompanyId == companyId && c.Name == "Keyboards");
        var storage    = context.ProductCategories.Local.First(c => c.CompanyId == companyId && c.Name == "RAM & Storage");
        var adapters   = context.ProductCategories.Local.First(c => c.CompanyId == companyId && c.Name == "Adapters & Chargers");
        return (laptopParts, screens, batteries, keyboards, storage, adapters);
    }

    private static (
        ServiceCategory hardwareCat, ServiceCategory softwareCat,
        ServiceCategory displayCat, ServiceCategory batteryIssueCat
    ) SeedServiceCategoriesAndGetRefs(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        SeedServiceCategories(context, companyId);
        var hardware = context.ServiceCategories.Local.First(c => c.CompanyId == companyId && c.Name == "Hardware Issues");
        var software = context.ServiceCategories.Local.First(c => c.CompanyId == companyId && c.Name == "Software Issues");
        var display  = context.ServiceCategories.Local.First(c => c.CompanyId == companyId && c.Name == "Display Issues");
        var battery  = context.ServiceCategories.Local.First(c => c.CompanyId == companyId && c.Name == "Battery Problems");
        return (hardware, software, display, battery);
    }

    private static void SeedLaptopProducts(
        Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId,
        ProductCategory screensCat, ProductCategory batteriesCat,
        ProductCategory keyboardsCat, ProductCategory storageCat, ProductCategory adaptersCat)
    {
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = screensCat.Id,   Name = "15.6 inch FHD Laptop Screen",    Kind = ProductKind.SparePart, SKU = "LAP-SCR-156",  DefaultSalePrice = 3999m,  IsSerialized = false, IsUniversal = false, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = screensCat.Id,   Name = "14 inch HD Laptop Screen",       Kind = ProductKind.SparePart, SKU = "LAP-SCR-140",  DefaultSalePrice = 2999m,  IsSerialized = false, IsUniversal = false, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = batteriesCat.Id, Name = "Laptop Battery 4000mAh Universal", Kind = ProductKind.SparePart, SKU = "LAP-BAT-001", DefaultSalePrice = 1999m,  IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = keyboardsCat.Id, Name = "Laptop Keyboard Universal US",   Kind = ProductKind.SparePart, SKU = "LAP-KBD-001",  DefaultSalePrice = 1499m,  IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = adaptersCat.Id,  Name = "Laptop Charger 65W USB-C",       Kind = ProductKind.SparePart, SKU = "LAP-CHG-65W",  DefaultSalePrice = 1299m,  IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = storageCat.Id,   Name = "Laptop RAM 8GB DDR4",            Kind = ProductKind.SparePart, SKU = "LAP-RAM-8G",   DefaultSalePrice = 2499m,  IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = storageCat.Id,   Name = "256GB SSD M.2 NVMe",             Kind = ProductKind.SparePart, SKU = "LAP-SSD-256",  DefaultSalePrice = 2999m,  IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );
    }

    private static void SeedLaptopServices(
        Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId,
        ServiceCategory hardwareCat, ServiceCategory softwareCat,
        ServiceCategory displayCat, ServiceCategory batteryIssueCat,
        ProductCategory laptopPartsCat)
    {
        context.Services.AddRange(
            new Service { CompanyId = companyId, ServiceCategoryId = displayCat.Id,      ProductCategoryId = laptopPartsCat.Id, Name = "Laptop Screen Replacement",     BasePrice = 1999m, RequiresInventory = true,  IsUniversal = false, IsActive = true },
            new Service { CompanyId = companyId, ServiceCategoryId = hardwareCat.Id,     ProductCategoryId = laptopPartsCat.Id, Name = "Laptop Keyboard Replacement",   BasePrice = 999m,  RequiresInventory = true,  IsUniversal = false, IsActive = true },
            new Service { CompanyId = companyId, ServiceCategoryId = hardwareCat.Id,     ProductCategoryId = laptopPartsCat.Id, Name = "RAM Upgrade Service",           BasePrice = 499m,  RequiresInventory = true,  IsUniversal = false, IsActive = true },
            new Service { CompanyId = companyId, ServiceCategoryId = softwareCat.Id,     ProductCategoryId = laptopPartsCat.Id, Name = "SSD Upgrade & Data Migration",  BasePrice = 799m,  RequiresInventory = true,  IsUniversal = false, IsActive = true },
            new Service { CompanyId = companyId, ServiceCategoryId = batteryIssueCat.Id, ProductCategoryId = laptopPartsCat.Id, Name = "Laptop Battery Replacement",    BasePrice = 899m,  RequiresInventory = true,  IsUniversal = false, IsActive = true },
            new Service { CompanyId = companyId, ServiceCategoryId = hardwareCat.Id,     ProductCategoryId = laptopPartsCat.Id, Name = "Laptop Motherboard Repair",     BasePrice = 2999m, RequiresInventory = false, IsUniversal = false, IsActive = true },
            new Service { CompanyId = companyId, ServiceCategoryId = hardwareCat.Id,     ProductCategoryId = laptopPartsCat.Id, Name = "Water Damage Treatment (Laptop)", BasePrice = 1499m, RequiresInventory = false, IsUniversal = false, IsActive = true }
        );
    }

    private static void SeedProductCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // ─── Laptop Parts ───────────────────────────────────────────────────────
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

        var screensCat      = new ProductCategory { CompanyId = companyId, Name = "Screens & Display",  ParentCategoryId = laptopParts.Id, SortOrder = 1, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var batteriesCat    = new ProductCategory { CompanyId = companyId, Name = "Batteries",          ParentCategoryId = laptopParts.Id, SortOrder = 2, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var keyboardsCat    = new ProductCategory { CompanyId = companyId, Name = "Keyboards",          ParentCategoryId = laptopParts.Id, SortOrder = 3, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var hingesCat       = new ProductCategory { CompanyId = companyId, Name = "Hinges",             ParentCategoryId = laptopParts.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var motherboardsCat = new ProductCategory { CompanyId = companyId, Name = "Motherboards",       ParentCategoryId = laptopParts.Id, SortOrder = 5, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var fansCat         = new ProductCategory { CompanyId = companyId, Name = "Cooling Fans",       ParentCategoryId = laptopParts.Id, SortOrder = 6, IsDirectProductExist = true, IsActive = true };
        var storageCat      = new ProductCategory { CompanyId = companyId, Name = "RAM & Storage",      ParentCategoryId = laptopParts.Id, SortOrder = 7, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(screensCat, batteriesCat, keyboardsCat, hingesCat, motherboardsCat, fansCat, storageCat);

        // ─── Accessories ────────────────────────────────────────────────────────
        var accessories = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Accessories",
            SortOrder = 2,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(accessories);

        var bagsCat     = new ProductCategory { CompanyId = companyId, Name = "Laptop Bags",          ParentCategoryId = accessories.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var mouseCat    = new ProductCategory { CompanyId = companyId, Name = "Mouse & Keyboards",    ParentCategoryId = accessories.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var hubsCat     = new ProductCategory { CompanyId = companyId, Name = "USB Hubs",             ParentCategoryId = accessories.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var adaptersCat = new ProductCategory { CompanyId = companyId, Name = "Adapters & Chargers",  ParentCategoryId = accessories.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var padsCat     = new ProductCategory { CompanyId = companyId, Name = "Cooling Pads",         ParentCategoryId = accessories.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(bagsCat, mouseCat, hubsCat, adaptersCat, padsCat);

        // ─── Services ───────────────────────────────────────────────────────────
        var services = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Services",
            SortOrder = 3,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(services);

        var screenRep      = new ProductCategory { CompanyId = companyId, Name = "Screen Replacement",  ParentCategoryId = services.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var batteryRep     = new ProductCategory { CompanyId = companyId, Name = "Battery Replacement", ParentCategoryId = services.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var keyboardRep    = new ProductCategory { CompanyId = companyId, Name = "Keyboard Repair",     ParentCategoryId = services.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var motherboardRep = new ProductCategory { CompanyId = companyId, Name = "Motherboard Repair",  ParentCategoryId = services.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var osRep          = new ProductCategory { CompanyId = companyId, Name = "OS Installation",     ParentCategoryId = services.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        var dataRep        = new ProductCategory { CompanyId = companyId, Name = "Data Recovery",       ParentCategoryId = services.Id, SortOrder = 6, IsDirectProductExist = true, IsActive = true };
        var virusRep       = new ProductCategory { CompanyId = companyId, Name = "Virus Removal",       ParentCategoryId = services.Id, SortOrder = 7, IsDirectProductExist = true, IsActive = true };
        var ramRep         = new ProductCategory { CompanyId = companyId, Name = "RAM Upgrade",         ParentCategoryId = services.Id, SortOrder = 8, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(screenRep, batteryRep, keyboardRep, motherboardRep, osRep, dataRep, virusRep, ramRep);

        // ─── Seed Products — existing 3 (from original seeder inline) ────────────
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

        // ─── S8: Universal spare parts (IsUniversal=true) ────────────────────────
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = storageCat.Id,    Name = "Thermal Paste Arctic MX-4",           Kind = ProductKind.SparePart,  SKU = "LAP-THP-MX4",  DefaultSalePrice = 499m,   IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = storageCat.Id,    Name = "DDR4 RAM 4GB 2666MHz SODIMM",          Kind = ProductKind.SparePart,  SKU = "LAP-RAM-4G",   DefaultSalePrice = 1499m,  IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = storageCat.Id,    Name = "DDR4 RAM 16GB 3200MHz SODIMM",         Kind = ProductKind.SparePart,  SKU = "LAP-RAM-16G",  DefaultSalePrice = 3999m,  IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = storageCat.Id,    Name = "512GB SSD M.2 NVMe 2280",              Kind = ProductKind.SparePart,  SKU = "LAP-SSD-512",  DefaultSalePrice = 4999m,  IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = storageCat.Id,    Name = "1TB SSD M.2 NVMe 2280",                Kind = ProductKind.SparePart,  SKU = "LAP-SSD-1TB",  DefaultSalePrice = 7999m,  IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = storageCat.Id,    Name = "Laptop Wi-Fi Card Intel AX201",        Kind = ProductKind.SparePart,  SKU = "LAP-WFC-AX2",  DefaultSalePrice = 799m,   IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = storageCat.Id,    Name = "Anti-Static Brush Set",                Kind = ProductKind.Accessory,  SKU = "LAP-BRS-001",  DefaultSalePrice = 199m,   IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = fansCat.Id,       Name = "Laptop Cooling Fan Universal 5V",      Kind = ProductKind.SparePart,  SKU = "LAP-FAN-5V",   DefaultSalePrice = 799m,   IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = hingesCat.Id,     Name = "Laptop Hinge Set Universal",           Kind = ProductKind.SparePart,  SKU = "LAP-HNG-001",  DefaultSalePrice = 599m,   IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = hingesCat.Id,     Name = "Laptop DC Jack Power Connector",       Kind = ProductKind.SparePart,  SKU = "LAP-DCJ-001",  DefaultSalePrice = 299m,   IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = keyboardsCat.Id,  Name = "Laptop Keyboard US Layout Backlit",    Kind = ProductKind.SparePart,  SKU = "LAP-KBL-US",   DefaultSalePrice = 1999m,  IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = batteriesCat.Id,  Name = "CMOS Battery CR2032",                  Kind = ProductKind.SparePart,  SKU = "LAP-CMO-001",  DefaultSalePrice = 99m,    IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = adaptersCat.Id,   Name = "Laptop Screen Cleaning Kit",           Kind = ProductKind.Accessory,  SKU = "LAP-CLN-001",  DefaultSalePrice = 299m,   IsSerialized = false, IsUniversal = true,  IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );

        // ─── Screen size varieties (IsUniversal=false, no brand) ─────────────────
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = screensCat.Id, Name = "11.6inch HD Screen 1366x768",         Kind = ProductKind.SparePart, SKU = "LAP-SCR-116",  DefaultSalePrice = 2499m,  IsSerialized = false, IsUniversal = false, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = screensCat.Id, Name = "13.3inch FHD IPS Screen",             Kind = ProductKind.SparePart, SKU = "LAP-SCR-133",  DefaultSalePrice = 3499m,  IsSerialized = false, IsUniversal = false, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = screensCat.Id, Name = "17.3inch FHD Laptop Screen",          Kind = ProductKind.SparePart, SKU = "LAP-SCR-173",  DefaultSalePrice = 4999m,  IsSerialized = false, IsUniversal = false, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = screensCat.Id, Name = "MacBook 13.3inch Retina Screen",      Kind = ProductKind.SparePart, SKU = "LAP-SCR-MBR",  DefaultSalePrice = 8999m,  IsSerialized = false, IsUniversal = false, IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );

        // ─── Brand-grade batteries (IsUniversal=false, no model mapping) ─────────
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = batteriesCat.Id, Name = "Dell Laptop Battery 65Wh",          Kind = ProductKind.SparePart, SKU = "LAP-BAT-DEL",  DefaultSalePrice = 2499m,  IsSerialized = false, IsUniversal = false, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = batteriesCat.Id, Name = "HP Laptop Battery 45Wh",            Kind = ProductKind.SparePart, SKU = "LAP-BAT-HP45", DefaultSalePrice = 1999m,  IsSerialized = false, IsUniversal = false, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = batteriesCat.Id, Name = "Lenovo ThinkPad Battery 56Wh",      Kind = ProductKind.SparePart, SKU = "LAP-BAT-LEN",  DefaultSalePrice = 2999m,  IsSerialized = false, IsUniversal = false, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = batteriesCat.Id, Name = "MacBook Pro Battery 73Wh",          Kind = ProductKind.SparePart, SKU = "LAP-BAT-MBP",  DefaultSalePrice = 5999m,  IsSerialized = false, IsUniversal = false, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = batteriesCat.Id, Name = "MacBook Air Battery 50Wh M1 M2",    Kind = ProductKind.SparePart, SKU = "LAP-BAT-MBA",  DefaultSalePrice = 4999m,  IsSerialized = false, IsUniversal = false, IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );

        // ─── S9: Laptop accessories (IsUniversal=true) ───────────────────────────
        context.Products.AddRange(
            // Bags
            new Product { CompanyId = companyId, CategoryId = bagsCat.Id,    Name = "Laptop Sleeve 15.6inch Neoprene",     Kind = ProductKind.Accessory, SKU = "LAP-SLV-156",  DefaultSalePrice = 799m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = bagsCat.Id,    Name = "Laptop Backpack 17inch Premium",       Kind = ProductKind.Accessory, SKU = "LAP-BAG-17P",  DefaultSalePrice = 1999m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            // Mouse & Keyboards
            new Product { CompanyId = companyId, CategoryId = mouseCat.Id,   Name = "Wireless Mouse Bluetooth",             Kind = ProductKind.Accessory, SKU = "LAP-MWB-001",  DefaultSalePrice = 699m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = mouseCat.Id,   Name = "USB Mechanical Keyboard Compact",      Kind = ProductKind.Accessory, SKU = "LAP-KBE-001",  DefaultSalePrice = 1499m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            // USB Hubs
            new Product { CompanyId = companyId, CategoryId = hubsCat.Id,    Name = "USB-C Hub 7-in-1 Multiport",           Kind = ProductKind.Accessory, SKU = "LAP-HUB-7N1",  DefaultSalePrice = 1499m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = hubsCat.Id,    Name = "USB 3.0 Hub 4-Port",                   Kind = ProductKind.Accessory, SKU = "LAP-HUB-4P3",  DefaultSalePrice = 699m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            // Adapters & Chargers
            new Product { CompanyId = companyId, CategoryId = adaptersCat.Id,Name = "45W USB-C GaN Charger",                Kind = ProductKind.Accessory, SKU = "LAP-CHG-45G",  DefaultSalePrice = 999m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = adaptersCat.Id,Name = "100W USB-C Power Delivery Charger",    Kind = ProductKind.Accessory, SKU = "LAP-CHG-100",  DefaultSalePrice = 1999m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = adaptersCat.Id,Name = "HDMI to VGA Adapter",                  Kind = ProductKind.Accessory, SKU = "LAP-ADP-HVG",  DefaultSalePrice = 399m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            // Cooling Pads
            new Product { CompanyId = companyId, CategoryId = padsCat.Id,    Name = "Laptop Cooling Pad with 2 Fans",       Kind = ProductKind.Accessory, SKU = "LAP-CDP-2FN",  DefaultSalePrice = 999m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = padsCat.Id,    Name = "Wrist Rest Pad Memory Foam",           Kind = ProductKind.Accessory, SKU = "LAP-WRP-001",  DefaultSalePrice = 499m,   IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );
    }

    private static void SeedServiceCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // ─── Hardware Issues ─────────────────────────────────────────────────────
        var hardware = new ServiceCategory
        {
            CompanyId = companyId,
            Name = "Hardware Issues",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ServiceCategories.Add(hardware);

        var displayIssues  = new ServiceCategory { CompanyId = companyId, Name = "Display Issues",    ParentCategoryId = hardware.Id, SortOrder = 1, IsActive = true };
        var batteryIssues  = new ServiceCategory { CompanyId = companyId, Name = "Battery Problems",  ParentCategoryId = hardware.Id, SortOrder = 2, IsActive = true };
        var keyboardIssues = new ServiceCategory { CompanyId = companyId, Name = "Keyboard Issues",   ParentCategoryId = hardware.Id, SortOrder = 3, IsActive = true };
        var overheatIssues = new ServiceCategory { CompanyId = companyId, Name = "Overheating",       ParentCategoryId = hardware.Id, SortOrder = 4, IsActive = true };
        var bootIssues     = new ServiceCategory { CompanyId = companyId, Name = "Boot Issues",       ParentCategoryId = hardware.Id, SortOrder = 5, IsActive = true };

        context.ServiceCategories.AddRange(displayIssues, batteryIssues, keyboardIssues, overheatIssues, bootIssues);

        // ─── Software Issues ─────────────────────────────────────────────────────
        var software = new ServiceCategory
        {
            CompanyId = companyId,
            Name = "Software Issues",
            SortOrder = 2,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ServiceCategories.Add(software);

        var osIssues   = new ServiceCategory { CompanyId = companyId, Name = "OS Install/Reinstall", ParentCategoryId = software.Id, SortOrder = 1, IsActive = true };
        var dataIssues = new ServiceCategory { CompanyId = companyId, Name = "Data Recovery",        ParentCategoryId = software.Id, SortOrder = 2, IsActive = true };
        var virusIssues= new ServiceCategory { CompanyId = companyId, Name = "Virus Removal",        ParentCategoryId = software.Id, SortOrder = 3, IsActive = true };
        var softIssues = new ServiceCategory { CompanyId = companyId, Name = "Software Errors",      ParentCategoryId = software.Id, SortOrder = 4, IsActive = true };

        context.ServiceCategories.AddRange(osIssues, dataIssues, virusIssues, softIssues);

        // ─── Seed Laptop Services (inline) ───────────────────────────────────────
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

    // ─── S10: Model-specific products with ProductDeviceModelMapping ────────────
    private static async Task SeedLaptopModelSpecificProductsAsync(
        Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId, CancellationToken ct)
    {
        // Get category refs from EF local cache (added during SeedProductCategories)
        var screensCat   = context.ProductCategories.Local.First(c => c.CompanyId == companyId && c.Name == "Screens & Display");
        var batteriesCat = context.ProductCategories.Local.First(c => c.CompanyId == companyId && c.Name == "Batteries");
        var keyboardsCat = context.ProductCategories.Local.First(c => c.CompanyId == companyId && c.Name == "Keyboards");

        // Get model refs from EF local cache (added during SeedDeviceBrandsAndModelsAsync)
        DeviceModel? GetModel(string name) =>
            context.DeviceModels.Local.FirstOrDefault(m => m.Name == name);

        var dellXps13  = GetModel("XPS 13");
        var dellInsp15 = GetModel("Inspiron 15");
        var hpElite840 = GetModel("EliteBook 840");
        var lenovoX1   = GetModel("ThinkPad X1");
        var macAirM2   = GetModel("MacBook Air M2");
        var macPro14   = GetModel("MacBook Pro 14");

        // Helper: create product + mapping in one call (null model = skip)
        void AddMapped(ProductCategory cat, string name, string sku, decimal price, DeviceModel? model)
        {
            if (model == null) return;
            var p = new Product
            {
                CompanyId = companyId,
                CategoryId = cat.Id,
                Name = name,
                Kind = ProductKind.SparePart,
                SKU = sku,
                DefaultSalePrice = price,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            };
            context.Products.Add(p);
            context.ProductDeviceModelMappings.Add(
                new ProductDeviceModelMapping { ProductId = p.Id, DeviceModelId = model.Id });
        }

        // Dell XPS 13
        AddMapped(screensCat,   "Dell XPS 13 13.4inch FHD+ Screen",      "LAP-SCR-DXP13",  6999m,  dellXps13);
        AddMapped(batteriesCat, "Dell XPS 13 Battery 52Wh",               "LAP-BAT-DXP13",  3499m,  dellXps13);
        // Dell Inspiron 15
        AddMapped(keyboardsCat, "Dell Inspiron 15 Keyboard US",            "LAP-KBD-DIN15",  1799m,  dellInsp15);
        // HP EliteBook 840
        AddMapped(screensCat,   "HP EliteBook 840 14inch FHD Screen",      "LAP-SCR-HPE840", 5499m,  hpElite840);
        AddMapped(keyboardsCat, "HP EliteBook 840 Keyboard",               "LAP-KBD-HPE840", 1999m,  hpElite840);
        // Lenovo ThinkPad X1
        AddMapped(screensCat,   "Lenovo ThinkPad X1 14inch 2K Screen",     "LAP-SCR-LTX1",   7499m,  lenovoX1);
        AddMapped(batteriesCat, "Lenovo ThinkPad X1 Battery 57Wh",         "LAP-BAT-LTX1",   3999m,  lenovoX1);
        // MacBook Air M2
        AddMapped(screensCat,   "MacBook Air M2 13.6inch Liquid Retina",   "LAP-SCR-MBAM2",  12999m, macAirM2);
        // MacBook Pro 14
        AddMapped(screensCat,   "MacBook Pro 14inch MiniLED Screen",       "LAP-SCR-MBP14",  16999m, macPro14);
        AddMapped(batteriesCat, "MacBook Pro 14 Battery 70Wh",             "LAP-BAT-MBP14",  6999m,  macPro14);

        await Task.CompletedTask; // keeps method async for consistent call site
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
