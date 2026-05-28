using Flowtap_Domain.BoundedContexts.Core.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Identity;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.UserAccountId);
        b.Property(x => x.DeviceInfo).HasMaxLength(500);
        b.Property(x => x.IpAddress).HasMaxLength(64);
    }
}
