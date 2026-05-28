using Flowtap_Domain.SharedKernel;

namespace Flowtap_Medical.Domain.Entities;

public class Consultation : TenantEntity
{
    public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;
    public string? ChiefComplaint { get; set; }
    public string? Diagnosis { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? VitalSigns { get; set; }
    public string? LabResults { get; set; }
    public string? DoctorNotes { get; set; }
    public decimal? ConsultationFee { get; set; }
    public Guid? SaleId { get; set; }
    public ICollection<Prescription> Prescriptions { get; set; } = [];
}
