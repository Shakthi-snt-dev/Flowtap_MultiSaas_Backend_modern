using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Organization;

public class StoreSettingConfiguration : IEntityTypeConfiguration<StoreSetting>
{
    public void Configure(EntityTypeBuilder<StoreSetting> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.LocationId).IsUnique();
        b.HasIndex(x => x.CompanyId);

        b.Property(x => x.ThemeMode).HasMaxLength(20).HasDefaultValue("light");
        b.Property(x => x.ColorTheme).HasMaxLength(30).HasDefaultValue("default");
        b.Property(x => x.AccentColor).HasMaxLength(20).HasDefaultValue("blue");
        b.Property(x => x.FontFamily).HasMaxLength(30).HasDefaultValue("inter");
        b.Property(x => x.BorderRadius).HasMaxLength(20).HasDefaultValue("normal");
        b.Property(x => x.SidebarDensity).HasMaxLength(20).HasDefaultValue("comfortable");
        b.Property(x => x.MaxDiscountPercent).HasColumnType("decimal(5,2)");
        b.Property(x => x.ReceiptFooterText).HasMaxLength(500);
        b.Property(x => x.OpeningTime).HasMaxLength(5);
        b.Property(x => x.ClosingTime).HasMaxLength(5);
    }
}
