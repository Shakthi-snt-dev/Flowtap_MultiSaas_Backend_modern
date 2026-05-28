using Flowtap_Repair.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Repair.Infrastructure.Persistence.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.Description).HasMaxLength(2000);
        b.Property(x => x.BasePrice).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.CompanyId);

        b.HasMany(x => x.SupportedModels)
            .WithOne(m => m.Service)
            .HasForeignKey(m => m.ServiceId);

        b.HasMany(x => x.PartRequirements)
            .WithOne(p => p.Service)
            .HasForeignKey(p => p.ServiceId);
    }
}

public class ServiceCategoryConfiguration : IEntityTypeConfiguration<ServiceCategory>
{
    public void Configure(EntityTypeBuilder<ServiceCategory> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.HasIndex(x => x.CompanyId);
    }
}

public class ServicePartRequirementConfiguration : IEntityTypeConfiguration<ServicePartRequirement>
{
    public void Configure(EntityTypeBuilder<ServicePartRequirement> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.ServiceId);
    }
}

public class ServiceDeviceModelMappingConfiguration : IEntityTypeConfiguration<ServiceDeviceModelMapping>
{
    public void Configure(EntityTypeBuilder<ServiceDeviceModelMapping> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.ServiceId, x.DeviceModelId }).IsUnique();
    }
}

public class WorkTaskConfiguration : IEntityTypeConfiguration<WorkTask>
{
    public void Configure(EntityTypeBuilder<WorkTask> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Title).IsRequired().HasMaxLength(500);
        b.Property(x => x.Description).HasMaxLength(2000);
        b.HasIndex(x => x.CompanyId);

        b.HasMany(x => x.Tags)
            .WithOne(t => t.WorkTask)
            .HasForeignKey(t => t.WorkTaskId);

        b.HasMany(x => x.TimeLogs)
            .WithOne(tl => tl.WorkTask)
            .HasForeignKey(tl => tl.TaskId);
    }
}

public class WorkTaskTagConfiguration : IEntityTypeConfiguration<WorkTaskTag>
{
    public void Configure(EntityTypeBuilder<WorkTaskTag> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Tag).IsRequired().HasMaxLength(100);
        b.HasIndex(x => x.WorkTaskId);
    }
}

public class TaskTimeLogConfiguration : IEntityTypeConfiguration<TaskTimeLog>
{
    public void Configure(EntityTypeBuilder<TaskTimeLog> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.TaskId);
    }
}
