using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Identity.Entities;
using Flowtap_Food.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flowtap_Food.Application.StockAlerts;

/// <summary>Contact info merged from UserAccount (login) + UserProfile (display).</summary>
internal record UserContact(string? Email, string? Phone);

/// <summary>
/// Resolves notification contacts for a StockAlertRule from its NotifyRoles list.
///
/// Supported role values (stored in StockAlertRule.NotifyRoles, comma-separated):
///
///   Owner            → Tenant.OwnerId → UserAccount
///   Chef             → all Employees where Role="Chef"  at the linked store
///   Manager          → all Employees where Role="Manager" at the linked store
///   Admin            → all Employees where Role="Admin" at the linked store
///   WarehouseManager → Warehouse.ManagerEmployeeId → Employee → UserAccount
///   StoreManager     → Store.ManagerEmployeeId → Employee → UserAccount
///
/// Per-person channel routing:
///   Has Email  → receives Email notification (if SendEmail is on)
///   Has Phone  → receives SMS  notification (if SendSms is on)
///   Has Phone  → receives WhatsApp notification (if SendWhatsApp is on)
///   WhatsApp runs on the same phone number — no separate field needed.
/// </summary>
public static class StockAlertRoleResolver
{
    private static readonly ILogger Logger =
        LoggerFactory.Create(b => b.AddConsole()).CreateLogger(nameof(StockAlertRoleResolver));

    public static async Task<ResolvedContacts> ResolveAsync(
        StockAlertRule rule,
        IApplicationDbContext coreDb,
        CancellationToken ct)
    {
        var roles = rule.GetNotifyRoles().ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (roles.Count == 0)
        {
            Logger.LogWarning("StockAlertRule {RuleId}: NotifyRoles is empty — nobody will be notified", rule.Id);
            return new ResolvedContacts([], []);
        }

        var acc = new ContactAccumulator();

        // Warehouse → Store link needed for employee lookups
        var warehouse = await coreDb.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == rule.WarehouseId, ct);

        Guid? storeId = warehouse?.LocationId;

        // ── Owner ─────────────────────────────────────────────────────────────
        if (roles.Contains("Owner"))
        {
            var tenant = await coreDb.Tenants.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == rule.CompanyId, ct);
            if (tenant is null)
            {
                Logger.LogWarning("Owner lookup: Tenant not found for CompanyId={CompanyId}", rule.CompanyId);
            }
            else
            {
                var ownerContact = await GetContactByUserIdAsync(coreDb, tenant.OwnerId, ct);
                if (string.IsNullOrWhiteSpace(ownerContact.Email) && string.IsNullOrWhiteSpace(ownerContact.Phone))
                    Logger.LogWarning("Owner: no Email or Phone found in UserAccount or UserProfile");
                else
                    Logger.LogInformation("Owner resolved: Email={Email} Phone={Phone}",
                        ownerContact.Email ?? "none", ownerContact.Phone ?? "none");
                acc.Add(ownerContact, "Owner");
            }
        }

        // ── WHAdmin:{warehouseId} — manager of a specific warehouse by ID ──────
        foreach (var role in roles.Where(r => r.StartsWith("WHAdmin:", StringComparison.OrdinalIgnoreCase)))
        {
            var idPart = role["WHAdmin:".Length..];
            if (!Guid.TryParse(idPart, out var whId)) continue;

            var wh = await coreDb.Warehouses.AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == whId, ct);

            if (wh is null) { Logger.LogWarning("WHAdmin: Warehouse {Id} not found", whId); continue; }
            if (wh.ManagerEmployeeId is null)
            {
                Logger.LogWarning("WHAdmin: Warehouse '{Name}' has no Manager assigned", wh.Name);
                continue;
            }

            var contact = await GetContactByEmployeeAsync(coreDb, wh.ManagerEmployeeId.Value, ct);
            if (string.IsNullOrWhiteSpace(contact.Email) && string.IsNullOrWhiteSpace(contact.Phone))
            {
                Logger.LogWarning("WHAdmin: Manager of '{Name}' has no Email or Phone in UserAccount or UserProfile", wh.Name);
                continue;
            }
            acc.Add(contact, $"{wh.Name} Admin");
        }

        // ── Legacy type-based roles ───────────────────────────────────────────
        if (roles.Contains("WarehouseManager") && warehouse?.ManagerEmployeeId is not null)
            acc.Add(await GetContactByEmployeeAsync(coreDb, warehouse.ManagerEmployeeId.Value, ct), "Warehouse Manager");

        if (roles.Contains("InStoreWarehouseManager") && storeId is not null)
        {
            var inStoreWh = await coreDb.Warehouses.AsNoTracking()
                .FirstOrDefaultAsync(w =>
                    w.LocationId == storeId.Value &&
                    w.Type == Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums.WarehouseType.InStore, ct);
            if (inStoreWh?.ManagerEmployeeId is not null)
                acc.Add(await GetContactByEmployeeAsync(coreDb, inStoreWh.ManagerEmployeeId.Value, ct), "InStore Warehouse Admin");
        }

        if (roles.Contains("KitchenWarehouseManager") && storeId is not null)
        {
            var kitchenWh = await coreDb.Warehouses.AsNoTracking()
                .FirstOrDefaultAsync(w =>
                    w.LocationId == storeId.Value &&
                    w.Type == Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums.WarehouseType.KitchenStore, ct);
            if (kitchenWh?.ManagerEmployeeId is not null)
                acc.Add(await GetContactByEmployeeAsync(coreDb, kitchenWh.ManagerEmployeeId.Value, ct), "Kitchen Warehouse Admin");
        }

        // ── Role-based employee lookup (Chef / Manager / any custom role) ─────
        var employeeRolesToQuery = roles
            .Where(r => !IsStructuralRole(r))
            .ToList();

        if (employeeRolesToQuery.Count > 0 && storeId is not null)
        {
            // Find employees with any of the specified roles who have access to this store
            var employeeIds = await coreDb.EmployeeLocationAccesses
                .AsNoTracking()
                .Where(la => la.StoreId == storeId.Value && la.HasAccess)
                .Select(la => la.EmployeeId)
                .ToListAsync(ct);

            if (employeeIds.Count > 0)
            {
                var matchedEmployees = await coreDb.Employees
                    .AsNoTracking()
                    .Where(e => employeeIds.Contains(e.Id) &&
                                e.Role != null &&
                                employeeRolesToQuery.Contains(e.Role))
                    .ToListAsync(ct);

                var userAccountIds = matchedEmployees
                    .Select(e => e.UserAccountId)
                    .Distinct()
                    .ToList();

                var accounts = await coreDb.UserAccounts
                    .AsNoTracking()
                    .Where(u => userAccountIds.Contains(u.Id))
                    .ToListAsync(ct);

                foreach (var emp in matchedEmployees)
                {
                    var contact = await GetContactByEmployeeAsync(coreDb, emp.Id, ct);
                    acc.Add(contact, emp.Role ?? "Staff");
                }
            }
        }

        return acc.Build();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Roles resolved via entity FK lookups — not via Employee.Role string matching.</summary>
    private static bool IsStructuralRole(string role) =>
        role.StartsWith("WHAdmin:",              StringComparison.OrdinalIgnoreCase) ||
        role.Equals("Owner",                    StringComparison.OrdinalIgnoreCase) ||
        role.Equals("WarehouseManager",         StringComparison.OrdinalIgnoreCase) ||
        role.Equals("InStoreWarehouseManager",  StringComparison.OrdinalIgnoreCase) ||
        role.Equals("KitchenWarehouseManager",  StringComparison.OrdinalIgnoreCase) ||
        role.Equals("StoreManager",             StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Resolves contact info for an employee.
    /// Checks BOTH UserAccount (login credentials) AND UserProfile (display contact info)
    /// because different registration flows populate different fields.
    /// </summary>
    private static async Task<UserContact> GetContactByEmployeeAsync(
        IApplicationDbContext coreDb, Guid employeeId, CancellationToken ct)
    {
        var employee = await coreDb.Employees.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == employeeId, ct);
        if (employee is null) return new UserContact(null, null);
        return await GetContactByUserIdAsync(coreDb, employee.UserAccountId, ct);
    }

    /// <summary>
    /// Resolves contact info for a user — merges UserAccount and UserProfile.
    /// UserAccount holds the login email; UserProfile holds the display email + phone.
    /// </summary>
    private static async Task<UserContact> GetContactByUserIdAsync(
        IApplicationDbContext coreDb, Guid userId, CancellationToken ct)
    {
        var account = await coreDb.UserAccounts.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        var profile = await coreDb.UserProfiles.AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserAccountId == userId, ct);

        // Prefer profile email/phone (set by admin when adding employee)
        // Fall back to account email/phone (set during self-registration)
        var email = !string.IsNullOrWhiteSpace(profile?.Email)   ? profile!.Email
                  : !string.IsNullOrWhiteSpace(account?.Email)   ? account!.Email
                  : null;

        var phone = !string.IsNullOrWhiteSpace(profile?.Phone)         ? profile!.Phone
                  : !string.IsNullOrWhiteSpace(account?.PhoneNumber)   ? account!.PhoneNumber
                  : null;

        return new UserContact(email, phone);
    }
}

// ── Contact accumulator ───────────────────────────────────────────────────────

internal class ContactAccumulator
{
    // address → role label (deduplication by address)
    private readonly Dictionary<string, string> _emails = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _phones = new(StringComparer.OrdinalIgnoreCase);

    public void Add(UserContact contact, string roleLabel)
    {
        if (!string.IsNullOrWhiteSpace(contact.Email))
            _emails[contact.Email.Trim()] = roleLabel;
        if (!string.IsNullOrWhiteSpace(contact.Phone))
            _phones[contact.Phone.Trim()] = roleLabel;
    }

    public ResolvedContacts Build() => new(
        [.. _emails.Select(kv => new ResolvedContact(kv.Key, kv.Value))],
        [.. _phones.Select(kv => new ResolvedContact(kv.Key, kv.Value))]);
}

/// <summary>A resolved contact with the role that produced it.</summary>
public record ResolvedContact(string Address, string RoleLabel);

/// <param name="Emails">De-duplicated email contacts with their role labels.</param>
/// <param name="Phones">De-duplicated phone contacts — used for both SMS and WhatsApp.</param>
public record ResolvedContacts(
    IReadOnlyList<ResolvedContact> Emails,
    IReadOnlyList<ResolvedContact> Phones);
