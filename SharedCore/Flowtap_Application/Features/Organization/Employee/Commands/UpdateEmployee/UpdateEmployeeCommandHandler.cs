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

namespace Flowtap_Application.Features.Organization.Employee.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateEmployeeCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(UpdateEmployeeCommand request, CancellationToken ct)
    {
        var employee = await db.Employees
            .Include(e => e.LocationAccess)
            .Include(e => e.Permissions)
            .FirstOrDefaultAsync(e => e.Id == request.Id, ct);

        if (employee == null) return Result<Unit>.Failure("Employee not found.");

        // Update corresponding UserAccount & UserProfile
        var userAccount = await db.UserAccounts
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == employee.UserAccountId, ct);

        if (userAccount != null)
        {
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                userAccount.Email = request.Email.Trim().ToLower();
            }
            userAccount.PhoneNumber = request.Phone;

            if (userAccount.Profile != null)
            {
                userAccount.Profile.Name = request.Name;
                userAccount.Profile.Phone = request.Phone;
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    userAccount.Profile.Email = request.Email.Trim().ToLower();
                }
            }
            else
            {
                userAccount.Profile = new Flowtap_Domain.BoundedContexts.Core.Identity.Entities.UserProfile
                {
                    UserAccountId = userAccount.Id,
                    Name = request.Name,
                    Phone = request.Phone,
                    Email = request.Email ?? string.Empty
                };
                db.UserProfiles.Add(userAccount.Profile);
            }
        }

        // Update employee properties
        employee.Role          = request.Role;
        employee.JobTitle      = request.JobTitle;
        employee.Department    = request.Department;
        employee.Comment       = request.Comment;
        employee.VATIN         = request.Vatin;
        employee.Salary        = request.Salary;
        employee.SalaryType    = request.SalaryType;
        employee.SalaryCurrency = request.SalaryCurrency;
        if (!string.IsNullOrWhiteSpace(request.JoinedAt))
        {
            var parsedDate = DateTime.Parse(request.JoinedAt, null, System.Globalization.DateTimeStyles.RoundtripKind);
            employee.JoinedAt = parsedDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc)
                : parsedDate.ToUniversalTime();
        }
        employee.Status        = request.IsActive ? EmployeeStatus.Active : EmployeeStatus.Suspended;

        // Synchronize store locations
        // If LocationIds is null, preserve existing access (UI no longer sends this field).
        // If LocationIds is an explicit list (even empty), sync to exactly that list.
        if (request.LocationIds != null)
        {
            var toRemoveLocations = employee.LocationAccess
                .Where(la => !request.LocationIds.Contains(la.LocationId))
                .ToList();
            foreach (var la in toRemoveLocations)
                db.EmployeeLocationAccesses.Remove(la);

            foreach (var locId in request.LocationIds)
            {
                if (!employee.LocationAccess.Any(la => la.LocationId == locId))
                {
                    db.EmployeeLocationAccesses.Add(new OrgEntities.EmployeeLocationAccess
                    {
                        EmployeeId = employee.Id,
                        LocationId = locId,
                        StoreId = locId,
                        HasAccess = true
                    });
                }
            }
        }
        // else: LocationIds == null → leave existing location accesses untouched

        // Synchronize module permissions
        if (request.Permissions != null)
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
                var existingPerm = employee.Permissions.FirstOrDefault(ep => ep.PermissionId == kvp.Key);
                if (existingPerm != null)
                {
                    existingPerm.IsGranted = kvp.Value;
                }
                else
                {
                    db.EmployeePermissions.Add(new OrgEntities.EmployeePermission
                    {
                        EmployeeId = employee.Id,
                        PermissionId = kvp.Key,
                        IsGranted = kvp.Value
                    });
                }
            }
        }

        await db.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}
