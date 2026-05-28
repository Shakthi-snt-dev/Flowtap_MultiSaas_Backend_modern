using Flowtap_Application.Common.Interfaces;
using Flowtap_Food.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.DbContext;

/// <summary>
/// Extends IApplicationDbContext with Food-industry-specific DbSets.
/// Injected in all food handlers and behaviors.
/// </summary>
public interface IFoodDbContext : IApplicationDbContext
{
    DbSet<FoodTable> FoodTables { get; }
    DbSet<KitchenOrder> KitchenOrders { get; }
    DbSet<KitchenOrderItem> KitchenOrderItems { get; }
    DbSet<Recipe> Recipes { get; }
    DbSet<RecipeIngredient> RecipeIngredients { get; }
    DbSet<StockAlertRule> StockAlertRules { get; }
}
