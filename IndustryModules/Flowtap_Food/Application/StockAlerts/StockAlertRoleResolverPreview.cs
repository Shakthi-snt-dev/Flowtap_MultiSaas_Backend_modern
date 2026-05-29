using Flowtap_Application.Common.Interfaces;
using Flowtap_Food.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Application.StockAlerts;

/// <summary>
/// Diagnostic version of the role resolver.
/// Returns human-readable contact info + warnings explaining why some contacts are missing.
/// Used by the /preview-recipients endpoint — does NOT send anything.
/// </summary>
public static class StockAlertRoleResolverPreview
{
    public record PreviewContact(
        string Role,
        string? Email,
        string? Phone,
        string Status);   // "OK" | "NoContact" | "NoManager" | "NoAccount"

    public static async Task<List<PreviewContact>> ResolveWithDiagnosticsAsync(
        StockAlertRule rule,
        IApplicationDbContext coreDb,
        List<string> warnings,
        CancellationToken ct)
    {
        var result = new List<PreviewContact>();
        var roles  = rule.GetNotifyRoles().ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (roles.Count == 0)
        {
            warnings.Add("No roles selected — edit the rule and check the 'Who should be notified?' roles.");
            return result;
        }

        var warehouse = await coreDb.Warehouses.AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == rule.WarehouseId, ct);

        Guid? storeId = warehouse?.LocationId;

        // ── Owner ─────────────────────────────────────────────────────────────
        if (roles.Contains("Owner"))
        {
            var tenant = await coreDb.Tenants.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == rule.CompanyId, ct);
            if (tenant is null)
            {
                warnings.Add("Owner: Tenant record not found.");
            }
            else
            {
                var (oEmail, oPhone) = await GetContactAsync(coreDb, tenant.OwnerId, ct);
                if (string.IsNullOrWhiteSpace(oEmail) && string.IsNullOrWhiteSpace(oPhone))
                    warnings.Add("Owner: No Email or Phone found in UserAccount or UserProfile — add contact info in Settings → Profile.");
                else
                    result.Add(new PreviewContact("Owner", oEmail, oPhone, "OK"));
            }
        }

        // ── WHAdmin:{id} ──────────────────────────────────────────────────────
        foreach (var role in roles.Where(r => r.StartsWith("WHAdmin:", StringComparison.OrdinalIgnoreCase)))
        {
            var idPart = role["WHAdmin:".Length..];
            if (!Guid.TryParse(idPart, out var whId)) { warnings.Add($"Invalid role format: {role}"); continue; }

            var wh = await coreDb.Warehouses.AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == whId, ct);
            if (wh is null) { warnings.Add($"Warehouse {whId} not found."); continue; }

            if (wh.ManagerEmployeeId is null)
            {
                warnings.Add($"'{wh.Name}': No manager assigned — Inventory → Warehouses → Edit → set Manager Employee.");
                result.Add(new PreviewContact($"{wh.Name} Admin", null, null, "NoManager"));
                continue;
            }

            var (wEmail, wPhone) = await GetContactByEmpAsync(coreDb, wh.ManagerEmployeeId.Value, ct);
            if (string.IsNullOrWhiteSpace(wEmail) && string.IsNullOrWhiteSpace(wPhone))
            {
                warnings.Add($"'{wh.Name}': Manager has no Email or Phone in UserAccount or UserProfile.");
                result.Add(new PreviewContact($"{wh.Name} Admin", null, null, "NoContact"));
                continue;
            }

            result.Add(new PreviewContact($"{wh.Name} Admin", wEmail, wPhone, "OK"));
        }

        // ── Chef / Manager / other role-based employees ───────────────────────
        var empRoles = roles.Where(r =>
            !r.StartsWith("WHAdmin:", StringComparison.OrdinalIgnoreCase) &&
            !r.Equals("Owner", StringComparison.OrdinalIgnoreCase) &&
            !r.Equals("WarehouseManager", StringComparison.OrdinalIgnoreCase) &&
            !r.Equals("InStoreWarehouseManager", StringComparison.OrdinalIgnoreCase) &&
            !r.Equals("KitchenWarehouseManager", StringComparison.OrdinalIgnoreCase) &&
            !r.Equals("StoreManager", StringComparison.OrdinalIgnoreCase)).ToList();

        if (empRoles.Count > 0)
        {
            if (storeId is null)
            {
                warnings.Add($"Role-based lookup ({string.Join(", ", empRoles)}): warehouse has no LinkedStore — assign warehouse to a store first.");
            }
            else
            {
                var empIds = await coreDb.EmployeeLocationAccesses.AsNoTracking()
                    .Where(la => la.StoreId == storeId.Value && la.HasAccess)
                    .Select(la => la.EmployeeId).ToListAsync(ct);

                var matched = await coreDb.Employees.AsNoTracking()
                    .Where(e => empIds.Contains(e.Id) && e.Role != null && empRoles.Contains(e.Role))
                    .ToListAsync(ct);

                if (matched.Count == 0)
                    warnings.Add($"No employees with role(s) '{string.Join(", ", empRoles)}' found at this store.");

                foreach (var emp in matched)
                {
                    var acct = await coreDb.UserAccounts.AsNoTracking()
                        .FirstOrDefaultAsync(u => u.Id == emp.UserAccountId, ct);
                    // Check both UserAccount and UserProfile
                    var (eEmail, ePhone) = await GetContactByEmpAsync(coreDb, emp.Id, ct);
                    if (string.IsNullOrWhiteSpace(eEmail) && string.IsNullOrWhiteSpace(ePhone))
                    {
                        warnings.Add($"{emp.Role} employee has no Email or Phone in UserAccount or UserProfile.");
                        result.Add(new PreviewContact(emp.Role!, null, null, "NoContact"));
                    }
                    else
                        result.Add(new PreviewContact(emp.Role!, eEmail, ePhone, "OK"));
                }
            }
        }

        return result;
    }

    // ── Contact helpers — check both UserAccount (login) and UserProfile (display) ──

    private static async Task<(string? Email, string? Phone)> GetContactAsync(
        IApplicationDbContext db, Guid userId, CancellationToken ct)
    {
        var account = await db.UserAccounts.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
        var profile = await db.UserProfiles.AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserAccountId == userId, ct);

        var email = !string.IsNullOrWhiteSpace(profile?.Email)   ? profile!.Email
                  : !string.IsNullOrWhiteSpace(account?.Email)   ? account!.Email
                  : null;
        var phone = !string.IsNullOrWhiteSpace(profile?.Phone)       ? profile!.Phone
                  : !string.IsNullOrWhiteSpace(account?.PhoneNumber) ? account!.PhoneNumber
                  : null;
        return (email, phone);
    }

    private static async Task<(string? Email, string? Phone)> GetContactByEmpAsync(
        IApplicationDbContext db, Guid empId, CancellationToken ct)
    {
        var emp = await db.Employees.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == empId, ct);
        if (emp is null) return (null, null);
        return await GetContactAsync(db, emp.UserAccountId, ct);
    }
}
