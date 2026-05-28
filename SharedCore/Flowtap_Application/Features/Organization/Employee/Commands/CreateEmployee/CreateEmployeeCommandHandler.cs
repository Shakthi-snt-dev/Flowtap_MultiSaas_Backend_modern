using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrgEntities = Flowtap_Domain.BoundedContexts.Core.Organization.Entities;

namespace Flowtap_Application.Features.Organization.Employee.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateEmployeeCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateEmployeeCommand request, CancellationToken ct)
    {
        var email = request.Email?.Trim().ToLower();
        var userAccount = await db.UserAccounts
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == email, ct);

        if (userAccount == null)
        {
            userAccount = new Flowtap_Domain.BoundedContexts.Core.Identity.Entities.UserAccount
            {
                Email = email ?? $"{Guid.NewGuid()}@flowtap.local",
                PhoneNumber = request.Phone,
                IsEmailVerified = true,
                AccountType = Flowtap_Domain.BoundedContexts.Core.Identity.Enums.AccountType.Staff
            };
            db.UserAccounts.Add(userAccount);

            var profile = new Flowtap_Domain.BoundedContexts.Core.Identity.Entities.UserProfile
            {
                UserAccount = userAccount,
                Name = request.Name,
                Phone = request.Phone,
                Email = email ?? string.Empty
            };
            db.UserProfiles.Add(profile);
            
            // Save newly created user account so we have a valid Guid for joins
            await db.SaveChangesAsync(ct);
        }

        var existing = await db.Employees
            .AnyAsync(e => e.UserAccountId == userAccount.Id && e.CompanyId == request.CompanyId, ct);
        if (existing) return Result<Guid>.Failure("User is already an employee of this company.");

        var employee = new OrgEntities.Employee
        {
            UserAccountId   = userAccount.Id,
            CompanyId       = request.CompanyId,
            Role            = request.Role,
            JobTitle        = request.JobTitle,
            Department      = request.Department,
            JoinedAt        = !string.IsNullOrWhiteSpace(request.JoinedAt)
                                ? DateTime.Parse(request.JoinedAt, null, System.Globalization.DateTimeStyles.RoundtripKind).ToUniversalTime()
                                : DateTime.UtcNow,
            Comment         = request.Comment,
            VATIN           = request.Vatin,
            Salary          = request.Salary,
            SalaryType      = request.SalaryType,
            SalaryCurrency  = request.SalaryCurrency,
            AccessPin       = request.Pin,
            Status          = request.IsActive ? EmployeeStatus.Active : EmployeeStatus.Suspended
        };

        db.Employees.Add(employee);

        // Map location (store) accesses — if none supplied, auto-assign the default (first active) store
        if (request.LocationIds != null && request.LocationIds.Any())
        {
            foreach (var locId in request.LocationIds)
            {
                db.EmployeeLocationAccesses.Add(new OrgEntities.EmployeeLocationAccess
                {
                    Employee = employee,
                    LocationId = locId,
                    StoreId = locId,
                    HasAccess = true
                });
            }
        }
        else
        {
            var defaultStore = await db.Stores
                .Where(s => s.CompanyId == request.CompanyId && s.IsActive)
                .OrderBy(s => s.CreatedAt)
                .FirstOrDefaultAsync(ct);

            if (defaultStore != null)
            {
                db.EmployeeLocationAccesses.Add(new OrgEntities.EmployeeLocationAccess
                {
                    Employee = employee,
                    LocationId = defaultStore.Id,
                    StoreId = defaultStore.Id,
                    HasAccess = true
                });
            }
        }

        // Map granular permissions
        if (request.Permissions != null && request.Permissions.Any())
        {
            var keyToCategoryName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "POS",            "Sales" },
                { "Inventory",      "Inventory" },
                { "ServiceTickets", "Service & Tickets" },
                { "Purchasing",     "Purchase" },
                { "Clients",        "Sales" },
                { "Employees",      "Organization" },
                { "Reports",        "Reports & Audit" },
                { "Settings",       "Organization" }
            };

            var allPermissions = await db.Permissions
                .Include(p => p.Category)
                .ToListAsync(ct);

            var resolvedPermissions = new Dictionary<Guid, bool>();

            foreach (var kvp in request.Permissions)
            {
                if (keyToCategoryName.TryGetValue(kvp.Key, out var categoryName))
                {
                    var categoryPerms = allPermissions
                        .Where(p => p.Category.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

                    foreach (var p in categoryPerms)
                    {
                        if (resolvedPermissions.TryGetValue(p.Id, out var existingVal))
                        {
                            resolvedPermissions[p.Id] = existingVal || kvp.Value;
                        }
                        else
                        {
                            resolvedPermissions[p.Id] = kvp.Value;
                        }
                    }
                }
                else
                {
                    var perm = allPermissions.FirstOrDefault(p => p.Key.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase));
                    if (perm != null)
                    {
                        resolvedPermissions[perm.Id] = kvp.Value;
                    }
                }
            }

            foreach (var kvp in resolvedPermissions)
            {
                db.EmployeePermissions.Add(new OrgEntities.EmployeePermission
                {
                    Employee = employee,
                    PermissionId = kvp.Key,
                    IsGranted = kvp.Value
                });
            }
        }

        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(employee.Id);
    }
}
