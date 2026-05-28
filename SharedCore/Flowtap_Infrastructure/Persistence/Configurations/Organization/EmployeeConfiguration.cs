using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Organization;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.JobTitle).HasMaxLength(200);
        b.Property(x => x.Department).HasMaxLength(200);
        b.HasIndex(x => new { x.CompanyId, x.UserAccountId });

        b.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Permissions)
            .WithOne(p => p.Employee)
            .HasForeignKey(p => p.EmployeeId);

        b.HasMany(x => x.StatusPermissions)
            .WithOne(p => p.Employee)
            .HasForeignKey(p => p.EmployeeId);

        b.HasMany(x => x.LocationAccess)
            .WithOne(p => p.Employee)
            .HasForeignKey(p => p.EmployeeId);
    }
}

public class SalarySettingConfiguration : IEntityTypeConfiguration<SalarySetting>
{
    public void Configure(EntityTypeBuilder<SalarySetting> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.EmployeeId);
        b.Property(x => x.FixedSalary).HasColumnType("decimal(18,4)");
        b.Property(x => x.TicketSalaryPercent).HasColumnType("decimal(18,4)");
        b.Property(x => x.ServicesSalaryPercent).HasColumnType("decimal(18,4)");
        b.Property(x => x.PartsSalaryPercent).HasColumnType("decimal(18,4)");
        b.Property(x => x.SalesSalaryPercent).HasColumnType("decimal(18,4)");
    }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.DisplayName).IsRequired().HasMaxLength(200);
        b.Property(x => x.Key).IsRequired().HasMaxLength(200);
        b.HasIndex(x => x.Key).IsUnique();
    }
}

public class PermissionCategoryConfiguration : IEntityTypeConfiguration<PermissionCategory>
{
    public void Configure(EntityTypeBuilder<PermissionCategory> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
    }
}

public class StatusPermissionConfiguration : IEntityTypeConfiguration<StatusPermission>
{
    public void Configure(EntityTypeBuilder<StatusPermission> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.StatusName).IsRequired().HasMaxLength(200);
        b.Property(x => x.DisplayName).HasMaxLength(200);
    }
}

public class EmployeePermissionConfiguration : IEntityTypeConfiguration<EmployeePermission>
{
    public void Configure(EntityTypeBuilder<EmployeePermission> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.EmployeeId, x.PermissionId });
    }
}

public class EmployeeStatusPermissionConfiguration : IEntityTypeConfiguration<EmployeeStatusPermission>
{
    public void Configure(EntityTypeBuilder<EmployeeStatusPermission> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.EmployeeId, x.StatusPermissionId });
    }
}

public class EmployeeLocationAccessConfiguration : IEntityTypeConfiguration<EmployeeLocationAccess>
{
    public void Configure(EntityTypeBuilder<EmployeeLocationAccess> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.EmployeeId, x.LocationId });

        b.HasOne(x => x.Store)
            .WithMany()
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class EmployeeRightsConfiguration : IEntityTypeConfiguration<EmployeeRights>
{
    public void Configure(EntityTypeBuilder<EmployeeRights> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.EmployeeId);
    }
}

public class OrderTypeConfiguration : IEntityTypeConfiguration<OrderType>
{
    public void Configure(EntityTypeBuilder<OrderType> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
    }
}

public class LocationOrderTypeConfiguration : IEntityTypeConfiguration<LocationOrderType>
{
    public void Configure(EntityTypeBuilder<LocationOrderType> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.LocationId, x.OrderTypeId });
    }
}
