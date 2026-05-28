using Flowtap_Repair.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Repair.Infrastructure.Persistence.Configurations;

public class DeviceBrandConfiguration : IEntityTypeConfiguration<DeviceBrand>
{
    public void Configure(EntityTypeBuilder<DeviceBrand> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);

        b.HasMany(x => x.Models)
            .WithOne(m => m.Brand)
            .HasForeignKey(m => m.BrandId);
    }
}

public class DeviceModelConfiguration : IEntityTypeConfiguration<DeviceModel>
{
    public void Configure(EntityTypeBuilder<DeviceModel> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.HasIndex(x => x.BrandId);
    }
}

public class ProductDeviceModelMappingConfiguration : IEntityTypeConfiguration<ProductDeviceModelMapping>
{
    public void Configure(EntityTypeBuilder<ProductDeviceModelMapping> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.ProductId, x.DeviceModelId }).IsUnique();
    }
}
