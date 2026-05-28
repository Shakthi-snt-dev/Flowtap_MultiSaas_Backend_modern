using System.Threading;
using System.Threading.Tasks;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Organization.Employee.Commands.ResetEmployeePin;

public class ResetEmployeePinCommandHandler(IApplicationDbContext db)
    : IRequestHandler<ResetEmployeePinCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ResetEmployeePinCommand request, CancellationToken ct)
    {
        var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == request.Id, ct);
        if (employee == null) return Result<Unit>.Failure("Employee not found.");

        employee.AccessPin = request.Pin;

        await db.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}
