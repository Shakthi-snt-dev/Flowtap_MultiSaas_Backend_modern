using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Inventory;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(300);
        b.Property(x => x.SKU).HasMaxLength(100);
        b.Property(x => x.DefaultCostPrice).HasColumnType("decimal(18,4)");
        b.Property(x => x.DefaultSalePrice).HasColumnType("decimal(18,4)");

        b.HasIndex(x => new { x.CompanyId, x.SKU }).IsUnique();

        b.HasMany(x => x.Variants)
            .WithOne(v => v.Product)
            .HasForeignKey(v => v.ProductId);

        b.HasMany(x => x.Media)
            .WithOne(m => m.Product)
            .HasForeignKey(m => m.ProductId);
    }
}

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(300);
        b.HasIndex(x => x.CompanyId);
    }
}

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(300);
        b.Property(x => x.SKU).HasMaxLength(100);
        b.Property(x => x.AdditionalPrice).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.ProductId);
    }
}

public class ProductMediaConfiguration : IEntityTypeConfiguration<ProductMedia>
{
    public void Configure(EntityTypeBuilder<ProductMedia> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Url).IsRequired().HasMaxLength(1000);
        b.HasIndex(x => x.ProductId);
        b.HasIndex(x => x.IsPrimary);
    }
}

public class ProductLocationPriceConfiguration : IEntityTypeConfiguration<ProductLocationPrice>
{
    public void Configure(EntityTypeBuilder<ProductLocationPrice> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.CostPrice).HasColumnType("decimal(18,4)");
        b.Property(x => x.SalePrice).HasColumnType("decimal(18,4)");
        b.Property(x => x.MRP).HasColumnType("decimal(18,4)");
        b.Property(x => x.TaxSlabId);   // nullable FK — no cascade delete, just null out
        b.HasIndex(x => new { x.ProductId, x.LocationId }).IsUnique();
    }
}



public class BarcodeTemplateConfiguration : IEntityTypeConfiguration<BarcodeTemplate>
{
    public void Configure(EntityTypeBuilder<BarcodeTemplate> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.HasIndex(x => x.CompanyId);
    }
}

public class SerialCounterConfiguration : IEntityTypeConfiguration<SerialCounter>
{
    public void Configure(EntityTypeBuilder<SerialCounter> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.CompanyId);
        b.HasIndex(x => x.ProductId).IsUnique();
    }
}

public class InventorySettingsConfiguration : IEntityTypeConfiguration<InventorySettings>
{
    public void Configure(EntityTypeBuilder<InventorySettings> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.CompanyId).IsUnique();
    }
}

public class LocationInventorySettingsConfiguration : IEntityTypeConfiguration<LocationInventorySettings>
{
    public void Configure(EntityTypeBuilder<LocationInventorySettings> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.LocationId).IsUnique();
    }
}
