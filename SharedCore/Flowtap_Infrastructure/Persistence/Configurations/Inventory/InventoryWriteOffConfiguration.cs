using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Inventory;

public class InventoryWriteOffConfiguration : IEntityTypeConfiguration<InventoryWriteOff>
{
    public void Configure(EntityTypeBuilder<InventoryWriteOff> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.WriteOffNumber).HasMaxLength(100);
        b.Property(x => x.Reason).HasMaxLength(1000);
        b.Property(x => x.Notes).HasMaxLength(2000);
        b.Property(x => x.Quantity).HasColumnType("decimal(18,4)");
        b.Property(x => x.UnitCost).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.CompanyId);

        b.HasMany(x => x.Attachments)
            .WithOne(a => a.InventoryWriteOff)
            .HasForeignKey(a => a.InventoryWriteOffId);
    }
}

public class InventoryWriteOffAttachmentConfiguration : IEntityTypeConfiguration<InventoryWriteOffAttachment>
{
    public void Configure(EntityTypeBuilder<InventoryWriteOffAttachment> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.FileUrl).IsRequired().HasMaxLength(1000);
        b.Property(x => x.FileType).HasMaxLength(100);
        b.HasIndex(x => x.InventoryWriteOffId);
    }
}
