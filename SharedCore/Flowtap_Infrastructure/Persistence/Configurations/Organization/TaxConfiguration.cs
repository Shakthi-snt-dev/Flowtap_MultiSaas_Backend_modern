using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Organization;

public class TaxConfigurationEntityConfiguration : IEntityTypeConfiguration<TaxConfiguration>
{
    public void Configure(EntityTypeBuilder<TaxConfiguration> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.CountryCode).HasMaxLength(10);
        b.Property(x => x.TaxIdNumber).HasMaxLength(100);
        b.Property(x => x.TaxRepresentativeName).HasMaxLength(200);
        b.HasIndex(x => x.CompanyId);

        b.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.RateRules)
            .WithOne(r => r.TaxConfiguration)
            .HasForeignKey(r => r.TaxConfigurationId);
    }
}

public class TaxSlabConfiguration : IEntityTypeConfiguration<TaxSlab>
{
    public void Configure(EntityTypeBuilder<TaxSlab> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(200);
        b.Property(x => x.Code).HasMaxLength(50);
        b.HasIndex(x => x.CompanyId);

        b.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.RateRules)
            .WithOne(r => r.TaxSlab)
            .HasForeignKey(r => r.TaxSlabId);
    }
}

public class TaxRateRuleConfiguration : IEntityTypeConfiguration<TaxRateRule>
{
    public void Configure(EntityTypeBuilder<TaxRateRule> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.ComponentName).IsRequired().HasMaxLength(200);
        b.Property(x => x.Jurisdiction).HasMaxLength(200);
        b.Property(x => x.Rate).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.TaxSlabId);
        b.HasIndex(x => x.TaxConfigurationId);
    }
}
