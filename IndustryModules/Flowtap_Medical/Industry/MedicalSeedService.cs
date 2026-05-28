using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;

namespace Flowtap_Medical.Industry;

/// <summary>
/// Plugin seeder for the Medical industry.
/// Registered in DI by AddMedicalModule() and discovered by SeedIndustryDataCommandHandler.
/// </summary>
public class MedicalSeedService : IIndustryDataSeeder
{
    public IndustryType Industry => IndustryType.Medical;

    public Task SeedAsync(IApplicationDbContext context, Guid companyId, string businessType, CancellationToken ct)
    {
        SeedProductCategories(context, companyId);
        return Task.CompletedTask;
    }

    private static void SeedProductCategories(IApplicationDbContext context, Guid companyId)
    {
        // Medicines
        var medicines = new ProductCategory { CompanyId = companyId, Name = "Medicines", SortOrder = 1, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(medicines);

        var tabletsCat    = new ProductCategory { CompanyId = companyId, Name = "Tablets & Capsules",   ParentCategoryId = medicines.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var syrupsCat     = new ProductCategory { CompanyId = companyId, Name = "Syrups & Liquids",     ParentCategoryId = medicines.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var injectionsCat = new ProductCategory { CompanyId = companyId, Name = "Injections & Drops",   ParentCategoryId = medicines.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        var creamsCat     = new ProductCategory { CompanyId = companyId, Name = "Topical & Creams",     ParentCategoryId = medicines.Id, SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        var vitaminsCat   = new ProductCategory { CompanyId = companyId, Name = "Vitamins & Supplements", ParentCategoryId = medicines.Id, SortOrder = 5, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(tabletsCat, syrupsCat, injectionsCat, creamsCat, vitaminsCat);

        // Medical Devices & Equipment
        var devices = new ProductCategory { CompanyId = companyId, Name = "Medical Devices & Equipment", SortOrder = 2, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(devices);

        var diagnosticCat = new ProductCategory { CompanyId = companyId, Name = "Diagnostic Devices",   ParentCategoryId = devices.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var monitoringCat = new ProductCategory { CompanyId = companyId, Name = "Monitoring Equipment", ParentCategoryId = devices.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(diagnosticCat, monitoringCat);

        // Consumables
        var consumables = new ProductCategory { CompanyId = companyId, Name = "Consumables", SortOrder = 3, IsSubCategoryExist = true, IsActive = true };
        context.ProductCategories.Add(consumables);

        var bandagesCat = new ProductCategory { CompanyId = companyId, Name = "Bandages & Dressings", ParentCategoryId = consumables.Id, SortOrder = 1, IsDirectProductExist = true, IsActive = true };
        var glovesCat   = new ProductCategory { CompanyId = companyId, Name = "Gloves & Masks",       ParentCategoryId = consumables.Id, SortOrder = 2, IsDirectProductExist = true, IsActive = true };
        var syringesCat = new ProductCategory { CompanyId = companyId, Name = "Syringes & Needles",   ParentCategoryId = consumables.Id, SortOrder = 3, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.AddRange(bandagesCat, glovesCat, syringesCat);

        // Lab Tests
        var labTestsCat = new ProductCategory { CompanyId = companyId, Name = "Lab Tests", SortOrder = 4, IsDirectProductExist = true, IsActive = true };
        context.ProductCategories.Add(labTestsCat);

        // Sample Products
        context.Products.AddRange(
            new Product { CompanyId = companyId, CategoryId = tabletsCat.Id,    Name = "Paracetamol 500mg Tablets (Box of 100)",            Kind = ProductKind.Accessory, SKU = "MED-TB-PAR", DefaultCostPrice = 1.50m,  DefaultSalePrice = 9.99m,  IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = diagnosticCat.Id, Name = "Digital Upper Arm Blood Pressure Monitor",           Kind = ProductKind.Accessory, SKU = "MED-EQ-BPM", DefaultCostPrice = 12.00m, DefaultSalePrice = 39.99m, IsSerialized = true,  IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published },
            new Product { CompanyId = companyId, CategoryId = glovesCat.Id,     Name = "Disposable Nitrile Examination Gloves (Box of 100)", Kind = ProductKind.Accessory, SKU = "MED-CN-GLV", DefaultCostPrice = 2.50m,  DefaultSalePrice = 12.50m, IsSerialized = false, IsUniversal = true, IsActive = true, PublishStatus = ProductPublishStatus.Published }
        );
    }
}
