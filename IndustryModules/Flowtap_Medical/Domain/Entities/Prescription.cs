using Flowtap_Domain.SharedKernel;

namespace Flowtap_Medical.Domain.Entities;

public class Prescription : AuditableEntity
{
    public Guid ConsultationId { get; set; }
    public Consultation Consultation { get; set; } = null!;
    public string MedicineName { get; set; } = string.Empty;
    public Guid? ProductId { get; set; }
    public string Dosage { get; set; } = string.Empty;
    public string? Duration { get; set; }
    public string? Instructions { get; set; }
    public bool IsDispensed { get; set; }
}
