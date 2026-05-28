using Flowtap_Application.Common.Interfaces;
using Flowtap_Medical.Domain.Entities;
using Flowtap_Infrastructure.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Flowtap_Medical.DbContext;

/// <summary>
/// Extends the shared ApplicationDbContext with Medical-industry entities.
/// Registered as IMedicalDbContext in the Medical API DI container.
/// </summary>
public class MedicalDbContext(
    DbContextOptions<MedicalDbContext> options,
    ICurrentUserService currentUser,
    IPublisher publisher)
    : ApplicationDbContext(ChangeOptionsType<ApplicationDbContext>(options), currentUser, publisher),
      IMedicalDbContext
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Consultation> Consultations => Set<Consultation>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Patient>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasMany(x => x.Appointments).WithOne(x => x.Patient).HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Appointment>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Consultation).WithOne(x => x.Appointment).HasForeignKey<Consultation>(x => x.AppointmentId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Consultation>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasMany(x => x.Prescriptions).WithOne(x => x.Consultation).HasForeignKey(x => x.ConsultationId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Prescription>(b => b.HasKey(x => x.Id));
    }

    // Helper: convert DbContextOptions<TDerived> → DbContextOptions<TBase>
    private static DbContextOptions<TBase> ChangeOptionsType<TBase>(DbContextOptions options)
        where TBase : Microsoft.EntityFrameworkCore.DbContext
    {
        var builder = new DbContextOptionsBuilder<TBase>();
        foreach (var extension in options.Extensions)
            ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(extension);
        return builder.Options;
    }
}
