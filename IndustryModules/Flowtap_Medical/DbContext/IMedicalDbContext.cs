using Flowtap_Application.Common.Interfaces;
using Flowtap_Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Medical.DbContext;

public interface IMedicalDbContext : IApplicationDbContext
{
    DbSet<Patient> Patients { get; }
    DbSet<Appointment> Appointments { get; }
    DbSet<Consultation> Consultations { get; }
    DbSet<Prescription> Prescriptions { get; }
}
