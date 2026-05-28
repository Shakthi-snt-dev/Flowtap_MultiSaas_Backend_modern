using Flowtap_Application.Common.Interfaces;
using Flowtap_Food.Domain.Entities;
using Flowtap_Infrastructure.Persistence.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Flowtap_Food.DbContext;

/// <summary>
/// Extends the shared ApplicationDbContext with Food-industry entities.
/// Registered as IFoodDbContext in the Food API DI container.
/// </summary>
public class FoodDbContext(
    DbContextOptions<FoodDbContext> options,
    ICurrentUserService currentUser,
    IPublisher publisher)
    : ApplicationDbContext(ChangeOptionsType<ApplicationDbContext>(options), currentUser, publisher),
      IFoodDbContext
{
    public DbSet<FoodTable> FoodTables => Set<FoodTable>();
    public DbSet<KitchenOrder> KitchenOrders => Set<KitchenOrder>();
    public DbSet<KitchenOrderItem> KitchenOrderItems => Set<KitchenOrderItem>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<StockAlertRule> StockAlertRules => Set<StockAlertRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FoodTable>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasMany(x => x.KitchenOrders).WithOne(x => x.Table).HasForeignKey(x => x.TableId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<KitchenOrder>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasMany(x => x.Items).WithOne(x => x.KitchenOrder).HasForeignKey(x => x.KitchenOrderId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<KitchenOrderItem>(b => b.HasKey(x => x.Id));

        modelBuilder.Entity<Recipe>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasMany(x => x.Ingredients).WithOne(x => x.Recipe).HasForeignKey(x => x.RecipeId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RecipeIngredient>(b => b.HasKey(x => x.Id));
        modelBuilder.Entity<StockAlertRule>(b => b.HasKey(x => x.Id));
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
