using Flowtap_Repair.Application.Commands.CreateTask;
using Flowtap_Repair.Application.Commands.UpdateTask;
using Flowtap_Repair.Application.Commands.UpdateTaskStatus;
using Flowtap_Repair.Application.Commands.ToggleTaskTimer;
using Flowtap_Repair.Application.Queries.GetTasks;
using Flowtap_Presentation.Authorization;
using Flowtap_Presentation.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Repair.Controllers;

[RequirePermission("ServiceTickets")]
[Route("api/v1/tasks")]
public class TaskController(ISender sender) : ApiController(sender)
{
    public sealed class CreateTaskRequest
    {
        public Guid LocationId { get; set; }
        public Guid AuthorEmployeeId { get; set; }
        public Guid AssigneeEmployeeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public DateTime? Deadline { get; set; }
        public Guid? TicketId { get; set; }
        public List<string> Tags { get; set; } = [];
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest req, CancellationToken ct)
    {
        var command = new CreateTaskCommand(
            CurrentTenantId,
            req.LocationId,
            req.AuthorEmployeeId,
            req.AssigneeEmployeeId,
            req.Title,
            req.Description,
            req.Priority,
            req.Deadline,
            req.TicketId,
            req.Tags
        );
        return Created(await Sender.Send(command, ct));
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks(
        [FromQuery] Guid? ticketId,
        [FromQuery] Guid? assigneeEmployeeId,
        [FromQuery] Guid? authorEmployeeId,
        [FromQuery] string? status,
        CancellationToken ct)
        => Ok(await Sender.Send(
            new GetTasksQuery(CurrentTenantId, ticketId, assigneeEmployeeId, authorEmployeeId, status), ct));

    public sealed class UpdateTaskRequest
    {
        public Guid? AssigneeEmployeeId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Priority { get; set; }
        public DateTime? Deadline { get; set; }
        public List<string>? Tags { get; set; }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest req, CancellationToken ct)
    {
        var command = new UpdateTaskCommand(
            id, CurrentTenantId,
            req.AssigneeEmployeeId,
            req.Title, req.Description,
            req.Priority, req.Deadline,
            req.Tags
        );
        return FromResult(await Sender.Send(command, ct));
    }

    public sealed class UpdateStatusRequest { public string Status { get; set; } = "New"; }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest req, CancellationToken ct)
        => FromResult(await Sender.Send(new UpdateTaskStatusCommand(CurrentTenantId, id, req.Status), ct));

    [HttpPost("{id:guid}/timer/toggle")]
    public async Task<IActionResult> ToggleTimer(Guid id, CancellationToken ct)
    {
        if (CurrentUserId == Guid.Empty) return Unauthorized();
        return FromResult(await Sender.Send(new ToggleTaskTimerCommand(CurrentTenantId, CurrentUserId, id), ct));
    }
}
