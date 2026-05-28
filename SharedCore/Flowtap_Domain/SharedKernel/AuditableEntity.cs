namespace Flowtap_Domain.SharedKernel;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}
