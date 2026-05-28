using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrgEntities = Flowtap_Domain.BoundedContexts.Core.Organization.Entities;

namespace Flowtap_Application.Features.Organization.Tenant.Commands.CreateTenant;

public class CreateTenantCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser,
    ITaxTemplateService taxTemplate)
    : IRequestHandler<CreateTenantCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTenantCommand request, CancellationToken ct)
    {
        if (currentUser.UserId is null) throw new UnauthorizedException();

        var subdomainTaken = await db.Tenants.AnyAsync(t => t.SubDomain == request.SubDomain.ToLower(), ct);
        if (subdomainTaken) return Result<Guid>.Failure("This subdomain is already taken.");

        if (!Enum.TryParse<IndustryType>(request.IndustryType, true, out var industry))
            return Result<Guid>.Failure($"Invalid industry type: {request.IndustryType}");

        var tenant = new OrgEntities.Tenant
        {
            OwnerId = currentUser.UserId.Value,
            Title = request.Title,
            Phone = request.Phone,
            Email = request.Email,
            Country = request.Country,
            Currency = request.Currency,
            SubDomain = request.SubDomain.ToLower(),
            BusinessType = request.BusinessType,
            IndustryType = industry,
            Settings = new OrgEntities.TenantSettings { IsOnboardingComplete = false }
        };

        db.Tenants.Add(tenant);

        // Link owner as AppUser
        var appUser = await db.AppUsers.FirstOrDefaultAsync(a => a.UserAccountId == currentUser.UserId, ct);
        if (appUser is null)
        {
            db.AppUsers.Add(new OrgEntities.AppUser { UserAccountId = currentUser.UserId.Value, CompanyId = tenant.Id });
        }
        else
        {
            appUser.CompanyId = tenant.Id;
        }

        // Start trial
        var trial = new Flowtap_Domain.BoundedContexts.Modules.Subscription.Entities.TrialPlan
        {
            CompanyId = tenant.Id,
            TrialStartDate = DateTime.UtcNow,
            TrialEndDate = DateTime.UtcNow.AddDays(30),
            LocationCount = 1
        };
        db.TrialPlans.Add(trial);

        // ── Save Tenant first so Store FK (TenantId) can reference it ──────────
        await db.SaveChangesAsync(ct);

        // Create default Store (must come after Tenant is persisted)
        var countryCode = request.Country.Length == 2 ? request.Country.ToUpper() : "US";
        var currencyCode = request.Currency.Length == 3 ? request.Currency.ToUpper() : "USD";

        var store = new OrgEntities.Store
        {
            CompanyId = tenant.Id,
            Tenant = tenant,             // wire navigation so EF resolves TenantId shadow FK
            Title = request.Title,
            Phone = request.Phone,
            Address = string.Empty,
            CountryCode = countryCode,
            CurrencyCode = currencyCode,
            TimeZoneId = "UTC",
            DefaultOrderType = Guid.Empty
        };
        db.Stores.Add(store);

        // Create default Warehouse for the store
        var warehouse = new Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities.Warehouse
        {
            CompanyId = tenant.Id,
            LocationId = store.Id,   // link warehouse to the default store
            Name = "Main Store — Stock Room",
            Code = "WH-MAIN",
            Type = WarehouseType.InStore,
            Status = WarehouseStatus.Active,
            IsActive = true,
            City = string.Empty,
            State = string.Empty,
            Country = countryCode,
            PostalCode = string.Empty
        };
        db.Warehouses.Add(warehouse);

        await db.SaveChangesAsync(ct);

        // Apply tax template based on country
        await taxTemplate.ApplyTemplateAsync(store.Id, countryCode, ct);

        return Result<Guid>.Success(tenant.Id);
    }
}
