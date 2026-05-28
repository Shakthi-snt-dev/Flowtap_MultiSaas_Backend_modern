using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowtap_Infrastructure.Persistence.Configurations.Sales;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.IdempotencyKey).HasMaxLength(128);
        b.HasIndex(x => x.IdempotencyKey);
        b.HasIndex(x => x.SaleId);
        b.Property(x => x.Amount).HasColumnType("decimal(18,4)");
        b.Property(x => x.ExternalReference).HasMaxLength(256);
        b.Property(x => x.Comment).HasMaxLength(500);
        b.HasIndex(x => x.TicketId);
        b.HasIndex(x => x.CompanyId);
    }
}

public class PaymentAccountConfiguration : IEntityTypeConfiguration<PaymentAccount>
{
    public void Configure(EntityTypeBuilder<PaymentAccount> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.HasIndex(x => x.CompanyId);

        b.HasMany(x => x.Payments)
            .WithOne(p => p.Account)
            .HasForeignKey(p => p.AccountId);

        b.HasMany(x => x.MethodMappings)
            .WithOne(m => m.PaymentAccount)
            .HasForeignKey(m => m.PaymentAccountId);
    }
}

public class PaymentMethodMappingConfiguration : IEntityTypeConfiguration<PaymentMethodMapping>
{
    public void Configure(EntityTypeBuilder<PaymentMethodMapping> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.CompanyId, x.PaymentAccountId });
    }
}

public class PaymentGatewayTransactionConfiguration : IEntityTypeConfiguration<PaymentGatewayTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentGatewayTransaction> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.ExternalOrderId).HasMaxLength(256);
        b.Property(x => x.ExternalPaymentId).HasMaxLength(256);
        b.Property(x => x.Currency).HasMaxLength(10);
        b.Property(x => x.FailureReason).HasMaxLength(1000);
        b.Property(x => x.Amount).HasColumnType("decimal(18,4)");
        b.HasIndex(x => x.PaymentId);
        b.HasIndex(x => x.CompanyId);
    }
}
