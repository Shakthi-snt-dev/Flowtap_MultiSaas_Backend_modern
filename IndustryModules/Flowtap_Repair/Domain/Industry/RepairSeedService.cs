using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Domain.Industry;

/// <summary>
/// Seeds default Repair industry data: service categories and device brands.
/// Registered as IIndustryDataSeeder via RepairServiceExtensions.
/// Idempotent per business type — safe to call multiple times for the same company
/// with different businessType values (e.g. first "mobile", then "laptop").
/// Each sub-seeder has a unique root category name, so we check that specific name
/// to avoid re-seeding the same type twice.
/// </summary>
public class RepairSeedService(Flowtap_Repair.DbContext.IRepairDbContext repairContext) : IIndustryDataSeeder
{
    public IndustryType Industry => IndustryType.RepairShop;

    public async Task SeedAsync(IApplicationDbContext _, Guid companyId, string? businessType, CancellationToken ct)
    {
        // Per-type idempotency check: each business type has a unique root category name.
        // This allows multi-type shops (e.g. mobile + laptop) to seed both without collision.
        var rootCategoryName = businessType?.ToLower() switch
        {
            "mobile" or "phone"         => "Spare Parts",
            "laptop" or "computer"      => "Laptop Parts",
            "automobile" or "car"       => "Car Parts",
            "electronics" or "general"  => "Electronics Parts",
            "smartwatch" or "wearable"  => "Smartwatch Parts",
            "gaming" or "console"       => "Gaming Console Parts",
            "tv" or "display"           => "TV & Display Parts",
            "printer" or "peripheral"   => "Printer Parts",
            _                           => "Spare Parts"
        };

        var alreadySeeded = await repairContext.ProductCategories
            .AnyAsync(c => c.CompanyId == companyId && c.Name == rootCategoryName, ct);
        if (alreadySeeded) return;

        // Route to the correct sub-seeder
        switch (businessType?.ToLower())
        {
            case "automobile":
            case "car":
                await RepairShopCarSeeder.SeedAsync(repairContext, companyId, ct);
                break;

            case "laptop":
            case "computer":
                await RepairShopLaptopSeeder.SeedAsync(repairContext, companyId, ct);
                break;

            case "electronics":
            case "general":
                await RepairShopElectronicsSeeder.SeedAsync(repairContext, companyId, ct);
                break;

            case "smartwatch":
            case "wearable":
                await RepairShopSmartwatchSeeder.SeedAsync(repairContext, companyId, ct);
                break;

            case "gaming":
            case "console":
                await RepairShopGamingSeeder.SeedAsync(repairContext, companyId, ct);
                break;

            case "tv":
            case "display":
                await RepairShopTVSeeder.SeedAsync(repairContext, companyId, ct);
                break;

            case "printer":
            case "peripheral":
                await RepairShopPrinterSeeder.SeedAsync(repairContext, companyId, ct);
                break;

            case "mobile":
            case "phone":
            default:
                await RepairShopMobileSeeder.SeedAsync(repairContext, companyId, ct);
                break;
        }

        await repairContext.SaveChangesAsync(ct);
    }
}
