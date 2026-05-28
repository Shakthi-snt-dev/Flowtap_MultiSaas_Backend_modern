using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Inventory;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.HasIndex(x => x.CompanyId);
    }
}

public class WarehouseStockConfiguration : IEntityTypeConfiguration<WarehouseStock>
{
    public void Configure(EntityTypeBuilder<WarehouseStock> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Quantity).HasColumnType("decimal(18,4)");
        b.Property(x => x.ReservedQuantity).HasColumnType("decimal(18,4)");
        b.HasIndex(x => new { x.WarehouseId, x.ProductId }).IsUnique();
    }
}

public class WarehouseRackConfiguration : IEntityTypeConfiguration<WarehouseRack>
{
    public void Configure(EntityTypeBuilder<WarehouseRack> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        b.HasIndex(x => x.WarehouseId);
    }
}

public class WarehouseBinConfiguration : IEntityTypeConfiguration<WarehouseBin>
{
    public void Configure(EntityTypeBuilder<WarehouseBin> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Code).HasMaxLength(100);
        b.HasIndex(x => x.RackId);
    }
}

public class WarehouseBinStockConfiguration : IEntityTypeConfiguration<WarehouseBinStock>
{
    public void Configure(EntityTypeBuilder<WarehouseBinStock> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Quantity).HasColumnType("decimal(18,4)");
        b.HasIndex(x => new { x.BinId, x.ProductId });
    }
}
