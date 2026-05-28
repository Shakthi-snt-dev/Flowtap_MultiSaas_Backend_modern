using System.Threading;
using System.Threading.Tasks;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Organization.Employee.Commands.UpdateEmployeeStatus;

public class UpdateEmployeeStatusCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateEmployeeStatusCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(UpdateEmployeeStatusCommand request, CancellationToken ct)
    {
        var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == request.Id, ct);
        if (employee == null) return Result<Unit>.Failure("Employee not found.");

        employee.Status = request.IsActive ? EmployeeStatus.Active : EmployeeStatus.Suspended;

        await db.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}
