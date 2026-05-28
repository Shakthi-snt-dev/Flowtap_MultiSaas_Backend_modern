using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Queries.GetTasks;

public class GetTasksQueryHandler(IRepairDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetTasksQuery, Result<List<TaskDto>>>
{
    public async Task<Result<List<TaskDto>>> Handle(GetTasksQuery request, CancellationToken ct)
    {
        var query = db.WorkTasks
            .Include(t => t.Tags)
            .Include(t => t.TimeLogs)
            .Where(t => t.CompanyId == request.CompanyId);

        var storeId = currentUser.StoreId;
        if (storeId.HasValue && storeId.Value != Guid.Empty)
        {
            query = query.Where(t => t.LocationId == storeId.Value);
        }

        if (request.TicketId.HasValue)
            query = query.Where(t => t.TicketId == request.TicketId.Value);

        if (request.AssigneeEmployeeId.HasValue)
            query = query.Where(t => t.AssigneeEmployeeId == request.AssigneeEmployeeId.Value);

        if (request.AuthorEmployeeId.HasValue)
            query = query.Where(t => t.AuthorEmployeeId == request.AuthorEmployeeId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(t => t.Status.ToString() == request.Status);

        var tasks = await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);

        // ── Batch-load employee names (Author + Assignee) ─────────────────────────
        var employeeIds = tasks
            .SelectMany(t => new[] { t.AuthorEmployeeId, t.AssigneeEmployeeId })
            .Distinct()
            .ToList();

        var nameMap = await db.Employees
            .Where(e => employeeIds.Contains(e.Id))
            .Join(db.UserProfiles,
                  e => e.UserAccountId,
                  p => p.UserAccountId,
                  (e, p) => new { EmployeeId = e.Id, p.Name })
            .ToDictionaryAsync(x => x.EmployeeId, x => x.Name, ct);

        var dtos = tasks.Select(t => new TaskDto(
            t.Id,
            t.CompanyId,
            t.LocationId,
            t.AuthorEmployeeId,
            t.AssigneeEmployeeId,
            t.TicketId,
            t.Title,
            t.Description ?? string.Empty,
            t.Status.ToString(),
            t.Priority.ToString(),
            t.CreatedAt,
            t.Deadline,
            t.CompletedAt,
            t.Tags.Select(tag => tag.Tag).ToList(),
            t.TimeLogs.Any(l => l.StoppedAt == null),
            (long)t.TimeLogs.Sum(l => ((l.StoppedAt ?? DateTime.UtcNow) - l.StartedAt).TotalSeconds),
            AuthorName:   nameMap.TryGetValue(t.AuthorEmployeeId,   out var an) ? an : string.Empty,
            AssigneeName: nameMap.TryGetValue(t.AssigneeEmployeeId, out var sn) ? sn : string.Empty
        )).ToList();

        return Result<List<TaskDto>>.Success(dtos);
    }
}

