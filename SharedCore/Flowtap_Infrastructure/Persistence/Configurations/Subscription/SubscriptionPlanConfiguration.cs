using Flowtap_Domain.BoundedContexts.Modules.Subscription.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Subscription;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.Description).HasMaxLength(1000);
        b.Property(x => x.PricePerLocation).HasColumnType("decimal(18,4)");
        b.Property(x => x.YearlyDiscountPercent).HasColumnType("decimal(18,4)");
    }
}

public class CompanySubscriptionConfiguration : IEntityTypeConfiguration<CompanySubscription>
{
    public void Configure(EntityTypeBuilder<CompanySubscription> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.CompanyId);
        b.Property(x => x.TotalAmount).HasColumnType("decimal(18,4)");

        b.HasMany(x => x.Locations)
            .WithOne(sl => sl.CompanySubscription)
            .HasForeignKey(sl => sl.CompanySubscriptionId);

        b.HasMany(x => x.Invoices)
            .WithOne(bi => bi.CompanySubscription)
            .HasForeignKey(bi => bi.CompanySubscriptionId);

        b.HasMany(x => x.ChangeLogs)
            .WithOne(cl => cl.CompanySubscription)
            .HasForeignKey(cl => cl.CompanySubscriptionId);
    }
}

public class SubscriptionLocationConfiguration : IEntityTypeConfiguration<SubscriptionLocation>
{
    public void Configure(EntityTypeBuilder<SubscriptionLocation> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.LocationPrice).HasColumnType("decimal(18,4)");
        b.HasIndex(x => new { x.CompanySubscriptionId, x.LocationId }).IsUnique();
    }
}

public class BillingInvoiceConfiguration : IEntityTypeConfiguration<BillingInvoice>
{
    public void Configure(EntityTypeBuilder<BillingInvoice> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.InvoiceNumber).IsRequired().HasMaxLength(50);
        b.HasIndex(x => x.InvoiceNumber).IsUnique();
        b.Property(x => x.SubTotal).HasColumnType("decimal(18,4)");
        b.Property(x => x.TaxAmount).HasColumnType("decimal(18,4)");
        b.Property(x => x.TotalAmount).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.CompanySubscriptionId);

        b.HasMany(x => x.Payments)
            .WithOne(bp => bp.Invoice)
            .HasForeignKey(bp => bp.InvoiceId);
    }
}

public class BillingPaymentConfiguration : IEntityTypeConfiguration<BillingPayment>
{
    public void Configure(EntityTypeBuilder<BillingPayment> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.PaidAmount).HasColumnType("decimal(18,4)");
        b.Property(x => x.TransactionId).HasMaxLength(256);
        b.HasIndex(x => x.InvoiceId);
    }
}

public class SubscriptionChangeLogConfiguration : IEntityTypeConfiguration<SubscriptionChangeLog>
{
    public void Configure(EntityTypeBuilder<SubscriptionChangeLog> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Notes).HasMaxLength(1000);
        b.Property(x => x.OldAmount).HasColumnType("decimal(18,4)");
        b.Property(x => x.NewAmount).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.CompanySubscriptionId);
    }
}

public class TrialPlanConfiguration : IEntityTypeConfiguration<TrialPlan>
{
    public void Configure(EntityTypeBuilder<TrialPlan> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.CompanyId);
    }
}
