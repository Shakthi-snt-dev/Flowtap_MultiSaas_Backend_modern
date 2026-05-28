using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Identity.Enums;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Organization.Employee.Commands.PinLogin;

public class PinLoginCommandHandler(IApplicationDbContext db, IJwtService jwt)
    : IRequestHandler<PinLoginCommand, Result<PinLoginResponseDto>>
{
    // All modules — used when granting full access to owners
    private static readonly string[] AllModules =
        ["POS", "Inventory", "ServiceTickets", "Purchasing", "Clients", "Employees", "Reports", "Settings"];

    public async Task<Result<PinLoginResponseDto>> Handle(PinLoginCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Pin))
            return Result<PinLoginResponseDto>.Failure("PIN is required.");

        // Find the active employee with this PIN + their permissions
        var employee = await db.Employees
            .Include(e => e.Permissions)
                .ThenInclude(ep => ep.Permission)
                    .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(e =>
                e.CompanyId == request.CompanyId &&
                e.Status == EmployeeStatus.Active &&
                e.AccessPin == request.Pin, ct);

        if (employee is null)
            return Result<PinLoginResponseDto>.Failure("Invalid PIN. Please try again.");

        // Load the linked user account for token generation
        var userAccount = await db.UserAccounts
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == employee.UserAccountId, ct);

        if (userAccount is null)
            return Result<PinLoginResponseDto>.Failure("Employee account not found.");

        var name = userAccount.Profile?.Name ?? "Employee";

        // Derive initials
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var initials = parts.Length >= 2
            ? $"{parts[0][0]}{parts[^1][0]}"
            : name.Length > 0 ? name[..1] : "?";

        // ── Determine if this is an owner/admin PIN login ────────────────────────
        var isOwner = userAccount.AccountType is AccountType.Owner or AccountType.Admin;

        Dictionary<string, bool> permissions;
        List<System.Security.Claims.Claim> permissionClaims;
        string role;

        if (isOwner)
        {
            // Owner / Admin → full unrestricted access on all modules
            permissions = AllModules.ToDictionary(m => m, _ => true);

            // No "isEmployee" claim → PermissionContext treats this as a full owner session
            permissionClaims = [];
            role = userAccount.AccountType.ToString();
        }
        else
        {
            // Regular employee → only their granted module permissions
            permissions = new Dictionary<string, bool>
            {
                { "POS",            false },
                { "Inventory",      false },
                { "ServiceTickets", false },
                { "Purchasing",     false },
                { "Clients",        false },
                { "Employees",      false },
                { "Reports",        false },
                { "Settings",       false },
            };

            foreach (var ep in employee.Permissions ?? [])
            {
                if (ep.Permission?.Category == null || !ep.IsGranted) continue;
                var cat = ep.Permission.Category.Name;
                if (cat.Equals("Sales", StringComparison.OrdinalIgnoreCase))
                    { permissions["POS"] = true; permissions["Clients"] = true; }
                else if (cat.Equals("Inventory", StringComparison.OrdinalIgnoreCase))
                    permissions["Inventory"] = true;
                else if (cat.Equals("Service & Tickets", StringComparison.OrdinalIgnoreCase))
                    permissions["ServiceTickets"] = true;
                else if (cat.Equals("Purchase", StringComparison.OrdinalIgnoreCase))
                    permissions["Purchasing"] = true;
                else if (cat.Equals("Organization", StringComparison.OrdinalIgnoreCase))
                    { permissions["Employees"] = true; permissions["Settings"] = true; }
                else if (cat.Equals("Reports & Audit", StringComparison.OrdinalIgnoreCase))
                    permissions["Reports"] = true;
            }

            // Embed granted permissions as JWT claims so backend can enforce them
            permissionClaims = permissions
                .Where(kvp => kvp.Value)
                .Select(kvp => new System.Security.Claims.Claim("permission", kvp.Key))
                .ToList();

            // "isEmployee" claim signals that this token has restricted module access
            permissionClaims.Add(new System.Security.Claims.Claim("isEmployee", "true"));

            role = employee.Role ?? employee.JobTitle ?? "Employee";
        }

        var token = jwt.GenerateAccessToken(
            userAccount.Id,
            request.CompanyId,
            userAccount.Email,
            [role],
            permissionClaims);

        return Result<PinLoginResponseDto>.Success(new PinLoginResponseDto(
            token,
            employee.Id,
            userAccount.Id,
            name,
            userAccount.Email,
            employee.JobTitle,
            initials.ToUpper(),
            permissions,
            isOwner,
            employee.DefaultLocationId));
    }
}
