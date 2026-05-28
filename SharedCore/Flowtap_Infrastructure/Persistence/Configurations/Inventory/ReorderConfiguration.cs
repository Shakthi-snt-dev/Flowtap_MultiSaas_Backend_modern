using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Inventory;

public class ReorderRuleConfiguration : IEntityTypeConfiguration<ReorderRule>
{
    public void Configure(EntityTypeBuilder<ReorderRule> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.MinimumQuantity).HasColumnType("decimal(18,4)");
        b.Property(x => x.ReorderQuantity).HasColumnType("decimal(18,4)");
        b.HasIndex(x => new { x.ProductId, x.WarehouseId });
    }
}

public class ReorderAlertConfiguration : IEntityTypeConfiguration<ReorderAlert>
{
    public void Configure(EntityTypeBuilder<ReorderAlert> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.ReorderRuleId);
        b.HasIndex(x => new { x.ProductId, x.WarehouseId });
        b.HasIndex(x => x.CompanyId);
    }
}
