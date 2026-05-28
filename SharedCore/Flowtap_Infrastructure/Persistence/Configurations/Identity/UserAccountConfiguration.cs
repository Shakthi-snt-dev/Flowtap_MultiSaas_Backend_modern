using Flowtap_Domain.BoundedContexts.Core.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Identity;

public class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Email).IsUnique();
        b.Property(x => x.Email).IsRequired().HasMaxLength(256);
        b.Property(x => x.PasswordHash).HasMaxLength(512);
        b.Property(x => x.EmailVerificationToken).HasMaxLength(256);
        b.HasOne(x => x.Profile).WithOne(p => p.UserAccount)
            .HasForeignKey<UserProfile>(p => p.UserAccountId);
    }
}
