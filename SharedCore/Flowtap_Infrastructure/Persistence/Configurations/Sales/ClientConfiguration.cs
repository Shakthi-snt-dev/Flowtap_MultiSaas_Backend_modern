using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Sales;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.Phone).HasMaxLength(20);
        b.Property(x => x.Email).HasMaxLength(200);
        b.HasIndex(x => new { x.CompanyId, x.Phone });
    }
}
