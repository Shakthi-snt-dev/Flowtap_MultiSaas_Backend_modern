using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Domain.Entities;
using Flowtap_Repair.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Commands.UpdateTask;

public class UpdateTaskCommandHandler(IRepairDbContext db)
    : IRequestHandler<UpdateTaskCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateTaskCommand request, CancellationToken ct)
    {
        var task = await db.WorkTasks
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.CompanyId == request.CompanyId, ct)
            ?? throw new NotFoundException("WorkTask", request.Id);

        if (request.AssigneeEmployeeId.HasValue && request.AssigneeEmployeeId.Value != Guid.Empty)
            task.AssigneeEmployeeId = request.AssigneeEmployeeId.Value;

        if (!string.IsNullOrWhiteSpace(request.Title))
            task.Title = request.Title;

        if (request.Description is not null)
            task.Description = request.Description;

        if (!string.IsNullOrWhiteSpace(request.Priority)
            && Enum.TryParse<TaskPriority>(request.Priority, true, out var priority))
            task.Priority = priority;

        if (request.Deadline is not null)
            task.Deadline = request.Deadline.HasValue
                ? DateTime.SpecifyKind(request.Deadline.Value, DateTimeKind.Utc)
                : null;

        // Tags: replace entirely when provided
        if (request.Tags is not null)
        {
            task.Tags.Clear();
            foreach (var tag in request.Tags)
                task.Tags.Add(new WorkTaskTag { Tag = tag });
        }

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

