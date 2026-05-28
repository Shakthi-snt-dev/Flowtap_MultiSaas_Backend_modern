using Flowtap_Repair.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Repair.Infrastructure.Persistence.Configurations;

public class ServiceTicketConfiguration : IEntityTypeConfiguration<ServiceTicket>
{
    public void Configure(EntityTypeBuilder<ServiceTicket> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.TicketNumber).HasMaxLength(50);
        b.HasIndex(x => x.TicketNumber).IsUnique();

        b.Property(x => x.RowVersion).IsRowVersion();

        b.Property(x => x.MastersNotes).HasMaxLength(2000);
        b.Property(x => x.Reason).HasMaxLength(2000);

        b.OwnsOne(x => x.DeviceDetails, d =>
        {
            d.Property(p => p.DeviceType).HasMaxLength(100);
            d.Property(p => p.Brand).HasMaxLength(100);
            d.Property(p => p.Model).HasMaxLength(100);
            d.Property(p => p.Serial).HasMaxLength(200);
            d.Property(p => p.Modification).HasMaxLength(200);
            d.Property(p => p.Appearance).HasMaxLength(500);
            d.Property(p => p.Password).HasMaxLength(100);
            d.Property(p => p.Equipment).HasMaxLength(500);
        });

        b.OwnsOne(x => x.Financials, f =>
        {
            f.Property(p => p.EstimatedCost).HasColumnType("decimal(18,4)");
            f.Property(p => p.Prepayment).HasColumnType("decimal(18,4)");
            f.Property(p => p.PrepaymentMethod).HasMaxLength(50);
            f.Property(p => p.PrepaymentPaidAt);
            f.Property(p => p.TotalCost).HasColumnType("decimal(18,4)");
        });

        b.HasIndex(x => x.CompanyId);

        b.HasMany(x => x.Items)
            .WithOne(i => i.Ticket)
            .HasForeignKey(i => i.TicketId);

        b.HasMany(x => x.TimeLogs)
            .WithOne(t => t.Ticket)
            .HasForeignKey(t => t.TicketId);
    }
}

public class ServiceTicketItemConfiguration : IEntityTypeConfiguration<ServiceTicketItem>
{
    public void Configure(EntityTypeBuilder<ServiceTicketItem> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(300);
        b.Property(x => x.Price).HasColumnType("decimal(18,4)");
        b.Property(x => x.Cost).HasColumnType("decimal(18,4)");
        b.Property(x => x.Quantity).HasColumnType("decimal(18,4)");
        b.Property(x => x.DiscountAmount).HasColumnType("decimal(18,4)");
        b.Property(x => x.TaxPercent).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.TicketId);
    }
}

public class ServiceTicketPartUsageConfiguration : IEntityTypeConfiguration<ServiceTicketPartUsage>
{
    public void Configure(EntityTypeBuilder<ServiceTicketPartUsage> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Quantity).HasColumnType("decimal(18,4)");
        b.Property(x => x.UnitPrice).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.ServiceTicketId);
    }
}

public class TicketTimeLogConfiguration : IEntityTypeConfiguration<TicketTimeLog>
{
    public void Configure(EntityTypeBuilder<TicketTimeLog> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.TicketId);
    }
}

public class TechnicalFaultConfiguration : IEntityTypeConfiguration<TechnicalFault>
{
    public void Configure(EntityTypeBuilder<TechnicalFault> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Symptom).IsRequired().HasMaxLength(500);
        b.Property(x => x.PossibleCause).HasMaxLength(2000);
        b.Property(x => x.StandardSolution).HasMaxLength(2000);
        b.HasIndex(x => x.CompanyId);
    }
}

public class RepairChecklistTemplateConfiguration : IEntityTypeConfiguration<RepairChecklistTemplate>
{
    public void Configure(EntityTypeBuilder<RepairChecklistTemplate> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(500);
        b.HasIndex(x => x.CompanyId);
    }
}

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Action).IsRequired().HasMaxLength(200);
        b.Property(x => x.EntityType).HasMaxLength(100);
        b.Property(x => x.Details).HasColumnType("jsonb");
        b.HasIndex(x => x.CompanyId);
    }
}
