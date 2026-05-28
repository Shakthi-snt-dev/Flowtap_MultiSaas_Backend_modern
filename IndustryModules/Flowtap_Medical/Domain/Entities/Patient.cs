using Flowtap_Domain.SharedKernel;
using Flowtap_Medical.Domain.Enums;

namespace Flowtap_Medical.Domain.Entities;

public class Patient : TenantEntity
{
    public Guid LocationId { get; set; }
    public string PatientNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public BloodGroup? BloodGroup { get; set; }
    public string? Address { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Allergies { get; set; }
    public string? ChronicConditions { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = [];
}
