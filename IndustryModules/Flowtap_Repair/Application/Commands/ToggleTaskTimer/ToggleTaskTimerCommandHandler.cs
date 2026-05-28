using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Commands.ToggleTaskTimer;

public class ToggleTaskTimerCommandHandler(IRepairDbContext db, IDateTimeService dateTime)
    : IRequestHandler<ToggleTaskTimerCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ToggleTaskTimerCommand request, CancellationToken ct)
    {
        var activeLog = await db.TaskTimeLogs
            .FirstOrDefaultAsync(l => l.TaskId == request.TaskId
                && l.EmployeeId == request.EmployeeId
                && l.StoppedAt == null, ct);

        if (activeLog is not null)
        {
            activeLog.StoppedAt = dateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            return Result<bool>.Success(false); // false means timer stopped
        }

        var task = await db.WorkTasks
            .FirstOrDefaultAsync(t => t.Id == request.TaskId && t.CompanyId == request.CompanyId, ct);

        if (task is null)
            return Result<bool>.Failure("Task not found.");

        var newLog = new TaskTimeLog
        {
            TaskId = request.TaskId,
            EmployeeId = request.EmployeeId,
            StartedAt = dateTime.UtcNow
        };

        db.TaskTimeLogs.Add(newLog);
        await db.SaveChangesAsync(ct);

        return Result<bool>.Success(true); // true means timer started
    }
}

