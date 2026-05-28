using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Organization;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Title).HasMaxLength(200);
        b.Property(x => x.Phone).HasMaxLength(20);
        b.Property(x => x.Address).HasMaxLength(500);
        b.Property(x => x.CountryCode).HasMaxLength(10);
        b.Property(x => x.CurrencyCode).HasMaxLength(10);
        b.Property(x => x.TimeZoneId).HasMaxLength(100);
        b.Property(x => x.LocationCode).HasMaxLength(50);
        b.HasIndex(x => x.CompanyId);

        // Map the Tenant navigation to CompanyId — prevents EF generating a shadow TenantId column
        b.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
