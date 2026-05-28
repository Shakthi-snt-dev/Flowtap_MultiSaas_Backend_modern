using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Organization.Employee.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Organization.Employee.Queries.GetEmployee;

public class GetEmployeeQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetEmployeeQuery, Result<EmployeeDto>>
{
    public async Task<Result<EmployeeDto>> Handle(GetEmployeeQuery request, CancellationToken ct)
    {
        var employee = await db.Employees
            .Include(e => e.LocationAccess)
            .Include(e => e.Permissions)
                .ThenInclude(ep => ep.Permission)
                    .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(e => e.Id == request.Id, ct);

        if (employee == null) return Result<EmployeeDto>.Failure("Employee not found.");

        var userAccount = await db.UserAccounts
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == employee.UserAccountId, ct);

        if (userAccount == null) return Result<EmployeeDto>.Failure("Linked user account not found.");

        var name = userAccount.Profile?.Name ?? "Unknown";
        var phone = userAccount.PhoneNumber;
        var email = userAccount.Email;

        var locationIds = employee.LocationAccess.Select(la => la.LocationId).ToList();
        
        var permissions = new Dictionary<string, bool>
        {
            { "POS",            false },
            { "Inventory",      false },
            { "ServiceTickets", false },
            { "Purchasing",     false },
            { "Clients",        false },
            { "Employees",      false },
            { "Reports",        false },
            { "Settings",       false }
        };

        if (employee.Permissions != null)
        {
            foreach (var ep in employee.Permissions)
            {
                if (ep.Permission != null && ep.Permission.Category != null && ep.IsGranted)
                {
                    var categoryName = ep.Permission.Category.Name;

                    if (categoryName.Equals("Sales", StringComparison.OrdinalIgnoreCase))
                    {
                        permissions["POS"] = true;
                        permissions["Clients"] = true;
                    }
                    else if (categoryName.Equals("Inventory", StringComparison.OrdinalIgnoreCase))
                    {
                        permissions["Inventory"] = true;
                    }
                    else if (categoryName.Equals("Service & Tickets", StringComparison.OrdinalIgnoreCase))
                    {
                        permissions["ServiceTickets"] = true;
                    }
                    else if (categoryName.Equals("Purchase", StringComparison.OrdinalIgnoreCase))
                    {
                        permissions["Purchasing"] = true;
                    }
                    else if (categoryName.Equals("Organization", StringComparison.OrdinalIgnoreCase))
                    {
                        permissions["Employees"] = true;
                        permissions["Settings"] = true;
                    }
                    else if (categoryName.Equals("Reports & Audit", StringComparison.OrdinalIgnoreCase))
                    {
                        permissions["Reports"] = true;
                    }
                }
            }
        }

        var dto = new EmployeeDto(
            employee.Id,
            employee.UserAccountId,
            employee.CompanyId,
            name,
            email,
            phone,
            employee.Role,
            employee.JobTitle,
            employee.Department,
            employee.JoinedAt.HasValue ? employee.JoinedAt.Value.ToString("yyyy-MM-dd") : null,
            employee.Comment,
            employee.VATIN,
            employee.Salary,
            employee.SalaryType,
            employee.SalaryCurrency,
            employee.Status.ToString(),
            employee.Status == Flowtap_Domain.BoundedContexts.Core.Organization.Enums.EmployeeStatus.Active,
            employee.AccessPin,
            locationIds,
            permissions
        );

        return Result<EmployeeDto>.Success(dto);
    }
}
