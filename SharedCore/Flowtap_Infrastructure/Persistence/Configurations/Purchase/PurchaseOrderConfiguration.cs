using Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Purchase;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.PONumber).IsRequired().HasMaxLength(50);
        b.HasIndex(x => x.CompanyId);
        b.HasIndex(x => new { x.CompanyId, x.PONumber }).IsUnique();
        // Use PostgreSQL's built-in xmin system column as the concurrency token.
        b.UseXminAsConcurrencyToken();
        b.Property(x => x.InternalNotes).HasMaxLength(2000);
        b.Property(x => x.SubTotal).HasColumnType("decimal(18,4)");
        b.Property(x => x.TaxAmount).HasColumnType("decimal(18,4)");
        b.Property(x => x.TotalAmount).HasColumnType("decimal(18,4)");

        b.HasMany(x => x.Items)
            .WithOne(i => i.PurchaseOrder)
            .HasForeignKey(i => i.PurchaseOrderId);
    }
}

public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.UnitCost).HasColumnType("decimal(18,4)");
        b.Property(x => x.TaxPercent).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.PurchaseOrderId);
    }
}

public class PurchaseReturnConfiguration : IEntityTypeConfiguration<PurchaseReturn>
{
    public void Configure(EntityTypeBuilder<PurchaseReturn> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.ReturnNumber).IsRequired().HasMaxLength(50);
        b.HasIndex(x => new { x.CompanyId, x.ReturnNumber }).IsUnique();
        b.Property(x => x.Reason).HasMaxLength(2000);
        b.Property(x => x.SubTotal).HasColumnType("decimal(18,4)");
        b.Property(x => x.TaxAmount).HasColumnType("decimal(18,4)");
        b.Property(x => x.TotalAmount).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.PurchaseOrderId);

        b.HasMany(x => x.Items)
            .WithOne(i => i.PurchaseReturn)
            .HasForeignKey(i => i.PurchaseReturnId);
    }
}

public class PurchaseReturnItemConfiguration : IEntityTypeConfiguration<PurchaseReturnItem>
{
    public void Configure(EntityTypeBuilder<PurchaseReturnItem> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.UnitCost).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.PurchaseReturnId);
    }
}
