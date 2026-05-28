namespace Flowtap_Infrastructure.Services;

public static class CacheKeys
{
    public static string Tenant(Guid id) => $"tenant:{id}";
    public static string UserAccount(Guid id) => $"user:{id}";
    public static string SubscriptionPlan(Guid id) => $"plan:{id}";
    public static string ActiveCampaigns(Guid tenantId) => $"campaigns:active:{tenantId}";
    public static string StockLevel(Guid warehouseId, Guid productId) => $"stock:{warehouseId}:{productId}";
}
