using Flowtap_Domain.BoundedContexts.Core.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Identity;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.Phone).HasMaxLength(20);
        b.Property(x => x.AvatarUrl).HasMaxLength(500);
    }
}
