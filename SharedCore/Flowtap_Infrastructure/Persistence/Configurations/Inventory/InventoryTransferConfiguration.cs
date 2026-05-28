using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Inventory;

public class InventoryTransferConfiguration : IEntityTypeConfiguration<InventoryTransfer>
{
    public void Configure(EntityTypeBuilder<InventoryTransfer> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.TransferNumber).HasMaxLength(100);
        b.Property(x => x.Notes).HasMaxLength(1000);
        b.Property(x => x.VehicleNumber).HasMaxLength(100);
        b.Property(x => x.CourierName).HasMaxLength(200);
        b.Property(x => x.TrackingNumber).HasMaxLength(200);
        b.HasIndex(x => x.CompanyId);

        b.HasMany(x => x.Items)
            .WithOne(i => i.InventoryTransfer)
            .HasForeignKey(i => i.InventoryTransferId);
    }
}

public class InventoryTransferItemConfiguration : IEntityTypeConfiguration<InventoryTransferItem>
{
    public void Configure(EntityTypeBuilder<InventoryTransferItem> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Quantity).HasColumnType("decimal(18,4)");
        b.Property(x => x.ShippedQuantity).HasColumnType("decimal(18,4)");
        b.Property(x => x.ReceivedQuantity).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.InventoryTransferId);
    }
}
