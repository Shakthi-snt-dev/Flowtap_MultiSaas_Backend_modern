using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Inventory;

public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Reference).HasMaxLength(100);
        b.Property(x => x.Quantity).HasColumnType("decimal(18,4)");
        b.Property(x => x.CostPrice).HasColumnType("decimal(18,4)");
        b.HasIndex(x => new { x.CompanyId, x.ProductId });
    }
}

public class InventoryCostLayerConfiguration : IEntityTypeConfiguration<InventoryCostLayer>
{
    public void Configure(EntityTypeBuilder<InventoryCostLayer> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.RemainingQuantity).HasColumnType("decimal(18,4)");
        b.Property(x => x.UnitCost).HasColumnType("decimal(18,4)");
        b.HasIndex(x => new { x.ProductId, x.WarehouseId });
    }
}

public class InventorySerialConfiguration : IEntityTypeConfiguration<InventorySerial>
{
    public void Configure(EntityTypeBuilder<InventorySerial> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.CompanySerial).IsRequired().HasMaxLength(200);
        b.Property(x => x.ManufacturerSerial).HasMaxLength(200);
        b.HasIndex(x => new { x.CompanyId, x.CompanySerial }).IsUnique();
        b.HasIndex(x => x.ProductId);
    }
}

public class InventorySerialLocationConfiguration : IEntityTypeConfiguration<InventorySerialLocation>
{
    public void Configure(EntityTypeBuilder<InventorySerialLocation> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.SerialId, x.WarehouseId });
    }
}

public class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
{
    public void Configure(EntityTypeBuilder<StockAdjustment> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.AdjustmentNumber).HasMaxLength(100);
        b.Property(x => x.QuantityDifference).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.CompanyId);
    }
}

public class StockBatchConfiguration : IEntityTypeConfiguration<StockBatch>
{
    public void Configure(EntityTypeBuilder<StockBatch> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.BatchNumber).HasMaxLength(100);
        b.Property(x => x.CostPrice).HasColumnType("decimal(18,4)");
        b.HasIndex(x => new { x.ProductId, x.WarehouseId });
    }
}
