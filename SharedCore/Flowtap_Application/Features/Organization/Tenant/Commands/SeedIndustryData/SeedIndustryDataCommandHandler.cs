using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Organization.Tenant.Commands.SeedIndustryData.Seeders;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Organization.Tenant.Commands.SeedIndustryData;

public class SeedIndustryDataCommandHandler(IApplicationDbContext context, IEnumerable<IIndustryDataSeeder> seeders)
    : IRequestHandler<SeedIndustryDataCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(SeedIndustryDataCommand command, CancellationToken ct)
    {
        var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Id == command.CompanyId, ct);
        if (tenant == null)
            return Result<bool>.Failure("Tenant not found");

        if (!Enum.TryParse<IndustryType>(command.IndustryType, true, out var industryType))
            return Result<bool>.Failure($"Invalid industry type: {command.IndustryType}");

        // Set active modules
        tenant.ActiveModules = industryType switch
        {
            IndustryType.RepairShop => "POS,Inventory,ServiceTickets,Purchasing,Devices",
            IndustryType.Food => "POS,Inventory,KOT,Tables,Recipes,RawMaterials,Purchasing",
            IndustryType.Hotel => "POS,Rooms,Bookings,HouseKeeping,Inventory",
            IndustryType.Jewelry => "POS,Inventory,JewelryItems,MetalRates,MetalExchange,Purchasing",
            IndustryType.Medical => "Patients,Appointments,Consultations,Pharmacy,Inventory",
            _ => "POS,Inventory,Purchasing,Customers"
        };

        // Seed based on industry + business type using plugin pattern
        var seeder = seeders.FirstOrDefault(s => s.Industry == industryType);
        if (seeder != null)
        {
            await seeder.SeedAsync(context, command.CompanyId, command.BusinessType, ct);
        }
        else
        {
            // Default/fallback seeding (Retail)
            await RetailSeeder.SeedAsync(context, command.CompanyId, ct);
        }

        await context.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
