using Flowtap_Domain.BoundedContexts.Core.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Identity;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Token).IsUnique();
        b.HasIndex(x => x.UserAccountId);
        b.Property(x => x.Token).IsRequired().HasMaxLength(512);
        b.Property(x => x.ReplacedByToken).HasMaxLength(512);
    }
}
