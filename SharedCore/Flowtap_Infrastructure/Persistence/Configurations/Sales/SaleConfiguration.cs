using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Sales;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.IdempotencyKey).HasMaxLength(128);
        b.HasIndex(x => x.IdempotencyKey);
        // Use PostgreSQL's built-in xmin system column as the concurrency token.
        // xmin is automatically updated by Postgres on every row modification,
        // so it is always non-null and never requires a migration.
        b.UseXminAsConcurrencyToken();
        b.Property(x => x.SubTotal).HasColumnType("decimal(18,4)");
        b.Property(x => x.DiscountAmount).HasColumnType("decimal(18,4)");
        b.Property(x => x.TaxAmount).HasColumnType("decimal(18,4)");
        b.Property(x => x.TotalAmount).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.CompanyId);

        b.HasMany(x => x.Items)
            .WithOne(i => i.Sale)
            .HasForeignKey(i => i.SaleId);

        b.HasMany(x => x.Payments)
            .WithOne(p => p.Sale)
            .HasForeignKey(p => p.SaleId);

        b.HasMany(x => x.History)
            .WithOne(h => h.Sale)
            .HasForeignKey(h => h.SaleId);
    }
}

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.UnitPrice).HasColumnType("decimal(18,4)");
        b.Property(x => x.Quantity).HasColumnType("decimal(18,4)");
        b.Property(x => x.DiscountAmount).HasColumnType("decimal(18,4)");
        b.Property(x => x.Total).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.SaleId);
    }
}

public class SaleHistoryConfiguration : IEntityTypeConfiguration<SaleHistory>
{
    public void Configure(EntityTypeBuilder<SaleHistory> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Message).HasMaxLength(1000);
        b.HasIndex(x => x.SaleId);
    }
}
