using Flowtap_Application.Common.Interfaces;
using Flowtap_Jewelry.Domain.Entities;
using Flowtap_Infrastructure.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Flowtap_Jewelry.DbContext;

/// <summary>
/// Extends the shared ApplicationDbContext with Jewelry-industry entities.
/// Registered as IJewelryDbContext in the Jewelry API DI container.
/// </summary>
public class JewelryDbContext(
    DbContextOptions<JewelryDbContext> options,
    ICurrentUserService currentUser,
    IPublisher publisher)
    : ApplicationDbContext(ChangeOptionsType<ApplicationDbContext>(options), currentUser, publisher),
      IJewelryDbContext
{
    public DbSet<MetalRate> MetalRates => Set<MetalRate>();
    public DbSet<MetalExchangeTransaction> MetalExchangeTransactions => Set<MetalExchangeTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<MetalRate>(b => b.HasKey(x => x.Id));
        modelBuilder.Entity<MetalExchangeTransaction>(b => b.HasKey(x => x.Id));
    }

    // Helper: convert DbContextOptions<TDerived> → DbContextOptions<TBase>
    private static DbContextOptions<TBase> ChangeOptionsType<TBase>(DbContextOptions options)
        where TBase : Microsoft.EntityFrameworkCore.DbContext
    {
        var builder = new DbContextOptionsBuilder<TBase>();
        foreach (var extension in options.Extensions)
            ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(extension);
        return builder.Options;
    }
}
