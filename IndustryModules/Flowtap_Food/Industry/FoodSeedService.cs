using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;

namespace Flowtap_Food.Industry;

/// <summary>
/// Plugin seeder for the Food industry.
/// Registered in DI by AddFoodModule() and discovered by SeedIndustryDataCommandHandler.
/// Routes to sub-seeders based on businessType (restaurant, bakery, cafe, icecream, cloudkitchen).
/// </summary>
public class FoodSeedService : IIndustryDataSeeder
{
    public IndustryType Industry => IndustryType.Food;

    public Task SeedAsync(IApplicationDbContext context, Guid companyId, string businessType, CancellationToken ct)
    {
        return businessType?.ToLower() switch
        {
            "bakery"        => FoodBakerySeeder.SeedAsync(context, companyId, ct),
            "cafe"          => FoodCafeSeeder.SeedAsync(context, companyId, ct),
            "icecream"      => FoodIceCreamSeeder.SeedAsync(context, companyId, ct),
            "cloudkitchen"  => FoodCloudKitchenSeeder.SeedAsync(context, companyId, ct),
            _               => FoodRestaurantSeeder.SeedAsync(context, companyId, ct)  // default
        };
    }
}
