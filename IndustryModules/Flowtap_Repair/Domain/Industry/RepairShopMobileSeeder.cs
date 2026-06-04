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
        // Idempotency: check for root category by name and companyId
        var alreadySeeded = await context.ProductCategories
            .AnyAsync(c => c.CompanyId == companyId && c.Name == "Mobile Devices" && c.ParentCategoryId == null, ct);
        if (alreadySeeded) return;

        // ── 1. Product Categories (mobile + accessories + tablets only) ─────────
        var (
            mobileDevices,
            androidPhonesCat, iPhonesCat,
            casesCoversCat, cablesChargersCat, screenProtectorsCat,
            androidTabletsCat, iPadsCat
        ) = SeedProductCategories(context, companyId);

        // ── 2. Service Categories ────────────────────────────────────────────────
        var (
            screenRepairCat, batteryServiceCat, hardwareRepairCat,
            softwareServicesCat, waterDamageCat
        ) = SeedServiceCategories(context, companyId);

        // ── 3. Products ──────────────────────────────────────────────────────────
        SeedProducts(
            context, companyId,
            mobileDevices,
            androidPhonesCat, iPhonesCat,
            casesCoversCat, cablesChargersCat, screenProtectorsCat,
            androidTabletsCat, iPadsCat,
            out var samsungS23Screen, out var samsungS23Battery, out var samsungS23Cover,
            out var iphone14Screen, out var iphone14Battery, out var iphone14Cover,
            out var samsungS22Screen, out var samsungS22Battery,
            out var samsungA54Screen, out var samsungA54Battery,
            out var iphone13Screen,   out var iphone13Battery,
            out var iphone15Screen,   out var iphone15Battery);

        // ── 4. Device Brands & Models (mobile brands only) ───────────────────────
        var (samsung, apple, xiaomi, onePlus) =
            SeedDeviceBrands(context, androidPhonesCat, iPhonesCat);

        var (
            galaxyS23, galaxyS24, galaxyS22, galaxyA54, galaxyA34,
            iphone15Pro, iphone15, iphone14Pro, iphone14Model, iphone13
        ) = SeedDeviceModels(context, samsung, apple, xiaomi, onePlus,
                             androidPhonesCat, iPhonesCat);

        // ── 5. Product → Device Model Mappings ──────────────────────────────────
        SeedProductDeviceModelMappings(
            context,
            samsungS23Screen, samsungS23Battery, samsungS23Cover, galaxyS23,
            iphone14Screen, iphone14Battery, iphone14Cover, iphone14Model,
            samsungS22Screen, samsungS22Battery, galaxyS22,
            samsungA54Screen, samsungA54Battery, galaxyA54,
            iphone13Screen,   iphone13Battery,   iphone13,
            iphone15Screen,   iphone15Battery,   iphone15);

        // ── 6. Services (mobile only) ────────────────────────────────────────────
        SeedServices(
            context, companyId,
            screenRepairCat, batteryServiceCat, hardwareRepairCat,
            softwareServicesCat, waterDamageCat,
            androidPhonesCat, iPhonesCat,
            iphone14Model, galaxyS23);
    }

    // ────────────────────────────────────────────────────────────────────────────
    // Product Categories
    // ────────────────────────────────────────────────────────────────────────────
    private static (
        ProductCategory mobileDevices,
        ProductCategory androidPhonesCat, ProductCategory iPhonesCat,
        ProductCategory casesCoversCat, ProductCategory cablesChargersCat, ProductCategory screenProtectorsCat,
        ProductCategory androidTabletsCat, ProductCategory iPadsCat
    ) SeedProductCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // ── Mobile Devices (parent) ──
        var mobileDevices = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Mobile Devices",
            SortOrder = 1,
            IsBrandExist = true,
            IsSubCategoryExist = true,
            IsDirectProductExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(mobileDevices);

        var androidPhonesCat = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Android Phones",
            ParentCategoryId = mobileDevices.Id,
            SortOrder = 1,
            IsDirectProductExist = true,
            IsBrandExist = true,
            IsActive = true
        };
        var iPhonesCat = new ProductCategory
        {
            CompanyId = companyId,
            Name = "iPhones",
            ParentCategoryId = mobileDevices.Id,
            SortOrder = 2,
            IsDirectProductExist = true,
            IsBrandExist = true,
            IsActive = true
        };
        context.ProductCategories.AddRange(androidPhonesCat, iPhonesCat);

        // ── Phone Accessories (parent) ──
        var phoneAccessories = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Phone Accessories",
            SortOrder = 4,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(phoneAccessories);

        var casesCoversCat = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Cases & Covers",
            ParentCategoryId = phoneAccessories.Id,
            SortOrder = 1,
            IsDirectProductExist = true,
            IsActive = true
        };
        var cablesChargersCat = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Cables & Chargers",
            ParentCategoryId = phoneAccessories.Id,
            SortOrder = 2,
            IsDirectProductExist = true,
            IsActive = true
        };
        var screenProtectorsCat = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Screen Protectors",
            ParentCategoryId = phoneAccessories.Id,
            SortOrder = 3,
            IsDirectProductExist = true,
            IsActive = true
        };
        context.ProductCategories.AddRange(casesCoversCat, cablesChargersCat, screenProtectorsCat);

        // ── Tablets (parent) ──
        var tablets = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Tablets",
            SortOrder = 5,
            IsBrandExist = true,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(tablets);

        var androidTabletsCat = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Android Tablets",
            ParentCategoryId = tablets.Id,
            SortOrder = 1,
            IsDirectProductExist = true,
            IsBrandExist = true,
            IsActive = true
        };
        var iPadsCat = new ProductCategory
        {
            CompanyId = companyId,
            Name = "iPads",
            ParentCategoryId = tablets.Id,
            SortOrder = 2,
            IsDirectProductExist = true,
            IsActive = true
        };
        context.ProductCategories.AddRange(androidTabletsCat, iPadsCat);

        return (
            mobileDevices,
            androidPhonesCat, iPhonesCat,
            casesCoversCat, cablesChargersCat, screenProtectorsCat,
            androidTabletsCat, iPadsCat
        );
    }

    // ────────────────────────────────────────────────────────────────────────────
    // Service Categories
    // ────────────────────────────────────────────────────────────────────────────
    private static (
        ServiceCategory screenRepair, ServiceCategory batteryService,
        ServiceCategory hardwareRepair, ServiceCategory softwareServices,
        ServiceCategory waterDamage
    ) SeedServiceCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        var screenRepair = new ServiceCategory
        {
            CompanyId = companyId,
            Name = "Screen Repair",
            SortOrder = 1,
            IsActive = true
        };
        var batteryService = new ServiceCategory
        {
            CompanyId = companyId,
            Name = "Battery Service",
            SortOrder = 2,
            IsActive = true
        };
        var hardwareRepair = new ServiceCategory
        {
            CompanyId = companyId,
            Name = "Hardware Repair",
            SortOrder = 3,
            IsActive = true
        };
        var softwareServices = new ServiceCategory
        {
            CompanyId = companyId,
            Name = "Software Services",
            SortOrder = 4,
            IsActive = true
        };
        var waterDamage = new ServiceCategory
        {
            CompanyId = companyId,
            Name = "Water Damage",
            SortOrder = 5,
            IsActive = true
        };
        context.ServiceCategories.AddRange(
            screenRepair, batteryService, hardwareRepair,
            softwareServices, waterDamage);

        return (screenRepair, batteryService, hardwareRepair, softwareServices, waterDamage);
    }

    // ────────────────────────────────────────────────────────────────────────────
    // Products
    // ────────────────────────────────────────────────────────────────────────────
    private static void SeedProducts(
        Flowtap_Repair.DbContext.IRepairDbContext context,
        Guid companyId,
        ProductCategory mobileDevices,
        ProductCategory androidPhonesCat,
        ProductCategory iPhonesCat,
        ProductCategory casesCoversCat,
        ProductCategory cablesChargersCat,
        ProductCategory screenProtectorsCat,
        ProductCategory androidTabletsCat,
        ProductCategory iPadsCat,
        // ── Existing out params ──────────────────────────────────────────────────
        out Product samsungS23Screen,
        out Product samsungS23Battery,
        out Product samsungS23Cover,
        out Product iphone14Screen,
        out Product iphone14Battery,
        out Product iphone14Cover,
        // ── New model-specific out params ────────────────────────────────────────
        out Product samsungS22Screen,
        out Product samsungS22Battery,
        out Product samsungA54Screen,
        out Product samsungA54Battery,
        out Product iphone13Screen,
        out Product iphone13Battery,
        out Product iphone15Screen,
        out Product iphone15Battery)
    {
        // ── S1: Universal products (IsUniversal=true — appear for any device) ────
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId,
                CategoryId = screenProtectorsCat.Id,
                Name = "Screen Cleaning Kit",
                Kind = ProductKind.Accessory,
                SKU = "UNI-SCK-001",
                DefaultSalePrice = 199m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = cablesChargersCat.Id,
                Name = "Generic USB-C Cable 1m",
                Kind = ProductKind.Accessory,
                SKU = "UNI-USBC-001",
                DefaultSalePrice = 149m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = screenProtectorsCat.Id,
                Name = "Tempered Glass Protector Universal",
                Kind = ProductKind.Accessory,
                SKU = "UNI-TGP-001",
                DefaultSalePrice = 99m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            // Additional universal tools/adhesives
            new Product
            {
                CompanyId = companyId,
                CategoryId = screenProtectorsCat.Id,
                Name = "UV Curing Glue 5ml",
                Kind = ProductKind.SparePart,
                SKU = "UNI-UV-GLUE",
                DefaultSalePrice = 299m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = casesCoversCat.Id,
                Name = "B-7000 Adhesive Glue 25ml",
                Kind = ProductKind.SparePart,
                SKU = "UNI-B7000",
                DefaultSalePrice = 199m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = casesCoversCat.Id,
                Name = "Phone Opening Tool Kit 13pc",
                Kind = ProductKind.Accessory,
                SKU = "UNI-OPT-013",
                DefaultSalePrice = 399m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = cablesChargersCat.Id,
                Name = "Anti-Static Wrist Strap",
                Kind = ProductKind.Accessory,
                SKU = "UNI-AWS-001",
                DefaultSalePrice = 149m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = cablesChargersCat.Id,
                Name = "Wireless Charger Pad 15W",
                Kind = ProductKind.Accessory,
                SKU = "UNI-WCP-15W",
                DefaultSalePrice = 699m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = cablesChargersCat.Id,
                Name = "Micro USB Cable 1m",
                Kind = ProductKind.Accessory,
                SKU = "UNI-MUSB-001",
                DefaultSalePrice = 149m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = androidPhonesCat.Id,
                Name = "Thermal Paste 1g Silver",
                Kind = ProductKind.SparePart,
                SKU = "UNI-THP-001",
                DefaultSalePrice = 149m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );

        // ── S2: Mobile Devices parent-level products (visible for ALL mobile) ────
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId,
                CategoryId = mobileDevices.Id,
                Name = "Universal Phone Stand Holder",
                Kind = ProductKind.Accessory,
                SKU = "MOB-STD-001",
                DefaultSalePrice = 299m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = mobileDevices.Id,
                Name = "Waterproof Phone Pouch",
                Kind = ProductKind.Accessory,
                SKU = "MOB-WPP-001",
                DefaultSalePrice = 249m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = mobileDevices.Id,
                Name = "Phone Pop Socket Grip",
                Kind = ProductKind.Accessory,
                SKU = "MOB-PSK-001",
                DefaultSalePrice = 199m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = mobileDevices.Id,
                Name = "Magnetic Car Phone Mount",
                Kind = ProductKind.Accessory,
                SKU = "MOB-CPM-001",
                DefaultSalePrice = 499m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = mobileDevices.Id,
                Name = "OTG Adapter USB-C",
                Kind = ProductKind.Accessory,
                SKU = "MOB-OTG-001",
                DefaultSalePrice = 199m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = mobileDevices.Id,
                Name = "Screen Cleaning Spray 50ml",
                Kind = ProductKind.Accessory,
                SKU = "MOB-SCS-001",
                DefaultSalePrice = 199m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            // Legacy accessories under mobile (kept for compatibility)
            new Product
            {
                CompanyId = companyId,
                CategoryId = casesCoversCat.Id,
                Name = "Universal Phone Case 6.5 inch",
                Kind = ProductKind.Accessory,
                SKU = "MOB-CASE-001",
                DefaultSalePrice = 299m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = screenProtectorsCat.Id,
                Name = "Universal Screen Protector Mobile",
                Kind = ProductKind.Accessory,
                SKU = "MOB-SP-001",
                DefaultSalePrice = 99m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );

        // ── S3: Android Phones sub-category products ─────────────────────────────
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId,
                CategoryId = androidPhonesCat.Id,
                Name = "Android Type-C Fast Charger 33W",
                Kind = ProductKind.Accessory,
                SKU = "AND-CHG-001",
                DefaultSalePrice = 499m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = androidPhonesCat.Id,
                Name = "Android USB-C Data Cable",
                Kind = ProductKind.Accessory,
                SKU = "AND-CBL-001",
                DefaultSalePrice = 199m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = androidPhonesCat.Id,
                Name = "Android Earphones Type-C",
                Kind = ProductKind.Accessory,
                SKU = "AND-EAR-001",
                DefaultSalePrice = 499m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = androidPhonesCat.Id,
                Name = "Android Phone Case Transparent",
                Kind = ProductKind.Accessory,
                SKU = "AND-CST-001",
                DefaultSalePrice = 199m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = androidPhonesCat.Id,
                Name = "USB-C Charging Port (Generic)",
                Kind = ProductKind.SparePart,
                SKU = "AND-CHP-001",
                DefaultSalePrice = 299m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = androidPhonesCat.Id,
                Name = "Android Tempered Glass 6.5inch",
                Kind = ProductKind.Accessory,
                SKU = "AND-TGS-001",
                DefaultSalePrice = 149m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = androidPhonesCat.Id,
                Name = "Android Fast Charge Cable 2m",
                Kind = ProductKind.Accessory,
                SKU = "AND-FCC-2M",
                DefaultSalePrice = 299m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );

        // ── S4: iPhone sub-category products ─────────────────────────────────────
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId,
                CategoryId = iPhonesCat.Id,
                Name = "iPhone Lightning Cable 1m",
                Kind = ProductKind.Accessory,
                SKU = "APL-LCB-1M",
                DefaultSalePrice = 499m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = iPhonesCat.Id,
                Name = "iPhone 20W USB-C Charger",
                Kind = ProductKind.Accessory,
                SKU = "APL-CHG-20W",
                DefaultSalePrice = 799m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = iPhonesCat.Id,
                Name = "iPhone Tempered Glass Universal",
                Kind = ProductKind.Accessory,
                SKU = "APL-TGU-001",
                DefaultSalePrice = 199m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = iPhonesCat.Id,
                Name = "iPhone Privacy Screen Protector",
                Kind = ProductKind.Accessory,
                SKU = "APL-PSP-001",
                DefaultSalePrice = 299m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = iPhonesCat.Id,
                Name = "iPhone Lightning Port Flex Cable",
                Kind = ProductKind.SparePart,
                SKU = "APL-LPF-001",
                DefaultSalePrice = 599m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );

        // ── S5: Samsung Galaxy S23 specific products (existing) ──────────────────
        samsungS23Screen = new Product
        {
            CompanyId = companyId,
            CategoryId = androidPhonesCat.Id,
            Name = "Samsung Galaxy S23 AMOLED Screen",
            Kind = ProductKind.SparePart,
            SKU = "SAM-S23-SCR",
            DefaultSalePrice = 4999m,
            IsSerialized = false,
            IsUniversal = false,
            IsActive = true,
            PublishStatus = ProductPublishStatus.Published
        };
        samsungS23Battery = new Product
        {
            CompanyId = companyId,
            CategoryId = androidPhonesCat.Id,
            Name = "Samsung Galaxy S23 Battery 3900mAh",
            Kind = ProductKind.SparePart,
            SKU = "SAM-S23-BAT",
            DefaultSalePrice = 1499m,
            IsSerialized = false,
            IsUniversal = false,
            IsActive = true,
            PublishStatus = ProductPublishStatus.Published
        };
        samsungS23Cover = new Product
        {
            CompanyId = companyId,
            CategoryId = androidPhonesCat.Id,
            Name = "Samsung Galaxy S23 Back Cover",
            Kind = ProductKind.SparePart,
            SKU = "SAM-S23-CVR",
            DefaultSalePrice = 799m,
            IsSerialized = false,
            IsUniversal = false,
            IsActive = true,
            PublishStatus = ProductPublishStatus.Published
        };
        context.Products.AddRange(samsungS23Screen, samsungS23Battery, samsungS23Cover);

        // ── S5: iPhone 14 specific products (existing) ───────────────────────────
        iphone14Screen = new Product
        {
            CompanyId = companyId,
            CategoryId = iPhonesCat.Id,
            Name = "iPhone 14 OLED Display",
            Kind = ProductKind.SparePart,
            SKU = "APL-IP14-SCR",
            DefaultSalePrice = 8999m,
            IsSerialized = false,
            IsUniversal = false,
            IsActive = true,
            PublishStatus = ProductPublishStatus.Published
        };
        iphone14Battery = new Product
        {
            CompanyId = companyId,
            CategoryId = iPhonesCat.Id,
            Name = "iPhone 14 Battery 3279mAh",
            Kind = ProductKind.SparePart,
            SKU = "APL-IP14-BAT",
            DefaultSalePrice = 2499m,
            IsSerialized = false,
            IsUniversal = false,
            IsActive = true,
            PublishStatus = ProductPublishStatus.Published
        };
        iphone14Cover = new Product
        {
            CompanyId = companyId,
            CategoryId = iPhonesCat.Id,
            Name = "iPhone 14 Back Glass",
            Kind = ProductKind.SparePart,
            SKU = "APL-IP14-CVR",
            DefaultSalePrice = 1499m,
            IsSerialized = false,
            IsUniversal = false,
            IsActive = true,
            PublishStatus = ProductPublishStatus.Published
        };
        context.Products.AddRange(iphone14Screen, iphone14Battery, iphone14Cover);

        // ── S5: Samsung Galaxy S22 specific products (new) ───────────────────────
        samsungS22Screen = new Product
        {
            CompanyId = companyId,
            CategoryId = androidPhonesCat.Id,
            Name = "Samsung Galaxy S22 AMOLED Screen",
            Kind = ProductKind.SparePart,
            SKU = "SAM-S22-SCR",
            DefaultSalePrice = 3999m,
            IsSerialized = false,
            IsUniversal = false,
            IsActive = true,
            PublishStatus = ProductPublishStatus.Published
        };
        samsungS22Battery = new Product
        {
            CompanyId = companyId,
            CategoryId = androidPhonesCat.Id,
            Name = "Samsung Galaxy S22 Battery 3700mAh",
            Kind = ProductKind.SparePart,
            SKU = "SAM-S22-BAT",
            DefaultSalePrice = 1299m,
            IsSerialized = false,
            IsUniversal = false,
            IsActive = true,
            PublishStatus = ProductPublishStatus.Published
        };
        context.Products.AddRange(samsungS22Screen, samsungS22Battery);

        // ── S5: Samsung Galaxy A54 specific products (new) ───────────────────────
        samsungA54Screen = new Product
        {
            CompanyId = companyId,
            CategoryId = androidPhonesCat.Id,
            Name = "Samsung Galaxy A54 Super AMOLED Screen",
            Kind = ProductKind.SparePart,
            SKU = "SAM-A54-SCR",
            DefaultSalePrice = 2999m,
            IsSerialized = false,
            IsUniversal = false,
            IsActive = true,
            PublishStatus = ProductPublishStatus.Published
        };
        samsungA54Battery = new Product
        {
            CompanyId = companyId,
            CategoryId = androidPhonesCat.Id,
            Name = "Samsung Galaxy A54 Battery 5000mAh",
            Kind = ProductKind.SparePart,
            SKU = "SAM-A54-BAT",
            DefaultSalePrice = 1199m,
            IsSerialized = false,
            IsUniversal = false,
            IsActive = true,
            PublishStatus = ProductPublishStatus.Published
        };
        context.Products.AddRange(samsungA54Screen, samsungA54Battery);

        // ── S5: iPhone 13 specific products (new) ────────────────────────────────
        iphone13Screen = new Product
        {
            CompanyId = companyId,
            CategoryId = iPhonesCat.Id,
            Name = "iPhone 13 OLED Display Assembly",
            Kind = ProductKind.SparePart,
            SKU = "APL-IP13-SCR",
            DefaultSalePrice = 7999m,
            IsSerialized = false,
            IsUniversal = false,
            IsActive = true,
            PublishStatus = ProductPublishStatus.Published
        };
        iphone13Battery = new Product
        {
            CompanyId = companyId,
            CategoryId = iPhonesCat.Id,
            Name = "iPhone 13 Battery 3227mAh",
            Kind = ProductKind.SparePart,
            SKU = "APL-IP13-BAT",
            DefaultSalePrice = 1999m,
            IsSerialized = false,
            IsUniversal = false,
            IsActive = true,
            PublishStatus = ProductPublishStatus.Published
        };
        context.Products.AddRange(iphone13Screen, iphone13Battery);

        // ── S5: iPhone 15 specific products (new) ────────────────────────────────
        iphone15Screen = new Product
        {
            CompanyId = companyId,
            CategoryId = iPhonesCat.Id,
            Name = "iPhone 15 OLED Screen Assembly",
            Kind = ProductKind.SparePart,
            SKU = "APL-IP15-SCR",
            DefaultSalePrice = 11999m,
            IsSerialized = false,
            IsUniversal = false,
            IsActive = true,
            PublishStatus = ProductPublishStatus.Published
        };
        iphone15Battery = new Product
        {
            CompanyId = companyId,
            CategoryId = iPhonesCat.Id,
            Name = "iPhone 15 Battery 3349mAh",
            Kind = ProductKind.SparePart,
            SKU = "APL-IP15-BAT",
            DefaultSalePrice = 2999m,
            IsSerialized = false,
            IsUniversal = false,
            IsActive = true,
            PublishStatus = ProductPublishStatus.Published
        };
        context.Products.AddRange(iphone15Screen, iphone15Battery);

        // ── S6: Additional Cases & Covers ────────────────────────────────────────
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId,
                CategoryId = casesCoversCat.Id,
                Name = "Shockproof Rugged Case 6.7inch",
                Kind = ProductKind.Accessory,
                SKU = "ACC-CSR-001",
                DefaultSalePrice = 399m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = casesCoversCat.Id,
                Name = "Clear Silicone Case 6.1inch",
                Kind = ProductKind.Accessory,
                SKU = "ACC-CSS-001",
                DefaultSalePrice = 199m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = casesCoversCat.Id,
                Name = "Leather Flip Cover Universal",
                Kind = ProductKind.Accessory,
                SKU = "ACC-CSL-001",
                DefaultSalePrice = 449m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );

        // ── S6: Additional Cables & Chargers ─────────────────────────────────────
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId,
                CategoryId = cablesChargersCat.Id,
                Name = "65W GaN Fast Charger Dual Port",
                Kind = ProductKind.Accessory,
                SKU = "ACC-CHG-65G",
                DefaultSalePrice = 1299m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = cablesChargersCat.Id,
                Name = "10000mAh Power Bank",
                Kind = ProductKind.Accessory,
                SKU = "ACC-PBK-10K",
                DefaultSalePrice = 1299m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = cablesChargersCat.Id,
                Name = "USB-C Braided Cable 2m",
                Kind = ProductKind.Accessory,
                SKU = "ACC-CBC-2MB",
                DefaultSalePrice = 399m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );

        // ── S6: Additional Screen Protectors ─────────────────────────────────────
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId,
                CategoryId = screenProtectorsCat.Id,
                Name = "Anti-Glare Matte Screen Protector",
                Kind = ProductKind.Accessory,
                SKU = "ACC-SPM-001",
                DefaultSalePrice = 199m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = screenProtectorsCat.Id,
                Name = "Privacy Filter Screen Protector",
                Kind = ProductKind.Accessory,
                SKU = "ACC-SPP-001",
                DefaultSalePrice = 299m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = screenProtectorsCat.Id,
                Name = "Hydrogel Soft Screen Protector",
                Kind = ProductKind.Accessory,
                SKU = "ACC-SPH-001",
                DefaultSalePrice = 149m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );

        // ── S7: Android Tablets ───────────────────────────────────────────────────
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId,
                CategoryId = androidTabletsCat.Id,
                Name = "Android Tablet Tempered Glass 10.1inch",
                Kind = ProductKind.Accessory,
                SKU = "TAB-AND-TG1",
                DefaultSalePrice = 299m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = androidTabletsCat.Id,
                Name = "Android Tablet USB-C Charging Port",
                Kind = ProductKind.SparePart,
                SKU = "TAB-AND-CP1",
                DefaultSalePrice = 599m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = androidTabletsCat.Id,
                Name = "Android Tablet Folio Case 10.1inch",
                Kind = ProductKind.Accessory,
                SKU = "TAB-AND-FC1",
                DefaultSalePrice = 699m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );

        // ── S7: iPads ─────────────────────────────────────────────────────────────
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId,
                CategoryId = iPadsCat.Id,
                Name = "iPad 9th Gen Screen Assembly",
                Kind = ProductKind.SparePart,
                SKU = "TAB-IPD-SC9",
                DefaultSalePrice = 5999m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = iPadsCat.Id,
                Name = "iPad Smart Folio Case",
                Kind = ProductKind.Accessory,
                SKU = "TAB-IPD-FC1",
                DefaultSalePrice = 1499m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = iPadsCat.Id,
                Name = "iPad Lightning Charging Port",
                Kind = ProductKind.SparePart,
                SKU = "TAB-IPD-LP1",
                DefaultSalePrice = 799m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );
    }

    // ────────────────────────────────────────────────────────────────────────────
    // Device Brands
    // ────────────────────────────────────────────────────────────────────────────
    private static (
        DeviceBrand samsung, DeviceBrand apple, DeviceBrand xiaomi, DeviceBrand onePlus
    ) SeedDeviceBrands(
        Flowtap_Repair.DbContext.IRepairDbContext context,
        ProductCategory androidPhonesCat,
        ProductCategory iPhonesCat)
    {
        // Mobile brands with category binding
        var samsung = new DeviceBrand { Name = "Samsung", ProductCategoryId = androidPhonesCat.Id, IsActive = true };
        var apple   = new DeviceBrand { Name = "Apple",   ProductCategoryId = iPhonesCat.Id,       IsActive = true };
        var xiaomi  = new DeviceBrand { Name = "Xiaomi",  ProductCategoryId = androidPhonesCat.Id, IsActive = true };
        var onePlus = new DeviceBrand { Name = "OnePlus", ProductCategoryId = androidPhonesCat.Id, IsActive = true };

        // Other mobile brands (no specific sub-category)
        var redmi    = new DeviceBrand { Name = "Redmi",    IsActive = true };
        var oppo     = new DeviceBrand { Name = "Oppo",     IsActive = true };
        var vivo     = new DeviceBrand { Name = "Vivo",     IsActive = true };
        var realme   = new DeviceBrand { Name = "Realme",   IsActive = true };
        var huawei   = new DeviceBrand { Name = "Huawei",   IsActive = true };
        var nokia    = new DeviceBrand { Name = "Nokia",    IsActive = true };
        var motorola = new DeviceBrand { Name = "Motorola", IsActive = true };
        var sony     = new DeviceBrand { Name = "Sony",     IsActive = true };
        var lg       = new DeviceBrand { Name = "LG",       IsActive = true };
        var google   = new DeviceBrand { Name = "Google",   IsActive = true };

        context.DeviceBrands.AddRange(
            samsung, apple, xiaomi, onePlus,
            redmi, oppo, vivo, realme, huawei, nokia, motorola, sony, lg, google);

        return (samsung, apple, xiaomi, onePlus);
    }

    // ────────────────────────────────────────────────────────────────────────────
    // Device Models
    // ────────────────────────────────────────────────────────────────────────────
    private static (
        DeviceModel galaxyS23, DeviceModel galaxyS24, DeviceModel galaxyS22,
        DeviceModel galaxyA54, DeviceModel galaxyA34,
        DeviceModel iphone15Pro, DeviceModel iphone15, DeviceModel iphone14Pro,
        DeviceModel iphone14, DeviceModel iphone13
    ) SeedDeviceModels(
        Flowtap_Repair.DbContext.IRepairDbContext context,
        DeviceBrand samsung, DeviceBrand apple, DeviceBrand xiaomi, DeviceBrand onePlus,
        ProductCategory androidPhonesCat,
        ProductCategory iPhonesCat)
    {
        // Samsung models
        var galaxyS24   = new DeviceModel { BrandId = samsung.Id, ProductCategoryId = androidPhonesCat.Id, Name = "Galaxy S24",  IsActive = true };
        var galaxyS23   = new DeviceModel { BrandId = samsung.Id, ProductCategoryId = androidPhonesCat.Id, Name = "Galaxy S23",  IsActive = true };
        var galaxyS22   = new DeviceModel { BrandId = samsung.Id, ProductCategoryId = androidPhonesCat.Id, Name = "Galaxy S22",  IsActive = true };
        var galaxyA54   = new DeviceModel { BrandId = samsung.Id, ProductCategoryId = androidPhonesCat.Id, Name = "Galaxy A54",  IsActive = true };
        var galaxyA34   = new DeviceModel { BrandId = samsung.Id, ProductCategoryId = androidPhonesCat.Id, Name = "Galaxy A34",  IsActive = true };

        context.DeviceModels.AddRange(galaxyS24, galaxyS23, galaxyS22, galaxyA54, galaxyA34);

        // Apple mobile models
        var iphone15Pro = new DeviceModel { BrandId = apple.Id, ProductCategoryId = iPhonesCat.Id, Name = "iPhone 15 Pro", IsActive = true };
        var iphone15    = new DeviceModel { BrandId = apple.Id, ProductCategoryId = iPhonesCat.Id, Name = "iPhone 15",     IsActive = true };
        var iphone14Pro = new DeviceModel { BrandId = apple.Id, ProductCategoryId = iPhonesCat.Id, Name = "iPhone 14 Pro", IsActive = true };
        var iphone14    = new DeviceModel { BrandId = apple.Id, ProductCategoryId = iPhonesCat.Id, Name = "iPhone 14",     IsActive = true };
        var iphone13    = new DeviceModel { BrandId = apple.Id, ProductCategoryId = iPhonesCat.Id, Name = "iPhone 13",     IsActive = true };

        context.DeviceModels.AddRange(iphone15Pro, iphone15, iphone14Pro, iphone14, iphone13);

        // Xiaomi models
        context.DeviceModels.AddRange(
            new DeviceModel { BrandId = xiaomi.Id, ProductCategoryId = androidPhonesCat.Id, Name = "Redmi Note 12",  IsActive = true },
            new DeviceModel { BrandId = xiaomi.Id, ProductCategoryId = androidPhonesCat.Id, Name = "Redmi Note 13",  IsActive = true },
            new DeviceModel { BrandId = xiaomi.Id, ProductCategoryId = androidPhonesCat.Id, Name = "Xiaomi 13",      IsActive = true },
            new DeviceModel { BrandId = xiaomi.Id, ProductCategoryId = androidPhonesCat.Id, Name = "Xiaomi 13 Lite", IsActive = true }
        );

        // OnePlus models
        context.DeviceModels.AddRange(
            new DeviceModel { BrandId = onePlus.Id, ProductCategoryId = androidPhonesCat.Id, Name = "OnePlus 11",       IsActive = true },
            new DeviceModel { BrandId = onePlus.Id, ProductCategoryId = androidPhonesCat.Id, Name = "OnePlus Nord CE 3", IsActive = true }
        );

        return (
            galaxyS23, galaxyS24, galaxyS22, galaxyA54, galaxyA34,
            iphone15Pro, iphone15, iphone14Pro, iphone14, iphone13
        );
    }

    // ────────────────────────────────────────────────────────────────────────────
    // Product → Device Model Mappings
    // ────────────────────────────────────────────────────────────────────────────
    private static void SeedProductDeviceModelMappings(
        Flowtap_Repair.DbContext.IRepairDbContext context,
        // Existing — S23 & iPhone 14
        Product samsungS23Screen, Product samsungS23Battery, Product samsungS23Cover,
        DeviceModel galaxyS23,
        Product iphone14Screen, Product iphone14Battery, Product iphone14Cover,
        DeviceModel iphone14,
        // New — S22
        Product samsungS22Screen, Product samsungS22Battery,
        DeviceModel galaxyS22,
        // New — A54
        Product samsungA54Screen, Product samsungA54Battery,
        DeviceModel galaxyA54,
        // New — iPhone 13
        Product iphone13Screen, Product iphone13Battery,
        DeviceModel iphone13,
        // New — iPhone 15
        Product iphone15Screen, Product iphone15Battery,
        DeviceModel iphone15)
    {
        context.ProductDeviceModelMappings.AddRange(
            // ── Galaxy S23 (3 mappings) ──
            new ProductDeviceModelMapping { ProductId = samsungS23Screen.Id,  DeviceModelId = galaxyS23.Id },
            new ProductDeviceModelMapping { ProductId = samsungS23Battery.Id, DeviceModelId = galaxyS23.Id },
            new ProductDeviceModelMapping { ProductId = samsungS23Cover.Id,   DeviceModelId = galaxyS23.Id },
            // ── iPhone 14 (3 mappings) ──
            new ProductDeviceModelMapping { ProductId = iphone14Screen.Id,    DeviceModelId = iphone14.Id  },
            new ProductDeviceModelMapping { ProductId = iphone14Battery.Id,   DeviceModelId = iphone14.Id  },
            new ProductDeviceModelMapping { ProductId = iphone14Cover.Id,     DeviceModelId = iphone14.Id  },
            // ── Galaxy S22 (2 mappings) ──
            new ProductDeviceModelMapping { ProductId = samsungS22Screen.Id,  DeviceModelId = galaxyS22.Id },
            new ProductDeviceModelMapping { ProductId = samsungS22Battery.Id, DeviceModelId = galaxyS22.Id },
            // ── Galaxy A54 (2 mappings) ──
            new ProductDeviceModelMapping { ProductId = samsungA54Screen.Id,  DeviceModelId = galaxyA54.Id },
            new ProductDeviceModelMapping { ProductId = samsungA54Battery.Id, DeviceModelId = galaxyA54.Id },
            // ── iPhone 13 (2 mappings) ──
            new ProductDeviceModelMapping { ProductId = iphone13Screen.Id,    DeviceModelId = iphone13.Id  },
            new ProductDeviceModelMapping { ProductId = iphone13Battery.Id,   DeviceModelId = iphone13.Id  },
            // ── iPhone 15 (2 mappings) ──
            new ProductDeviceModelMapping { ProductId = iphone15Screen.Id,    DeviceModelId = iphone15.Id  },
            new ProductDeviceModelMapping { ProductId = iphone15Battery.Id,   DeviceModelId = iphone15.Id  }
        );
    }

    // ────────────────────────────────────────────────────────────────────────────
    // Services
    // ────────────────────────────────────────────────────────────────────────────
    private static void SeedServices(
        Flowtap_Repair.DbContext.IRepairDbContext context,
        Guid companyId,
        ServiceCategory screenRepairCat,
        ServiceCategory batteryServiceCat,
        ServiceCategory hardwareRepairCat,
        ServiceCategory softwareServicesCat,
        ServiceCategory waterDamageCat,
        ProductCategory androidPhonesCat,
        ProductCategory iPhonesCat,
        DeviceModel iphone14,
        DeviceModel galaxyS23)
    {
        // ── Universal services (IsUniversal=true) ──
        context.Services.AddRange(
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = softwareServicesCat.Id,
                Name = "Software Update & Troubleshooting",
                BasePrice = 299m,
                RequiresInventory = false,
                IsUniversal = true,
                IsActive = true
            },
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = softwareServicesCat.Id,
                Name = "Data Backup & Transfer",
                BasePrice = 399m,
                RequiresInventory = false,
                IsUniversal = true,
                IsActive = true
            },
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = softwareServicesCat.Id,
                Name = "Virus & Malware Removal",
                BasePrice = 499m,
                RequiresInventory = false,
                IsUniversal = true,
                IsActive = true
            },
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = softwareServicesCat.Id,
                Name = "Factory Reset & Setup",
                BasePrice = 299m,
                RequiresInventory = false,
                IsUniversal = true,
                IsActive = true
            }
        );

        // ── Mobile universal services (ProductCategoryId = Android Phones) ──
        context.Services.AddRange(
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = screenRepairCat.Id,
                ProductCategoryId = androidPhonesCat.Id,
                Name = "Screen Replacement (Mobile)",
                BasePrice = 799m,
                RequiresInventory = true,
                IsUniversal = false,
                IsActive = true
            },
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = batteryServiceCat.Id,
                ProductCategoryId = androidPhonesCat.Id,
                Name = "Battery Replacement (Mobile)",
                BasePrice = 499m,
                RequiresInventory = true,
                IsUniversal = false,
                IsActive = true
            },
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = hardwareRepairCat.Id,
                ProductCategoryId = androidPhonesCat.Id,
                Name = "Charging Port Repair",
                BasePrice = 399m,
                RequiresInventory = false,
                IsUniversal = false,
                IsActive = true
            },
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = hardwareRepairCat.Id,
                ProductCategoryId = androidPhonesCat.Id,
                Name = "Camera Module Replacement",
                BasePrice = 699m,
                RequiresInventory = true,
                IsUniversal = false,
                IsActive = true
            },
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = waterDamageCat.Id,
                ProductCategoryId = androidPhonesCat.Id,
                Name = "Water Damage Treatment (Mobile)",
                BasePrice = 999m,
                RequiresInventory = false,
                IsUniversal = false,
                IsActive = true
            },
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = hardwareRepairCat.Id,
                ProductCategoryId = androidPhonesCat.Id,
                Name = "Speaker / Microphone Repair",
                BasePrice = 349m,
                RequiresInventory = false,
                IsUniversal = false,
                IsActive = true
            }
        );

        // ── iPhone 14 model-specific services ──
        var ip14Screen = new Service
        {
            CompanyId = companyId,
            ServiceCategoryId = screenRepairCat.Id,
            ProductCategoryId = iPhonesCat.Id,
            Name = "iPhone 14 OLED Screen Replacement",
            BasePrice = 4499m,
            RequiresInventory = true,
            IsUniversal = false,
            IsActive = true
        };
        var ip14Battery = new Service
        {
            CompanyId = companyId,
            ServiceCategoryId = batteryServiceCat.Id,
            ProductCategoryId = iPhonesCat.Id,
            Name = "iPhone 14 Battery Replacement",
            BasePrice = 1499m,
            RequiresInventory = true,
            IsUniversal = false,
            IsActive = true
        };
        var ip14FaceId = new Service
        {
            CompanyId = companyId,
            ServiceCategoryId = hardwareRepairCat.Id,
            ProductCategoryId = iPhonesCat.Id,
            Name = "iPhone 14 Face ID Repair",
            BasePrice = 2999m,
            RequiresInventory = false,
            IsUniversal = false,
            IsActive = true
        };
        context.Services.AddRange(ip14Screen, ip14Battery, ip14FaceId);

        context.ServiceDeviceModelMappings.AddRange(
            new ServiceDeviceModelMapping { ServiceId = ip14Screen.Id,  DeviceModelId = iphone14.Id },
            new ServiceDeviceModelMapping { ServiceId = ip14Battery.Id, DeviceModelId = iphone14.Id },
            new ServiceDeviceModelMapping { ServiceId = ip14FaceId.Id,  DeviceModelId = iphone14.Id }
        );

        // ── Samsung Galaxy S23 model-specific services ──
        var s23Screen = new Service
        {
            CompanyId = companyId,
            ServiceCategoryId = screenRepairCat.Id,
            ProductCategoryId = androidPhonesCat.Id,
            Name = "Samsung S23 AMOLED Screen Replacement",
            BasePrice = 2999m,
            RequiresInventory = true,
            IsUniversal = false,
            IsActive = true
        };
        var s23Battery = new Service
        {
            CompanyId = companyId,
            ServiceCategoryId = batteryServiceCat.Id,
            ProductCategoryId = androidPhonesCat.Id,
            Name = "Samsung S23 Battery Replacement",
            BasePrice = 999m,
            RequiresInventory = true,
            IsUniversal = false,
            IsActive = true
        };
        context.Services.AddRange(s23Screen, s23Battery);

        context.ServiceDeviceModelMappings.AddRange(
            new ServiceDeviceModelMapping { ServiceId = s23Screen.Id,  DeviceModelId = galaxyS23.Id },
            new ServiceDeviceModelMapping { ServiceId = s23Battery.Id, DeviceModelId = galaxyS23.Id }
        );
    }
}
