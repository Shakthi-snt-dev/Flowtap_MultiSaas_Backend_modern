using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Infrastructure.Services;

public class TaxTemplateService(IApplicationDbContext db) : ITaxTemplateService
{
    public async Task<Result<bool>> ApplyTemplateAsync(Guid storeId, string countryCode, CancellationToken ct = default)
    {
        var store = await db.Stores
            .FirstOrDefaultAsync(s => s.Id == storeId, ct);

        if (store == null) return Result<bool>.Failure("Store not found");

        var existingConfig = await db.TaxConfigurations
            .Include(t => t.RateRules)
            .FirstOrDefaultAsync(t => t.Id == store.TaxConfigurationId || (t.CompanyId == store.CompanyId && t.CountryCode == countryCode), ct);

        if (existingConfig == null)
        {
            existingConfig = new TaxConfiguration
            {
                CompanyId = store.CompanyId,
                CountryCode = countryCode,
                IsActive = true
            };
            db.TaxConfigurations.Add(existingConfig);
            store.TaxConfiguration = existingConfig;
        }

        // Apply Template Logic
        switch (countryCode.ToUpper())
        {
            case "IN":
                existingConfig.SystemType = TaxSystemType.GST;
                await SetupIndiaGstTemplate(existingConfig, ct);
                break;
            case "CA":
                existingConfig.SystemType = TaxSystemType.SalesTax;
                await SetupCanadaTaxTemplate(existingConfig, ct);
                break;
            case "US":
                existingConfig.SystemType = TaxSystemType.SalesTax;
                await SetupUsaTaxTemplate(existingConfig, ct);
                break;
            case "AU":
            case "SG":
            case "NZ":
                existingConfig.SystemType = TaxSystemType.Australia; // Australia type used for generic GST countries
                await SetupVatTemplate(existingConfig, "GST", ct);
                break;
            case "SA":
            case "QA":
            case "OM":
                existingConfig.SystemType = TaxSystemType.VAT_UAE;
                await SetupVatTemplate(existingConfig, "VAT", ct);
                break;
            case "AE":
                existingConfig.SystemType = TaxSystemType.VAT_UAE;
                await SetupVatTemplate(existingConfig, "VAT", ct);
                break;
            case "GB":
            case "UK":
                existingConfig.SystemType = TaxSystemType.VAT_EU;
                await SetupVatTemplate(existingConfig, "VAT", ct);
                break;
            default:
                existingConfig.SystemType = TaxSystemType.SalesTax;
                await SetupVatTemplate(existingConfig, "Tax", ct);
                break;
        }

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }

    private async Task SetupIndiaGstTemplate(TaxConfiguration config, CancellationToken ct)
    {
        var slabs = await db.TaxSlabs.Where(s => s.CompanyId == config.CompanyId).ToListAsync(ct);
        
        // If no slabs exist, we could create default ones, but let's assume they exist or we use existing ones
        foreach (var slab in slabs)
        {
            // Remove old rules for this slab in this config to avoid duplicates
            var oldRules = config.RateRules.Where(r => r.TaxSlabId == slab.Id).ToList();
            foreach (var old in oldRules) config.RateRules.Remove(old);

            // Add CGST (50%)
            config.RateRules.Add(new TaxRateRule
            {
                TaxSlabId = slab.Id,
                ComponentName = "CGST",
                Rate = 0, // Set to 0 by default, user will update in Slab management if needed
                Jurisdiction = "Central"
            });

            // Add SGST (50%)
            config.RateRules.Add(new TaxRateRule
            {
                TaxSlabId = slab.Id,
                ComponentName = "SGST",
                Rate = 0,
                Jurisdiction = "State"
            });

            // Add IGST (100%)
            config.RateRules.Add(new TaxRateRule
            {
                TaxSlabId = slab.Id,
                ComponentName = "IGST",
                Rate = 0,
                Jurisdiction = "Inter-State"
            });
        }
    }

    private async Task SetupCanadaTaxTemplate(TaxConfiguration config, CancellationToken ct)
    {
        var slabs = await db.TaxSlabs.Where(s => s.CompanyId == config.CompanyId).ToListAsync(ct);
        
        foreach (var slab in slabs)
        {
            var oldRules = config.RateRules.Where(r => r.TaxSlabId == slab.Id).ToList();
            foreach (var old in oldRules) config.RateRules.Remove(old);

            // Federal Tax
            config.RateRules.Add(new TaxRateRule
            {
                TaxSlabId = slab.Id,
                ComponentName = "GST",
                Rate = 0,
                Jurisdiction = "Federal"
            });

            // Provincial Tax (User will adjust based on province like BC, ON, QC)
            config.RateRules.Add(new TaxRateRule
            {
                TaxSlabId = slab.Id,
                ComponentName = "PST/HST",
                Rate = 0,
                Jurisdiction = "Provincial"
            });
        }
    }

    private async Task SetupUsaTaxTemplate(TaxConfiguration config, CancellationToken ct)
    {
        var slabs = await db.TaxSlabs.Where(s => s.CompanyId == config.CompanyId).ToListAsync(ct);
        
        foreach (var slab in slabs)
        {
            var oldRules = config.RateRules.Where(r => r.TaxSlabId == slab.Id).ToList();
            foreach (var old in oldRules) config.RateRules.Remove(old);

            config.RateRules.Add(new TaxRateRule
            {
                TaxSlabId = slab.Id,
                ComponentName = "State Tax",
                Rate = 0,
                Jurisdiction = "State"
            });

            config.RateRules.Add(new TaxRateRule
            {
                TaxSlabId = slab.Id,
                ComponentName = "Local Tax",
                Rate = 0,
                Jurisdiction = "City/County"
            });
        }
    }

    private async Task SetupVatTemplate(TaxConfiguration config, string componentName, CancellationToken ct)
    {
        var slabs = await db.TaxSlabs.Where(s => s.CompanyId == config.CompanyId).ToListAsync(ct);
        
        foreach (var slab in slabs)
        {
            var oldRules = config.RateRules.Where(r => r.TaxSlabId == slab.Id).ToList();
            foreach (var old in oldRules) config.RateRules.Remove(old);

            config.RateRules.Add(new TaxRateRule
            {
                TaxSlabId = slab.Id,
                ComponentName = componentName,
                Rate = 0
            });
        }
    }
}
