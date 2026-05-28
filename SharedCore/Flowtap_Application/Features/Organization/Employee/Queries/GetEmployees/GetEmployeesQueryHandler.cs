using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Organization.Employee.DTOs;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Flowtap_Domain.BoundedContexts.Core.Identity.Enums;

namespace Flowtap_Application.Features.Organization.Employee.Queries.GetEmployees;

public class GetEmployeesQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetEmployeesQuery, Result<PaginatedList<EmployeeListItemDto>>>
{
    public async Task<Result<PaginatedList<EmployeeListItemDto>>> Handle(GetEmployeesQuery request, CancellationToken ct)
    {
        var query = db.Employees
            .Include(e => e.LocationAccess)
            .Include(e => e.Permissions)
                .ThenInclude(ep => ep.Permission)
                    .ThenInclude(p => p.Category)
            .Where(e => e.CompanyId == request.CompanyId);

        var storeId = currentUser.StoreId;
        if (storeId.HasValue && storeId.Value != Guid.Empty)
        {
            query = query.Where(e => e.LocationAccess.Any(la => la.LocationId == storeId.Value) || 
                                     db.UserAccounts.Any(ua => ua.Id == e.UserAccountId && 
                                         (ua.AccountType == AccountType.Owner || ua.AccountType == AccountType.Admin)) ||
                                     e.Role == "Owner");
        }

        // Optional filters
        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(e =>
                db.UserProfiles.Any(p => p.UserAccountId == e.UserAccountId &&
                    p.Name.Contains(request.Search)) ||
                db.UserAccounts.Any(u => u.Id == e.UserAccountId &&
                    ((u.Email != null && u.Email.Contains(request.Search)) || 
                     (u.PhoneNumber != null && u.PhoneNumber.Contains(request.Search)))));

        if (!string.IsNullOrWhiteSpace(request.Role))
            query = query.Where(e => e.Role == request.Role);

        if (request.IsActive.HasValue)
        {
            var status = request.IsActive.Value ? EmployeeStatus.Active : EmployeeStatus.Suspended;
            query = query.Where(e => e.Status == status);
        }

        var total = await query.CountAsync(ct);

        var employees = await query
            .OrderBy(e => e.JobTitle)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var userAccountIds = employees.Select(e => e.UserAccountId).Distinct().ToList();

        var userAccounts = await db.UserAccounts
            .Include(u => u.Profile)
            .Where(u => userAccountIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, ct);

        // ── Build module permissions map (same logic as PinLogin) ──────────────
        var keyToCategoryName = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
        {
            { "POS",            "Sales" },
            { "Inventory",      "Inventory" },
            { "ServiceTickets", "Service & Tickets" },
            { "Purchasing",     "Purchase" },
            { "Clients",        "Sales" },
            { "Employees",      "Organization" },
            { "Reports",        "Reports & Audit" },
            { "Settings",       "Organization" },
        };

        var items = employees.Select(emp =>
        {
            userAccounts.TryGetValue(emp.UserAccountId, out var ua);
            var name  = ua?.Profile?.Name ?? "Unknown";
            var email = ua?.Email;
            var phone = ua?.PhoneNumber;

            var locationIds = emp.LocationAccess.Select(la => la.LocationId).ToList();

            var permissions = new Dictionary<string, bool>
            {
                { "POS", false }, { "Inventory", false }, { "ServiceTickets", false },
                { "Purchasing", false }, { "Clients", false }, { "Employees", false },
                { "Reports", false }, { "Settings", false },
            };

            if (emp.Permissions != null)
            {
                foreach (var ep in emp.Permissions.Where(ep => ep.IsGranted && ep.Permission?.Category != null))
                {
                    var cat = ep.Permission!.Category!.Name;
                    foreach (var kvp in keyToCategoryName.Where(k =>
                        k.Value.Equals(cat, System.StringComparison.OrdinalIgnoreCase)))
                    {
                        permissions[kvp.Key] = true;
                    }
                }
            }

            return new EmployeeListItemDto(
                emp.Id,
                emp.UserAccountId,
                name,
                email,
                phone,
                emp.Role,
                emp.JobTitle,
                emp.Department,
                emp.JoinedAt.HasValue ? emp.JoinedAt.Value.ToString("yyyy-MM-dd") : null,
                emp.Comment,
                emp.VATIN,
                emp.Salary,
                emp.SalaryType,
                emp.SalaryCurrency,
                emp.Status.ToString(),
                emp.Status == EmployeeStatus.Active,
                locationIds,
                permissions
            );
        }).ToList();

        return Result<PaginatedList<EmployeeListItemDto>>.Success(
            new PaginatedList<EmployeeListItemDto>(items, total, request.Page, request.PageSize));
    }
}
