using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Organization.Employee.Queries.VerifyEmployeePin;

public class VerifyEmployeePinQueryHandler(IApplicationDbContext db)
    : IRequestHandler<VerifyEmployeePinQuery, Result<CashierDto>>
{
    public async Task<Result<CashierDto>> Handle(VerifyEmployeePinQuery request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Pin))
            return Result<CashierDto>.Failure("PIN is required.");

        // Look up active employee with permissions included
        var employee = await db.Employees
            .Include(e => e.Permissions)
                .ThenInclude(ep => ep.Permission)
                    .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(e =>
                e.CompanyId == request.CompanyId &&
                e.Status == EmployeeStatus.Active &&
                e.AccessPin == request.Pin, ct);

        if (employee is null)
            return Result<CashierDto>.Failure("Invalid PIN. Please try again.");

        // Load name from profile
        var profile = await db.UserProfiles
            .FirstOrDefaultAsync(p => p.UserAccountId == employee.UserAccountId, ct);
        var name = profile?.Name ?? "Employee";

        // Derive initials
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var initials = parts.Length >= 2
            ? $"{parts[0][0]}{parts[^1][0]}"
            : name.Length > 0 ? name[..1] : "?";

        // Build module-level permissions map (same logic as GetEmployeeQueryHandler)
        var permissions = new Dictionary<string, bool>
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

        return Result<CashierDto>.Success(
            new CashierDto(employee.Id, name, employee.JobTitle, initials.ToUpper(), permissions));
    }
}
