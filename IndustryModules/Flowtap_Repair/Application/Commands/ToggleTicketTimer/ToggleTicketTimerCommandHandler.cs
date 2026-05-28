using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Commands.ToggleTicketTimer;

public class ToggleTicketTimerCommandHandler(IRepairDbContext db, IDateTimeService dateTime)
    : IRequestHandler<ToggleTicketTimerCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ToggleTicketTimerCommand request, CancellationToken ct)
    {
        var activeLog = await db.TicketTimeLogs
            .FirstOrDefaultAsync(l => l.TicketId == request.TicketId
                && l.StartedByEmployeeId == request.EmployeeId
                && l.StoppedAt == null, ct);

        if (activeLog is not null)
        {
            activeLog.StoppedAt = dateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            return Result<bool>.Success(false); // false means timer stopped
        }

        var ticket = await db.ServiceTickets
            .FirstOrDefaultAsync(t => t.Id == request.TicketId && t.CompanyId == request.CompanyId, ct);

        if (ticket is null)
            return Result<bool>.Failure("Ticket not found.");

        var newLog = new TicketTimeLog
        {
            TicketId = request.TicketId,
            StartedByEmployeeId = request.EmployeeId,
            StartedAt = dateTime.UtcNow
        };

        db.TicketTimeLogs.Add(newLog);
        await db.SaveChangesAsync(ct);

        return Result<bool>.Success(true); // true means timer started
    }
}

