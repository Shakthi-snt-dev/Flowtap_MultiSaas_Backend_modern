using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Domain.Entities;
using Flowtap_Repair.Domain.Enums;
using ServiceTaskStatus = Flowtap_Repair.Domain.Enums.TaskStatus;
using MediatR;

namespace Flowtap_Repair.Application.Commands.CreateTask;

public class CreateTaskCommandHandler(IRepairDbContext db)
    : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTaskCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<TaskPriority>(request.Priority, true, out var priority))
            priority = TaskPriority.Medium;

        var task = new WorkTask
        {
            CompanyId = request.CompanyId,
            LocationId = request.LocationId,
            AuthorEmployeeId = request.AuthorEmployeeId,
            AssigneeEmployeeId = request.AssigneeEmployeeId,
            Title = request.Title,
            Description = request.Description,
            Priority = priority,
            // Npgsql requires DateTimeKind.Utc for timestamptz columns.
            // ASP.NET deserialises ISO-8601 strings as Kind=Unspecified, so we force UTC here.
            Deadline = request.Deadline.HasValue
                ? DateTime.SpecifyKind(request.Deadline.Value, DateTimeKind.Utc)
                : null,
            TicketId = request.TicketId,
            Status = ServiceTaskStatus.New,
            Tags = request.Tags.Select(t => new WorkTaskTag { Tag = t }).ToList()
        };

        db.WorkTasks.Add(task);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(task.Id);
    }
}

