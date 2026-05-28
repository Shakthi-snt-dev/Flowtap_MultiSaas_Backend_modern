using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Organization;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> b)
    {
        b.ToTable("Tenants");
        b.HasKey(x => x.Id);
        b.Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Property(x => x.SubDomain).IsRequired().HasMaxLength(100);
        b.HasIndex(x => x.SubDomain).IsUnique();
        b.Property(x => x.Phone).HasMaxLength(20);
        b.Property(x => x.Email).HasMaxLength(200);
        b.Property(x => x.Country).HasMaxLength(10);
        b.Property(x => x.Currency).HasMaxLength(10);
        b.Property(x => x.BusinessType).HasMaxLength(100);

        b.HasOne(x => x.Settings)
            .WithOne(s => s.Tenant)
            .HasForeignKey<TenantSettings>(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class TenantSettingsConfiguration : IEntityTypeConfiguration<TenantSettings>
{
    public void Configure(EntityTypeBuilder<TenantSettings> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.CompanyId).IsUnique();
    }
}
