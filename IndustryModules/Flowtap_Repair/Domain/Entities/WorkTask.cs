using Flowtap_Repair.Domain.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Repair.Domain.Entities;
public class WorkTask : TenantEntity
{
    public Guid LocationId { get; set; }
    public Guid AuthorEmployeeId { get; set; }
    public Guid AssigneeEmployeeId { get; set; }
    public Guid? TicketId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.New;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? CompletedAt { get; set; }
    public ICollection<WorkTaskTag> Tags { get; set; } = [];
    public ICollection<TaskTimeLog> TimeLogs { get; set; } = [];
}
