using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Flowtap_Infrastructure.Persistence.DbContext;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Presentation.Controllers;

// ─── Request DTOs ─────────────────────────────────────────────────────────────

public record SaveTaxConfigurationRequest(
    Guid CompanyId,
    int SystemType,
    string? TaxIdNumber,
    bool IsInclusive,
    bool IsTaxExempt = false,
    Guid? StoreId = null);

public record CreateSlabRequest(
    Guid CompanyId,
    string Name,
    string Code,
    decimal Rate,
    bool IsActive = true);

public record UpdateSlabRequest(
    string Name,
    string Code,
    decimal Rate,
    bool IsActive);

public record CreateRuleRequest(
    string ComponentName,
    decimal Rate,
    string? Jurisdiction,
    DateTime? EffectiveTo);

// ─── Controller ───────────────────────────────────────────────────────────────

[ApiController]
[Flowtap_Presentation.Authorization.RequirePermission("Settings")]
[Route("api/v1/[controller]")]
public class TaxController(
    ISender sender,
    ApplicationDbContext context,
    ITaxTemplateService taxTemplate) : ApiController(sender)
{
    private readonly ApplicationDbContext _context = context;

    // ── Tax Configuration ─────────────────────────────────────────────────────

    /// <summary>
    /// Get tax configuration. If storeId is provided, returns that store's linked config.
    /// Falls back to the company-wide config if the store has no specific config.
    /// </summary>
    [HttpGet("configuration")]
    public async Task<IActionResult> GetConfiguration(
        [FromQuery] Guid companyId,
        [FromQuery] Guid? storeId)
    {
        TaxConfiguration? config = null;
        var activeStoreId = storeId ?? CurrentStoreId;

        if (activeStoreId.HasValue && activeStoreId.Value != Guid.Empty)
        {
            var store = await _context.Stores
                .Include(s => s.TaxConfiguration)
                .FirstOrDefaultAsync(s => s.Id == activeStoreId.Value);

            config = store?.TaxConfiguration;
        }

        // Fall back to company-wide config
        config ??= await _context.TaxConfigurations
            .FirstOrDefaultAsync(t => t.CompanyId == companyId);

        return Ok(config);
    }

    /// <summary>
    /// Save or update tax configuration. If StoreId is provided, links the config to that store.
    /// </summary>
    [HttpPost("configuration")]
    [HttpPut("configuration")]
    public async Task<IActionResult> SaveConfiguration([FromBody] SaveTaxConfigurationRequest request)
    {
        TaxConfiguration? existing = null;

        // If storeId provided, try to get the store's existing config
        if (request.StoreId.HasValue)
        {
            var store = await _context.Stores
                .Include(s => s.TaxConfiguration)
                .FirstOrDefaultAsync(s => s.Id == request.StoreId.Value);

            existing = store?.TaxConfiguration;
        }

        // Fall back to company-wide config
        existing ??= await _context.TaxConfigurations
            .FirstOrDefaultAsync(t => t.CompanyId == request.CompanyId);

        if (existing == null)
        {
            existing = new TaxConfiguration
            {
                CompanyId = request.CompanyId,
                IsActive = true,
            };
            _context.TaxConfigurations.Add(existing);
        }

        existing.SystemType = (Flowtap_Domain.BoundedContexts.Core.Organization.Enums.TaxSystemType)request.SystemType;
        existing.TaxIdNumber = request.TaxIdNumber;
        existing.IsInclusive = request.IsInclusive;
        existing.IsTaxExempt = request.IsTaxExempt;

        await _context.SaveChangesAsync();

        // Link the config to the store
        if (request.StoreId.HasValue)
        {
            var store = await _context.Stores.FindAsync(request.StoreId.Value);
            if (store != null && store.TaxConfigurationId != existing.Id)
            {
                store.TaxConfigurationId = existing.Id;
                await _context.SaveChangesAsync();
            }
        }

        return Ok(new { existing.Id, existing.SystemType, existing.TaxIdNumber, existing.IsInclusive, existing.IsTaxExempt });
    }

    // ── Apply Country Template ────────────────────────────────────────────────

    /// <summary>
    /// Seeds country-appropriate tax slabs and rate rule components for a store.
    /// e.g. Canada → GST + PST/HST components; India → CGST + SGST + IGST components.
    /// </summary>
    [HttpPost("apply-template")]
    public async Task<IActionResult> ApplyTemplate(
        [FromQuery] Guid storeId,
        [FromQuery] string countryCode)
    {
        var result = await taxTemplate.ApplyTemplateAsync(storeId, countryCode);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        // After applying template, ensure store.TaxConfigurationId FK is set
        // (TaxTemplateService sets the navigation property but may not set the FK scalar)
        var store = await _context.Stores
            .Include(s => s.TaxConfiguration)
            .FirstOrDefaultAsync(s => s.Id == storeId);

        if (store?.TaxConfiguration != null && store.TaxConfigurationId != store.TaxConfiguration.Id)
        {
            store.TaxConfigurationId = store.TaxConfiguration.Id;
            await _context.SaveChangesAsync();
        }

        return Ok();
    }

    // ── Tax Slabs ─────────────────────────────────────────────────────────────

    [HttpGet("slabs")]
    public async Task<IActionResult> GetSlabs([FromQuery] Guid companyId)
    {
        var slabs = await _context.TaxSlabs
            .Include(s => s.RateRules)
            .Where(s => s.CompanyId == companyId)
            .ToListAsync();

        var result = slabs.Select(s => new
        {
            s.Id,
            s.Name,
            s.Code,
            s.IsActive,
            Rate = s.RateRules.Any() ? s.RateRules.Sum(r => r.Rate) : (decimal?)null,
            RateRules = s.RateRules.Select(r => new
            {
                r.Id,
                r.ComponentName,
                r.Rate,
                r.Jurisdiction,
                r.EffectiveFrom,
                r.EffectiveTo,
            }),
        });

        return Ok(result);
    }

    [HttpPost("slabs")]
    public async Task<IActionResult> CreateSlab([FromBody] CreateSlabRequest request)
    {
        var slab = new TaxSlab
        {
            CompanyId = request.CompanyId,
            Name = request.Name,
            Code = request.Code,
            IsActive = request.IsActive,
        };
        _context.TaxSlabs.Add(slab);
        await _context.SaveChangesAsync();

        if (request.Rate > 0)
        {
            var configId = await GetOrCreateTaxConfigId(request.CompanyId);
            _context.TaxRateRules.Add(new TaxRateRule
            {
                TaxSlabId = slab.Id,
                TaxConfigurationId = configId,
                ComponentName = request.Code,
                Rate = request.Rate,
                EffectiveFrom = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync();
        }

        return Ok(new { slab.Id, slab.Name, slab.Code, Rate = request.Rate, slab.IsActive });
    }

    [HttpPut("slabs/{id:guid}")]
    public async Task<IActionResult> UpdateSlab(Guid id, [FromBody] UpdateSlabRequest request)
    {
        var slab = await _context.TaxSlabs
            .Include(s => s.RateRules)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (slab == null) return NotFound();

        slab.Name = request.Name;
        slab.Code = request.Code;
        slab.IsActive = request.IsActive;

        // If slab has exactly one auto-generated rule (same code as slab), update its rate
        var defaultRule = slab.RateRules.FirstOrDefault(r => r.ComponentName == slab.Code || slab.RateRules.Count == 1);
        if (defaultRule != null && slab.RateRules.Count == 1)
        {
            defaultRule.ComponentName = request.Code;
            defaultRule.Rate = request.Rate;
        }
        else if (!slab.RateRules.Any() && request.Rate > 0)
        {
            var configId = await GetOrCreateTaxConfigId(slab.CompanyId);
            _context.TaxRateRules.Add(new TaxRateRule
            {
                TaxSlabId = slab.Id,
                TaxConfigurationId = configId,
                ComponentName = request.Code,
                Rate = request.Rate,
                EffectiveFrom = DateTime.UtcNow,
            });
        }
        // If slab has multiple component rules (CGST+SGST etc.), don't touch them — user manages via /rules

        await _context.SaveChangesAsync();
        var totalRate = slab.RateRules.Sum(r => r.Rate);
        return Ok(new { slab.Id, slab.Name, slab.Code, Rate = totalRate, slab.IsActive });
    }

    [HttpDelete("slabs/{id:guid}")]
    public async Task<IActionResult> DeleteSlab(Guid id)
    {
        var slab = await _context.TaxSlabs.FindAsync(id);
        if (slab == null) return NotFound();

        slab.IsActive = false;
        await _context.SaveChangesAsync();
        return Ok();
    }

    // ── Tax Rate Rules ────────────────────────────────────────────────────────

    [HttpGet("slabs/{slabId:guid}/rules")]
    public async Task<IActionResult> GetRules(Guid slabId)
    {
        var rules = await _context.TaxRateRules
            .Where(r => r.TaxSlabId == slabId)
            .ToListAsync();

        return Ok(rules);
    }

    [HttpPost("slabs/{slabId:guid}/rules")]
    public async Task<IActionResult> AddRule(Guid slabId, [FromBody] CreateRuleRequest request)
    {
        var slab = await _context.TaxSlabs.FindAsync(slabId);
        if (slab == null) return NotFound();

        var configId = await GetOrCreateTaxConfigId(slab.CompanyId);

        var rule = new TaxRateRule
        {
            TaxSlabId = slabId,
            TaxConfigurationId = configId,
            ComponentName = request.ComponentName,
            Rate = request.Rate,
            Jurisdiction = request.Jurisdiction,
            EffectiveFrom = DateTime.UtcNow,
            EffectiveTo = request.EffectiveTo,
        };
        _context.TaxRateRules.Add(rule);
        await _context.SaveChangesAsync();
        return Ok(rule);
    }

    [HttpPut("slabs/{slabId:guid}/rules/{ruleId:guid}")]
    public async Task<IActionResult> UpdateRule(Guid slabId, Guid ruleId, [FromBody] CreateRuleRequest request)
    {
        var rule = await _context.TaxRateRules
            .FirstOrDefaultAsync(r => r.Id == ruleId && r.TaxSlabId == slabId);

        if (rule == null) return NotFound();

        rule.ComponentName = request.ComponentName;
        rule.Rate = request.Rate;
        rule.Jurisdiction = request.Jurisdiction;
        rule.EffectiveTo = request.EffectiveTo;
        await _context.SaveChangesAsync();
        return Ok(rule);
    }

    [HttpDelete("slabs/{slabId:guid}/rules/{ruleId:guid}")]
    public async Task<IActionResult> DeleteRule(Guid slabId, Guid ruleId)
    {
        var rule = await _context.TaxRateRules
            .FirstOrDefaultAsync(r => r.Id == ruleId && r.TaxSlabId == slabId);

        if (rule == null) return NotFound();

        _context.TaxRateRules.Remove(rule);
        await _context.SaveChangesAsync();
        return Ok();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<Guid> GetOrCreateTaxConfigId(Guid companyId)
    {
        var config = await _context.TaxConfigurations
            .FirstOrDefaultAsync(c => c.CompanyId == companyId);

        if (config != null) return config.Id;

        var newConfig = new TaxConfiguration
        {
            CompanyId = companyId,
            SystemType = Flowtap_Domain.BoundedContexts.Core.Organization.Enums.TaxSystemType.None,
            IsActive = true,
            IsInclusive = false,
        };
        _context.TaxConfigurations.Add(newConfig);
        await _context.SaveChangesAsync();
        return newConfig.Id;
    }
}
