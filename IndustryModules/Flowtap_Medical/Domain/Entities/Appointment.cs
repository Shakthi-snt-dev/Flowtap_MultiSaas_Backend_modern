using Flowtap_Domain.SharedKernel;
using Flowtap_Medical.Domain.Enums;

namespace Flowtap_Medical.Domain.Entities;

public class Appointment : TenantEntity
{
    public Guid LocationId { get; set; }
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public Guid? DoctorEmployeeId { get; set; }
    public string AppointmentNumber { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public AppointmentType Type { get; set; } = AppointmentType.Consultation;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public int? DurationMinutes { get; set; }
    public Consultation? Consultation { get; set; }
}
