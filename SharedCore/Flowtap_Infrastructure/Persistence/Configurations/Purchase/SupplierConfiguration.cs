using Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Purchase;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.Phone).HasMaxLength(20);
        b.Property(x => x.Email).HasMaxLength(200);
        b.Property(x => x.Address).HasMaxLength(500);
        b.Property(x => x.ContactPerson).HasMaxLength(200);
        b.HasIndex(x => x.CompanyId);
    }
}
