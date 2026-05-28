using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Organization.Employee.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteEmployeeCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteEmployeeCommand request, CancellationToken ct)
    {
        var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == request.Id, ct);
        if (employee == null) return Result<Unit>.Failure("Employee not found.");

        // Explicitly clean up related join records to prevent database constraint errors
        var locations = await db.EmployeeLocationAccesses.Where(la => la.EmployeeId == employee.Id).ToListAsync(ct);
        db.EmployeeLocationAccesses.RemoveRange(locations);

        var permissions = await db.EmployeePermissions.Where(ep => ep.EmployeeId == employee.Id).ToListAsync(ct);
        db.EmployeePermissions.RemoveRange(permissions);

        db.Employees.Remove(employee);

        await db.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}
