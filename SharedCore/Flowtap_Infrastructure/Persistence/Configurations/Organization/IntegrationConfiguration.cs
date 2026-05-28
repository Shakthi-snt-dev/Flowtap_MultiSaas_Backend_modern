using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Organization;

public class IntegrationConfiguration : IEntityTypeConfiguration<Integration>
{
    public void Configure(EntityTypeBuilder<Integration> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Category).HasMaxLength(50).IsRequired();
        b.Property(x => x.Provider).HasMaxLength(100).IsRequired();
        b.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
        b.Property(x => x.ConfigJson).HasColumnType("jsonb");
        b.Property(x => x.WebhookUrl).HasMaxLength(500);
        b.Property(x => x.LastStatusMessage).HasMaxLength(1000);

        // One company can have at most one record per provider
        b.HasIndex(x => new { x.CompanyId, x.Provider }).IsUnique();
        b.HasIndex(x => x.CompanyId);

        b.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
