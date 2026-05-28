using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Sales;

public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.DiscountValue).HasColumnType("decimal(18,4)");
        b.Property(x => x.BudgetAmount).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.CompanyId);

        b.HasMany(x => x.Rules)
            .WithOne(r => r.Campaign)
            .HasForeignKey(r => r.CampaignId);
    }
}

public class CampaignProductRuleConfiguration : IEntityTypeConfiguration<CampaignProductRule>
{
    public void Configure(EntityTypeBuilder<CampaignProductRule> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.CampaignId);
    }
}

public class MarketingCampaignConfiguration : IEntityTypeConfiguration<MarketingCampaign>
{
    public void Configure(EntityTypeBuilder<MarketingCampaign> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Property(x => x.Message).HasMaxLength(2000);
        b.Property(x => x.DiscountPercentage).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.CompanyId);

        b.HasMany(x => x.TargetLocations)
            .WithOne(l => l.Campaign)
            .HasForeignKey(l => l.CampaignId);
    }
}

public class CampaignTargetLocationConfiguration : IEntityTypeConfiguration<CampaignTargetLocation>
{
    public void Configure(EntityTypeBuilder<CampaignTargetLocation> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.CampaignId, x.LocationId });
    }
}

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.PromoCode).IsRequired().HasMaxLength(50);
        b.HasIndex(x => x.PromoCode).IsUnique();
        b.Property(x => x.DiscountPercent).HasColumnType("decimal(18,4)");
        b.Property(x => x.MinOrderValue).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.CompanyId);
    }
}
