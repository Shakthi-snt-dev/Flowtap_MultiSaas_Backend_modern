using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.SharedKernel;
using Flowtap_Domain.BoundedContexts.Core.Identity.Entities;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Subscription.Entities;

using Microsoft.EntityFrameworkCore;
using MediatR;

namespace Flowtap_Infrastructure.Persistence.DbContext;

public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUser;
    private readonly IPublisher _publisher;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUser,
        IPublisher publisher) : base(options)
    {
        _currentUser = currentUser;
        _publisher = publisher;
    }

    // Identity
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<UserNotificationSettings> UserNotificationSettings => Set<UserNotificationSettings>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<NotificationQueue> NotificationQueues => Set<NotificationQueue>();
    public DbSet<AdminBroadcast> AdminBroadcasts => Set<AdminBroadcast>();
    public DbSet<AnnouncementTarget> AnnouncementTargets => Set<AnnouncementTarget>();
    public DbSet<DirectMessage> DirectMessages => Set<DirectMessage>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();

    // Organization
    public DbSet<Integration> Integrations => Set<Integration>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantSettings> TenantSettings => Set<TenantSettings>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<SalarySetting> SalarySettings => Set<SalarySetting>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<PermissionCategory> PermissionCategories => Set<PermissionCategory>();
    public DbSet<StatusPermission> StatusPermissions => Set<StatusPermission>();
    public DbSet<EmployeePermission> EmployeePermissions => Set<EmployeePermission>();
    public DbSet<EmployeeStatusPermission> EmployeeStatusPermissions => Set<EmployeeStatusPermission>();
    public DbSet<EmployeeLocationAccess> EmployeeLocationAccesses => Set<EmployeeLocationAccess>();
    public DbSet<EmployeeRights> EmployeeRights => Set<EmployeeRights>();
    public DbSet<StoreSetting> StoreSettings => Set<StoreSetting>();
    public DbSet<OrderType> OrderTypes => Set<OrderType>();
    public DbSet<LocationOrderType> LocationOrderTypes => Set<LocationOrderType>();
    public DbSet<TaxConfiguration> TaxConfigurations => Set<TaxConfiguration>();
    public DbSet<TaxSlab> TaxSlabs => Set<TaxSlab>();
    public DbSet<TaxRateRule> TaxRateRules => Set<TaxRateRule>();

    // Inventory
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductMedia> ProductMedia => Set<ProductMedia>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<WarehouseStock> WarehouseStocks => Set<WarehouseStock>();
    public DbSet<WarehouseRack> WarehouseRacks => Set<WarehouseRack>();
    public DbSet<WarehouseBin> WarehouseBins => Set<WarehouseBin>();
    public DbSet<WarehouseBinStock> WarehouseBinStocks => Set<WarehouseBinStock>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<InventoryCostLayer> InventoryCostLayers => Set<InventoryCostLayer>();
    public DbSet<InventorySerial> InventorySerials => Set<InventorySerial>();
    public DbSet<InventorySerialLocation> InventorySerialLocations => Set<InventorySerialLocation>();
    public DbSet<InventoryTransfer> InventoryTransfers => Set<InventoryTransfer>();
    public DbSet<InventoryTransferItem> InventoryTransferItems => Set<InventoryTransferItem>();
    public DbSet<InventoryWriteOff> InventoryWriteOffs => Set<InventoryWriteOff>();
    public DbSet<InventoryWriteOffAttachment> InventoryWriteOffAttachments => Set<InventoryWriteOffAttachment>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    public DbSet<StockBatch> StockBatches => Set<StockBatch>();
    public DbSet<ReorderRule> ReorderRules => Set<ReorderRule>();
    public DbSet<ReorderAlert> ReorderAlerts => Set<ReorderAlert>();
    public DbSet<BarcodeTemplate> BarcodeTemplates => Set<BarcodeTemplate>();
    public DbSet<InventorySettings> InventorySettings => Set<InventorySettings>();
    public DbSet<LocationInventorySettings> LocationInventorySettings => Set<LocationInventorySettings>();
    public DbSet<ProductLocationPrice> ProductLocationPrices => Set<ProductLocationPrice>();
    public DbSet<SerialCounter> SerialCounters => Set<SerialCounter>();

    // Sales
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<SaleHistory> SaleHistories => Set<SaleHistory>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentAccount> PaymentAccounts => Set<PaymentAccount>();
    public DbSet<PaymentMethodMapping> PaymentMethodMappings => Set<PaymentMethodMapping>();
    public DbSet<PaymentGatewayTransaction> PaymentGatewayTransactions => Set<PaymentGatewayTransaction>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<CampaignProductRule> CampaignProductRules => Set<CampaignProductRule>();
    public DbSet<MarketingCampaign> MarketingCampaigns => Set<MarketingCampaign>();
    public DbSet<CampaignTargetLocation> CampaignTargetLocations => Set<CampaignTargetLocation>();
    public DbSet<Offer> Offers => Set<Offer>();

    // Purchase
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<PurchaseReturn> PurchaseReturns => Set<PurchaseReturn>();
    public DbSet<PurchaseReturnItem> PurchaseReturnItems => Set<PurchaseReturnItem>();

    // Subscription
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<CompanySubscription> CompanySubscriptions => Set<CompanySubscription>();
    public DbSet<SubscriptionLocation> SubscriptionLocations => Set<SubscriptionLocation>();
    public DbSet<BillingInvoice> BillingInvoices => Set<BillingInvoice>();
    public DbSet<BillingPayment> BillingPayments => Set<BillingPayment>();
    public DbSet<SubscriptionChangeLog> SubscriptionChangeLogs => Set<SubscriptionChangeLog>();
    public DbSet<TrialPlan> TrialPlans => Set<TrialPlan>();

  

   

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var userId = _currentUser.UserId;

        var auditEntries = new List<AuditEntry>();

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                EntityName = entry.Entity.GetType().Name,
                UserId = userId,
                Action = entry.State.ToString()
            };
            
            if (entry.Entity is TenantEntity te)
            {
                auditEntry.CompanyId = te.CompanyId;
            }

            auditEntries.Add(auditEntry);

            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue!;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.NewValues[propertyName] = property.CurrentValue!;
                        break;

                    case EntityState.Deleted:
                        auditEntry.OldValues[propertyName] = property.OriginalValue!;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.ChangedColumns.Add(propertyName);
                            auditEntry.OldValues[propertyName] = property.OriginalValue!;
                            auditEntry.NewValues[propertyName] = property.CurrentValue!;
                        }
                        break;
                }
            }

            // Fill standard auditable fields
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = userId;
            }
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = userId;
            }
        }

        // Convert entries to entities
        foreach (var auditEntry in auditEntries)
        {
            AuditLogs.Add(auditEntry.ToAuditLog());
        }

        // Collect domain events
        var domainEvents = ChangeTracker.Entries<BaseEntity>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            entry.Entity.ClearDomainEvents();

        var result = await base.SaveChangesAsync(cancellationToken);

        // Publish domain events after save
        foreach (var ev in domainEvents)
            await _publisher.Publish(ev, cancellationToken);

        return result;
    }

    private class AuditEntry
    {
        public AuditEntry(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry) { }
        public Guid CompanyId { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public Dictionary<string, object> KeyValues { get; } = new();
        public Dictionary<string, object> OldValues { get; } = new();
        public Dictionary<string, object> NewValues { get; } = new();
        public List<string> ChangedColumns { get; } = new();

        public AuditLog ToAuditLog()
        {
            return new AuditLog
            {
                CompanyId = CompanyId,
                UserId = UserId,
                EntityName = EntityName,
                EntityId = System.Text.Json.JsonSerializer.Serialize(KeyValues),
                Action = Action,
                OldValues = OldValues.Count == 0 ? null : System.Text.Json.JsonSerializer.Serialize(OldValues),
                NewValues = NewValues.Count == 0 ? null : System.Text.Json.JsonSerializer.Serialize(NewValues),
                ChangedColumns = ChangedColumns.Count == 0 ? null : System.Text.Json.JsonSerializer.Serialize(ChangedColumns),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filter: filter IsActive == true on all TenantEntity DbSets
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(TenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(ApplyTenantEntityFilter),
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(null, new object[] { modelBuilder });
            }
        }
    }

    private static void ApplyTenantEntityFilter<T>(ModelBuilder modelBuilder)
        where T : TenantEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => e.IsActive);
    }
}
