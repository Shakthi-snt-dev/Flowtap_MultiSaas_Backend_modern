using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Domain.Industry;

public static class RepairShopCarSeeder
{
    public static async Task SeedAsync(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId, CancellationToken ct)
    {
        SeedProductCategories(context, companyId);
        SeedServiceCategories(context, companyId);
        await SeedDeviceBrandsAndModelsAsync(context, ct);
    }

    private static void SeedProductCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // --- Car Parts ---
        var carParts = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Car Parts",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsBrandExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(carParts);

        var engineCat = new ProductCategory { CompanyId = companyId, Name = "Engine Parts",             ParentCategoryId = carParts.Id, SortOrder = 1, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var electricalPartsCat = new ProductCategory { CompanyId = companyId, Name = "Electrical & Electronics", ParentCategoryId = carParts.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var bodyCat = new ProductCategory { CompanyId = companyId, Name = "Body & Exterior Parts",    ParentCategoryId = carParts.Id, SortOrder = 3, IsDirectProductExist = true, IsBrandExist = true, IsActive = true };
        var brakesCat = new ProductCategory { CompanyId = companyId, Name = "Brakes & Suspension",      ParentCategoryId = carParts.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var transmissionCat = new ProductCategory { CompanyId = companyId, Name = "Transmission & Gearbox",   ParentCategoryId = carParts.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        var coolingCat = new ProductCategory { CompanyId = companyId, Name = "Cooling System",           ParentCategoryId = carParts.Id, SortOrder = 6, IsDirectProductExist = true, IsActive = true };
        var otherCarPartsCat = new ProductCategory { CompanyId = companyId, Name = "Other Car Parts",          ParentCategoryId = carParts.Id, SortOrder = 7, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(engineCat, electricalPartsCat, bodyCat, brakesCat, transmissionCat, coolingCat, otherCarPartsCat);

        // --- Tyres & Wheels ---
        var tyres = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Tyres & Wheels",
            SortOrder = 2,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(tyres);

        var carTyresCat = new ProductCategory { CompanyId = companyId, Name = "Car Tyres",     ParentCategoryId = tyres.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var wheelsCat = new ProductCategory { CompanyId = companyId, Name = "Wheels & Rims", ParentCategoryId = tyres.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(carTyresCat, wheelsCat);

        // --- Car Accessories ---
        var accessories = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Car Accessories",
            SortOrder = 3,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(accessories);

        var audioCat = new ProductCategory { CompanyId = companyId, Name = "Audio & Entertainment",  ParentCategoryId = accessories.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var interiorCat = new ProductCategory { CompanyId = companyId, Name = "Interior Accessories",   ParentCategoryId = accessories.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var exteriorCat = new ProductCategory { CompanyId = companyId, Name = "Exterior Accessories",   ParentCategoryId = accessories.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var carCareCat = new ProductCategory { CompanyId = companyId, Name = "Car Care Products",      ParentCategoryId = accessories.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(audioCat, interiorCat, exteriorCat, carCareCat);

        // --- Services ---
        var services = new ProductCategory
        {
            CompanyId = companyId,
            Name = "Services",
            SortOrder = 4,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ProductCategories.Add(services);

        var engineRep = new ProductCategory { CompanyId = companyId, Name = "Engine Repair & Overhaul", ParentCategoryId = services.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var paintRep = new ProductCategory { CompanyId = companyId, Name = "Body & Paint Work",        ParentCategoryId = services.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var electricalRep = new ProductCategory { CompanyId = companyId, Name = "Electrical Repair",        ParentCategoryId = services.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var acRep = new ProductCategory { CompanyId = companyId, Name = "AC Repair & Service",      ParentCategoryId = services.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var serviceRep = new ProductCategory { CompanyId = companyId, Name = "Routine Servicing",        ParentCategoryId = services.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        var tyreRep = new ProductCategory { CompanyId = companyId, Name = "Tyre & Wheel Service",     ParentCategoryId = services.Id, SortOrder = 6, IsDirectProductExist = true, IsActive = true };

        context.ProductCategories.AddRange(engineRep, paintRep, electricalRep, acRep, serviceRep, tyreRep);

        // --- Seed Car Products ---
        context.Products.AddRange(
            new Product
            {
                CompanyId = companyId,
                CategoryId = brakesCat.Id,
                Name = "Premium Ceramic Brake Pads Set (Front)",
                Kind = ProductKind.SparePart,
                SKU = "CAR-BR-PAD",
                DefaultCostPrice = 22.00m,
                DefaultSalePrice = 69.99m,
                IsSerialized = false,
                IsUniversal = false,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = electricalPartsCat.Id,
                Name = "12V 65Ah Maintenance-Free Car Battery",
                Kind = ProductKind.SparePart,
                SKU = "CAR-EL-BAT",
                DefaultCostPrice = 45.00m,
                DefaultSalePrice = 119.99m,
                IsSerialized = true,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            },
            new Product
            {
                CompanyId = companyId,
                CategoryId = carCareCat.Id,
                Name = "Premium Carnauba Liquid Car Wax 500ml",
                Kind = ProductKind.Accessory,
                SKU = "CAR-AC-WAX",
                DefaultCostPrice = 4.00m,
                DefaultSalePrice = 18.50m,
                IsSerialized = false,
                IsUniversal = true,
                IsActive = true,
                PublishStatus = ProductPublishStatus.Published
            }
        );
    }

    private static void SeedServiceCategories(Flowtap_Repair.DbContext.IRepairDbContext context, Guid companyId)
    {
        // --- Engine & Mechanical ---
        var engine = new ServiceCategory
        {
            CompanyId = companyId,
            Name = "Engine & Mechanical",
            SortOrder = 1,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ServiceCategories.Add(engine);

        var overhaulIssues = new ServiceCategory { CompanyId = companyId, Name = "Engine Overhaul", ParentCategoryId = engine.Id, SortOrder = 1, IsActive = true };
        var leakIssues = new ServiceCategory { CompanyId = companyId, Name = "Oil Leak",        ParentCategoryId = engine.Id, SortOrder = 2, IsActive = true };
        var overheatIssues = new ServiceCategory { CompanyId = companyId, Name = "Overheating",     ParentCategoryId = engine.Id, SortOrder = 3, IsActive = true };
        var fuelIssues = new ServiceCategory { CompanyId = companyId, Name = "Fuel System",     ParentCategoryId = engine.Id, SortOrder = 4, IsActive = true };

        context.ServiceCategories.AddRange(overhaulIssues, leakIssues, overheatIssues, fuelIssues);

        // --- Electrical Issues ---
        var electrical = new ServiceCategory
        {
            CompanyId = companyId,
            Name = "Electrical Issues",
            SortOrder = 2,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ServiceCategories.Add(electrical);

        var batteryIssues = new ServiceCategory { CompanyId = companyId, Name = "Battery Issues",       ParentCategoryId = electrical.Id, SortOrder = 1, IsActive = true };
        var starterIssues = new ServiceCategory { CompanyId = companyId, Name = "Starter/Alternator",   ParentCategoryId = electrical.Id, SortOrder = 2, IsActive = true };
        var wiringIssues = new ServiceCategory { CompanyId = companyId, Name = "Wiring & Fuses",       ParentCategoryId = electrical.Id, SortOrder = 3, IsActive = true };
        var acIssues = new ServiceCategory { CompanyId = companyId, Name = "AC Electrical",        ParentCategoryId = electrical.Id, SortOrder = 4, IsActive = true };

        context.ServiceCategories.AddRange(batteryIssues, starterIssues, wiringIssues, acIssues);

        // --- Body & Exterior ---
        var bodyExterior = new ServiceCategory
        {
            CompanyId = companyId,
            Name = "Body & Exterior",
            SortOrder = 3,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ServiceCategories.Add(bodyExterior);

        var dentIssues = new ServiceCategory { CompanyId = companyId, Name = "Dents & Scratches",   ParentCategoryId = bodyExterior.Id, SortOrder = 1, IsActive = true };
        var panelIssues = new ServiceCategory { CompanyId = companyId, Name = "Panel Beating",       ParentCategoryId = bodyExterior.Id, SortOrder = 2, IsActive = true };
        var glassIssues = new ServiceCategory { CompanyId = companyId, Name = "Windscreen Damage",   ParentCategoryId = bodyExterior.Id, SortOrder = 3, IsActive = true };
        var paintIssues = new ServiceCategory { CompanyId = companyId, Name = "Paint Work",          ParentCategoryId = bodyExterior.Id, SortOrder = 4, IsActive = true };

        context.ServiceCategories.AddRange(dentIssues, panelIssues, glassIssues, paintIssues);

        // --- Routine Maintenance ---
        var routine = new ServiceCategory
        {
            CompanyId = companyId,
            Name = "Routine Maintenance",
            SortOrder = 4,
            IsSubCategoryExist = true,
            IsActive = true
        };
        context.ServiceCategories.Add(routine);

        var oilIssues = new ServiceCategory { CompanyId = companyId, Name = "Oil Change",           ParentCategoryId = routine.Id, SortOrder = 1, IsActive = true };
        var filterIssues = new ServiceCategory { CompanyId = companyId, Name = "Filter Replacement",   ParentCategoryId = routine.Id, SortOrder = 2, IsActive = true };
        var rotateIssues = new ServiceCategory { CompanyId = companyId, Name = "Tyre Rotation",        ParentCategoryId = routine.Id, SortOrder = 3, IsActive = true };
        var fullCheckIssues = new ServiceCategory { CompanyId = companyId, Name = "Full Service Check",   ParentCategoryId = routine.Id, SortOrder = 4, IsActive = true };

        context.ServiceCategories.AddRange(oilIssues, filterIssues, rotateIssues, fullCheckIssues);

        // --- Seed Auto Services ---
        context.Services.AddRange(
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = oilIssues.Id,
                Name = "Premium Synthetic Oil & Filter Service",
                Description = "Includes high-grade synthetic oil replacement, oil filter change, and complete fluid top-up.",
                EstimatedDuration = "30 mins",
                BasePrice = 79.99m,
                RequiresInventory = true,
                IsActive = true,
                IsUniversal = true
            },
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = batteryIssues.Id,
                Name = "Car Battery Replacement Service",
                Description = "Installation of high-quality maintenance-free battery, terminal cleaning, and battery health report.",
                EstimatedDuration = "20 mins",
                BasePrice = 129.99m,
                RequiresInventory = true,
                IsActive = true,
                IsUniversal = true
            },
            new Service
            {
                CompanyId = companyId,
                ServiceCategoryId = fullCheckIssues.Id,
                Name = "50-Point Full Vehicle Health Inspection",
                Description = "Thorough checks on brakes, suspension, diagnostics scans, exhaust, and all safety points.",
                EstimatedDuration = "60 mins",
                BasePrice = 49.99m,
                RequiresInventory = false,
                IsActive = true,
                IsUniversal = true
            }
        );
    }

    private static async Task SeedDeviceBrandsAndModelsAsync(Flowtap_Repair.DbContext.IRepairDbContext context, CancellationToken ct)
    {
        // Toyota
        var toyota = await GetOrCreateBrandAsync(context, "Toyota", ct);
        await AddModelsAsync(context, toyota, new[] { "Camry", "Corolla", "Fortuner", "Hilux", "Land Cruiser", "RAV4", "Yaris", "Prado" }, ct);

        // Honda
        var honda = await GetOrCreateBrandAsync(context, "Honda", ct);
        await AddModelsAsync(context, honda, new[] { "Civic", "City", "Accord", "CR-V", "HR-V", "Jazz", "Pilot" }, ct);

        // Hyundai
        var hyundai = await GetOrCreateBrandAsync(context, "Hyundai", ct);
        await AddModelsAsync(context, hyundai, new[] { "Tucson", "Santa Fe", "Elantra", "i20", "i30", "Creta", "Sonata" }, ct);

        // Kia
        var kia = await GetOrCreateBrandAsync(context, "Kia", ct);
        await AddModelsAsync(context, kia, new[] { "Sportage", "Sorento", "Cerato", "Stonic", "Seltos" }, ct);

        // BMW
        var bmw = await GetOrCreateBrandAsync(context, "BMW", ct);
        await AddModelsAsync(context, bmw, new[] { "3 Series", "5 Series", "7 Series", "X3", "X5", "X7" }, ct);

        // Mercedes-Benz
        var mercedes = await GetOrCreateBrandAsync(context, "Mercedes-Benz", ct);
        await AddModelsAsync(context, mercedes, new[] { "C-Class", "E-Class", "S-Class", "GLC", "GLE", "A-Class" }, ct);

        // Other brands — no models
        await GetOrCreateBrandAsync(context, "Audi", ct);
        await GetOrCreateBrandAsync(context, "Ford", ct);
        await GetOrCreateBrandAsync(context, "Chevrolet", ct);
        await GetOrCreateBrandAsync(context, "Volkswagen", ct);
        await GetOrCreateBrandAsync(context, "Nissan", ct);
        await GetOrCreateBrandAsync(context, "Suzuki", ct);
        await GetOrCreateBrandAsync(context, "Renault", ct);
        await GetOrCreateBrandAsync(context, "Jeep", ct);
        await GetOrCreateBrandAsync(context, "Mitsubishi", ct);
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

