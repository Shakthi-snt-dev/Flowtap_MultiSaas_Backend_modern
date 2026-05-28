using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Flowtap_Domain.SharedKernel;

namespace Flowtap_Infrastructure.Persistence.Seeds;

/// <summary>
/// Default permission seed data covering all bounded context entities.
/// Each entity gets: View, Create, Edit, Delete permissions.
/// Categories map to bounded context modules.
/// </summary>
public static class PermissionSeedData
{
    // ─── Category GUIDs ───────────────────────────────────────────────────────
    public static readonly Guid CatIdentityId       = new("10000000-0000-0000-0000-000000000001");
    public static readonly Guid CatOrganizationId   = new("10000000-0000-0000-0000-000000000002");
    public static readonly Guid CatInventoryId      = new("10000000-0000-0000-0000-000000000003");
    public static readonly Guid CatSalesId          = new("10000000-0000-0000-0000-000000000004");
    public static readonly Guid CatPurchaseId       = new("10000000-0000-0000-0000-000000000005");
    public static readonly Guid CatServiceTicketsId = new("10000000-0000-0000-0000-000000000006");
    public static readonly Guid CatSubscriptionId   = new("10000000-0000-0000-0000-000000000007");
    public static readonly Guid CatReportsId        = new("10000000-0000-0000-0000-000000000008");

    // ─── PermissionCategories ──────────────────────────────────────────────────
    public static IEnumerable<PermissionCategory> GetCategories() =>
    [
        Make<PermissionCategory>(CatIdentityId,       c => { c.Name = "Identity & Users";    c.Description = "User accounts, profiles and sessions";              c.SortOrder = 1; }),
        Make<PermissionCategory>(CatOrganizationId,   c => { c.Name = "Organization";        c.Description = "Tenants, stores, employees and permissions";        c.SortOrder = 2; }),
        Make<PermissionCategory>(CatInventoryId,      c => { c.Name = "Inventory";           c.Description = "Products, stock, warehouses and transfers";         c.SortOrder = 3; }),
        Make<PermissionCategory>(CatSalesId,          c => { c.Name = "Sales";               c.Description = "POS, clients, payments and campaigns";              c.SortOrder = 4; }),
        Make<PermissionCategory>(CatPurchaseId,       c => { c.Name = "Purchase";            c.Description = "Suppliers, purchase orders and returns";            c.SortOrder = 5; }),
        Make<PermissionCategory>(CatServiceTicketsId, c => { c.Name = "Service & Tickets";   c.Description = "Repair tickets, services and work tasks";           c.SortOrder = 6; }),
        Make<PermissionCategory>(CatSubscriptionId,   c => { c.Name = "Subscription";        c.Description = "Subscription plans, billing and invoices";          c.SortOrder = 7; }),
        Make<PermissionCategory>(CatReportsId,        c => { c.Name = "Reports & Audit";     c.Description = "Reports, audit logs and analytics";                c.SortOrder = 8; }),
    ];

    // ─── Permissions ───────────────────────────────────────────────────────────
    public static IEnumerable<Permission> GetPermissions() =>
    [
        // ── IDENTITY ─────────────────────────────────────────────────────────
        Perm("20000000-0000-0000-0001-000000000001", CatIdentityId, "users.view",          "View Users"),
        Perm("20000000-0000-0000-0001-000000000002", CatIdentityId, "users.create",        "Create Users"),
        Perm("20000000-0000-0000-0001-000000000003", CatIdentityId, "users.edit",          "Edit Users"),
        Perm("20000000-0000-0000-0001-000000000004", CatIdentityId, "users.delete",        "Delete Users"),

        Perm("20000000-0000-0000-0001-000000000005", CatIdentityId, "user_profile.view",   "View User Profile"),
        Perm("20000000-0000-0000-0001-000000000006", CatIdentityId, "user_profile.edit",   "Edit User Profile"),

        Perm("20000000-0000-0000-0001-000000000007", CatIdentityId, "user_sessions.view",  "View User Sessions"),
        Perm("20000000-0000-0000-0001-000000000008", CatIdentityId, "user_sessions.delete","Revoke User Sessions"),

        Perm("20000000-0000-0000-0001-000000000009", CatIdentityId, "notifications.view",  "View Notifications"),
        Perm("20000000-0000-0000-0001-000000000010", CatIdentityId, "notifications.edit",  "Manage Notification Settings"),

        // ── ORGANIZATION ──────────────────────────────────────────────────────
        Perm("20000000-0000-0000-0002-000000000001", CatOrganizationId, "tenants.view",         "View Company Settings"),
        Perm("20000000-0000-0000-0002-000000000002", CatOrganizationId, "tenants.edit",         "Edit Company Settings"),

        Perm("20000000-0000-0000-0002-000000000003", CatOrganizationId, "stores.view",          "View Stores"),
        Perm("20000000-0000-0000-0002-000000000004", CatOrganizationId, "stores.create",        "Create Stores"),
        Perm("20000000-0000-0000-0002-000000000005", CatOrganizationId, "stores.edit",          "Edit Stores"),
        Perm("20000000-0000-0000-0002-000000000006", CatOrganizationId, "stores.delete",        "Delete Stores"),

        Perm("20000000-0000-0000-0002-000000000007", CatOrganizationId, "employees.view",       "View Employees"),
        Perm("20000000-0000-0000-0002-000000000008", CatOrganizationId, "employees.create",     "Create Employees"),
        Perm("20000000-0000-0000-0002-000000000009", CatOrganizationId, "employees.edit",       "Edit Employees"),
        Perm("20000000-0000-0000-0002-000000000010", CatOrganizationId, "employees.delete",     "Delete Employees"),

        Perm("20000000-0000-0000-0002-000000000011", CatOrganizationId, "permissions.view",     "View Permissions"),
        Perm("20000000-0000-0000-0002-000000000012", CatOrganizationId, "permissions.manage",   "Manage Employee Permissions"),

        Perm("20000000-0000-0000-0002-000000000013", CatOrganizationId, "salary.view",          "View Salary Settings"),
        Perm("20000000-0000-0000-0002-000000000014", CatOrganizationId, "salary.edit",          "Edit Salary Settings"),

        Perm("20000000-0000-0000-0002-000000000015", CatOrganizationId, "order_types.view",     "View Order Types"),
        Perm("20000000-0000-0000-0002-000000000016", CatOrganizationId, "order_types.create",   "Create Order Types"),
        Perm("20000000-0000-0000-0002-000000000017", CatOrganizationId, "order_types.edit",     "Edit Order Types"),
        Perm("20000000-0000-0000-0002-000000000018", CatOrganizationId, "order_types.delete",   "Delete Order Types"),

        Perm("20000000-0000-0000-0002-000000000019", CatOrganizationId, "tax.view",             "View Tax Configuration"),
        Perm("20000000-0000-0000-0002-000000000020", CatOrganizationId, "tax.create",           "Create Tax Configuration"),
        Perm("20000000-0000-0000-0002-000000000021", CatOrganizationId, "tax.edit",             "Edit Tax Configuration"),
        Perm("20000000-0000-0000-0002-000000000022", CatOrganizationId, "tax.delete",           "Delete Tax Configuration"),

        // ── INVENTORY ─────────────────────────────────────────────────────────
        Perm("20000000-0000-0000-0003-000000000001", CatInventoryId, "products.view",            "View Products"),
        Perm("20000000-0000-0000-0003-000000000002", CatInventoryId, "products.create",          "Create Products"),
        Perm("20000000-0000-0000-0003-000000000003", CatInventoryId, "products.edit",            "Edit Products"),
        Perm("20000000-0000-0000-0003-000000000004", CatInventoryId, "products.delete",          "Delete Products"),

        Perm("20000000-0000-0000-0003-000000000005", CatInventoryId, "product_categories.view",  "View Product Categories"),
        Perm("20000000-0000-0000-0003-000000000006", CatInventoryId, "product_categories.create","Create Product Categories"),
        Perm("20000000-0000-0000-0003-000000000007", CatInventoryId, "product_categories.edit",  "Edit Product Categories"),
        Perm("20000000-0000-0000-0003-000000000008", CatInventoryId, "product_categories.delete","Delete Product Categories"),

        Perm("20000000-0000-0000-0003-000000000009", CatInventoryId, "warehouses.view",          "View Warehouses"),
        Perm("20000000-0000-0000-0003-000000000010", CatInventoryId, "warehouses.create",        "Create Warehouses"),
        Perm("20000000-0000-0000-0003-000000000011", CatInventoryId, "warehouses.edit",          "Edit Warehouses"),
        Perm("20000000-0000-0000-0003-000000000012", CatInventoryId, "warehouses.delete",        "Delete Warehouses"),

        Perm("20000000-0000-0000-0003-000000000013", CatInventoryId, "stock.view",               "View Stock"),
        Perm("20000000-0000-0000-0003-000000000014", CatInventoryId, "stock.adjust",             "Adjust Stock"),

        Perm("20000000-0000-0000-0003-000000000015", CatInventoryId, "stock_transfers.view",     "View Stock Transfers"),
        Perm("20000000-0000-0000-0003-000000000016", CatInventoryId, "stock_transfers.create",   "Create Stock Transfers"),
        Perm("20000000-0000-0000-0003-000000000017", CatInventoryId, "stock_transfers.edit",     "Edit Stock Transfers"),
        Perm("20000000-0000-0000-0003-000000000018", CatInventoryId, "stock_transfers.delete",   "Delete Stock Transfers"),

        Perm("20000000-0000-0000-0003-000000000019", CatInventoryId, "write_offs.view",          "View Write-Offs"),
        Perm("20000000-0000-0000-0003-000000000020", CatInventoryId, "write_offs.create",        "Create Write-Offs"),
        Perm("20000000-0000-0000-0003-000000000021", CatInventoryId, "write_offs.edit",          "Edit Write-Offs"),
        Perm("20000000-0000-0000-0003-000000000022", CatInventoryId, "write_offs.delete",        "Delete Write-Offs"),

        Perm("20000000-0000-0000-0003-000000000023", CatInventoryId, "device_brands.view",       "View Device Brands"),
        Perm("20000000-0000-0000-0003-000000000024", CatInventoryId, "device_brands.create",     "Create Device Brands"),
        Perm("20000000-0000-0000-0003-000000000025", CatInventoryId, "device_brands.edit",       "Edit Device Brands"),
        Perm("20000000-0000-0000-0003-000000000026", CatInventoryId, "device_brands.delete",     "Delete Device Brands"),

        Perm("20000000-0000-0000-0003-000000000027", CatInventoryId, "device_models.view",       "View Device Models"),
        Perm("20000000-0000-0000-0003-000000000028", CatInventoryId, "device_models.create",     "Create Device Models"),
        Perm("20000000-0000-0000-0003-000000000029", CatInventoryId, "device_models.edit",       "Edit Device Models"),
        Perm("20000000-0000-0000-0003-000000000030", CatInventoryId, "device_models.delete",     "Delete Device Models"),

        Perm("20000000-0000-0000-0003-000000000031", CatInventoryId, "barcode_templates.view",   "View Barcode Templates"),
        Perm("20000000-0000-0000-0003-000000000032", CatInventoryId, "barcode_templates.create", "Create Barcode Templates"),
        Perm("20000000-0000-0000-0003-000000000033", CatInventoryId, "barcode_templates.edit",   "Edit Barcode Templates"),
        Perm("20000000-0000-0000-0003-000000000034", CatInventoryId, "barcode_templates.delete", "Delete Barcode Templates"),

        Perm("20000000-0000-0000-0003-000000000035", CatInventoryId, "reorder_rules.view",       "View Reorder Rules"),
        Perm("20000000-0000-0000-0003-000000000036", CatInventoryId, "reorder_rules.create",     "Create Reorder Rules"),
        Perm("20000000-0000-0000-0003-000000000037", CatInventoryId, "reorder_rules.edit",       "Edit Reorder Rules"),
        Perm("20000000-0000-0000-0003-000000000038", CatInventoryId, "reorder_rules.delete",     "Delete Reorder Rules"),

        Perm("20000000-0000-0000-0003-000000000039", CatInventoryId, "inventory_settings.view",  "View Inventory Settings"),
        Perm("20000000-0000-0000-0003-000000000040", CatInventoryId, "inventory_settings.edit",  "Edit Inventory Settings"),

        // ── SALES ─────────────────────────────────────────────────────────────
        Perm("20000000-0000-0000-0004-000000000001", CatSalesId, "pos.access",              "Access POS"),
        Perm("20000000-0000-0000-0004-000000000002", CatSalesId, "pos.discount",            "Apply Discount in POS"),
        Perm("20000000-0000-0000-0004-000000000003", CatSalesId, "pos.refund",              "Process Refund in POS"),
        Perm("20000000-0000-0000-0004-000000000004", CatSalesId, "pos.void",                "Void Sale in POS"),

        Perm("20000000-0000-0000-0004-000000000005", CatSalesId, "sales.view",              "View Sales"),
        Perm("20000000-0000-0000-0004-000000000006", CatSalesId, "sales.create",            "Create Sales"),
        Perm("20000000-0000-0000-0004-000000000007", CatSalesId, "sales.edit",              "Edit Sales"),
        Perm("20000000-0000-0000-0004-000000000008", CatSalesId, "sales.delete",            "Delete Sales"),

        Perm("20000000-0000-0000-0004-000000000009", CatSalesId, "clients.view",            "View Clients"),
        Perm("20000000-0000-0000-0004-000000000010", CatSalesId, "clients.create",          "Create Clients"),
        Perm("20000000-0000-0000-0004-000000000011", CatSalesId, "clients.edit",            "Edit Clients"),
        Perm("20000000-0000-0000-0004-000000000012", CatSalesId, "clients.delete",          "Delete Clients"),

        Perm("20000000-0000-0000-0004-000000000013", CatSalesId, "payments.view",           "View Payments"),
        Perm("20000000-0000-0000-0004-000000000014", CatSalesId, "payments.create",         "Create Payments"),
        Perm("20000000-0000-0000-0004-000000000015", CatSalesId, "payments.edit",           "Edit Payments"),
        Perm("20000000-0000-0000-0004-000000000016", CatSalesId, "payments.delete",         "Delete Payments"),

        Perm("20000000-0000-0000-0004-000000000017", CatSalesId, "payment_accounts.view",   "View Payment Accounts"),
        Perm("20000000-0000-0000-0004-000000000018", CatSalesId, "payment_accounts.create", "Create Payment Accounts"),
        Perm("20000000-0000-0000-0004-000000000019", CatSalesId, "payment_accounts.edit",   "Edit Payment Accounts"),
        Perm("20000000-0000-0000-0004-000000000020", CatSalesId, "payment_accounts.delete", "Delete Payment Accounts"),

        Perm("20000000-0000-0000-0004-000000000021", CatSalesId, "campaigns.view",          "View Campaigns"),
        Perm("20000000-0000-0000-0004-000000000022", CatSalesId, "campaigns.create",        "Create Campaigns"),
        Perm("20000000-0000-0000-0004-000000000023", CatSalesId, "campaigns.edit",          "Edit Campaigns"),
        Perm("20000000-0000-0000-0004-000000000024", CatSalesId, "campaigns.delete",        "Delete Campaigns"),

        Perm("20000000-0000-0000-0004-000000000025", CatSalesId, "offers.view",             "View Offers"),
        Perm("20000000-0000-0000-0004-000000000026", CatSalesId, "offers.create",           "Create Offers"),
        Perm("20000000-0000-0000-0004-000000000027", CatSalesId, "offers.edit",             "Edit Offers"),
        Perm("20000000-0000-0000-0004-000000000028", CatSalesId, "offers.delete",           "Delete Offers"),

        // ── PURCHASE ──────────────────────────────────────────────────────────
        Perm("20000000-0000-0000-0005-000000000001", CatPurchaseId, "suppliers.view",           "View Suppliers"),
        Perm("20000000-0000-0000-0005-000000000002", CatPurchaseId, "suppliers.create",         "Create Suppliers"),
        Perm("20000000-0000-0000-0005-000000000003", CatPurchaseId, "suppliers.edit",           "Edit Suppliers"),
        Perm("20000000-0000-0000-0005-000000000004", CatPurchaseId, "suppliers.delete",         "Delete Suppliers"),

        Perm("20000000-0000-0000-0005-000000000005", CatPurchaseId, "purchase_orders.view",     "View Purchase Orders"),
        Perm("20000000-0000-0000-0005-000000000006", CatPurchaseId, "purchase_orders.create",   "Create Purchase Orders"),
        Perm("20000000-0000-0000-0005-000000000007", CatPurchaseId, "purchase_orders.edit",     "Edit Purchase Orders"),
        Perm("20000000-0000-0000-0005-000000000008", CatPurchaseId, "purchase_orders.delete",   "Delete Purchase Orders"),
        Perm("20000000-0000-0000-0005-000000000009", CatPurchaseId, "purchase_orders.approve",  "Approve Purchase Orders"),

        Perm("20000000-0000-0000-0005-000000000010", CatPurchaseId, "purchase_returns.view",    "View Purchase Returns"),
        Perm("20000000-0000-0000-0005-000000000011", CatPurchaseId, "purchase_returns.create",  "Create Purchase Returns"),
        Perm("20000000-0000-0000-0005-000000000012", CatPurchaseId, "purchase_returns.edit",    "Edit Purchase Returns"),
        Perm("20000000-0000-0000-0005-000000000013", CatPurchaseId, "purchase_returns.delete",  "Delete Purchase Returns"),

        // ── SERVICE TICKETS ────────────────────────────────────────────────────
        Perm("20000000-0000-0000-0006-000000000001", CatServiceTicketsId, "tickets.view",              "View Tickets"),
        Perm("20000000-0000-0000-0006-000000000002", CatServiceTicketsId, "tickets.create",            "Create Tickets"),
        Perm("20000000-0000-0000-0006-000000000003", CatServiceTicketsId, "tickets.edit",              "Edit Tickets"),
        Perm("20000000-0000-0000-0006-000000000004", CatServiceTicketsId, "tickets.delete",            "Delete Tickets"),
        Perm("20000000-0000-0000-0006-000000000005", CatServiceTicketsId, "tickets.assign",            "Assign Tickets"),
        Perm("20000000-0000-0000-0006-000000000006", CatServiceTicketsId, "tickets.close",             "Close Tickets"),

        Perm("20000000-0000-0000-0006-000000000007", CatServiceTicketsId, "services.view",             "View Services"),
        Perm("20000000-0000-0000-0006-000000000008", CatServiceTicketsId, "services.create",           "Create Services"),
        Perm("20000000-0000-0000-0006-000000000009", CatServiceTicketsId, "services.edit",             "Edit Services"),
        Perm("20000000-0000-0000-0006-000000000010", CatServiceTicketsId, "services.delete",           "Delete Services"),

        Perm("20000000-0000-0000-0006-000000000011", CatServiceTicketsId, "service_categories.view",   "View Service Categories"),
        Perm("20000000-0000-0000-0006-000000000012", CatServiceTicketsId, "service_categories.create", "Create Service Categories"),
        Perm("20000000-0000-0000-0006-000000000013", CatServiceTicketsId, "service_categories.edit",   "Edit Service Categories"),
        Perm("20000000-0000-0000-0006-000000000014", CatServiceTicketsId, "service_categories.delete", "Delete Service Categories"),

        Perm("20000000-0000-0000-0006-000000000015", CatServiceTicketsId, "work_tasks.view",            "View Work Tasks"),
        Perm("20000000-0000-0000-0006-000000000016", CatServiceTicketsId, "work_tasks.create",          "Create Work Tasks"),
        Perm("20000000-0000-0000-0006-000000000017", CatServiceTicketsId, "work_tasks.edit",            "Edit Work Tasks"),
        Perm("20000000-0000-0000-0006-000000000018", CatServiceTicketsId, "work_tasks.delete",          "Delete Work Tasks"),

        Perm("20000000-0000-0000-0006-000000000019", CatServiceTicketsId, "technical_faults.view",      "View Technical Faults"),
        Perm("20000000-0000-0000-0006-000000000020", CatServiceTicketsId, "technical_faults.create",    "Create Technical Faults"),
        Perm("20000000-0000-0000-0006-000000000021", CatServiceTicketsId, "technical_faults.edit",      "Edit Technical Faults"),
        Perm("20000000-0000-0000-0006-000000000022", CatServiceTicketsId, "technical_faults.delete",    "Delete Technical Faults"),

        Perm("20000000-0000-0000-0006-000000000023", CatServiceTicketsId, "checklist_templates.view",   "View Checklist Templates"),
        Perm("20000000-0000-0000-0006-000000000024", CatServiceTicketsId, "checklist_templates.create", "Create Checklist Templates"),
        Perm("20000000-0000-0000-0006-000000000025", CatServiceTicketsId, "checklist_templates.edit",   "Edit Checklist Templates"),
        Perm("20000000-0000-0000-0006-000000000026", CatServiceTicketsId, "checklist_templates.delete", "Delete Checklist Templates"),

        // ── SUBSCRIPTION ──────────────────────────────────────────────────────
        Perm("20000000-0000-0000-0007-000000000001", CatSubscriptionId, "subscription_plans.view",    "View Subscription Plans"),
        Perm("20000000-0000-0000-0007-000000000002", CatSubscriptionId, "subscription_plans.create",  "Create Subscription Plans"),
        Perm("20000000-0000-0000-0007-000000000003", CatSubscriptionId, "subscription_plans.edit",    "Edit Subscription Plans"),
        Perm("20000000-0000-0000-0007-000000000004", CatSubscriptionId, "subscription_plans.delete",  "Delete Subscription Plans"),

        Perm("20000000-0000-0000-0007-000000000005", CatSubscriptionId, "company_subscription.view",  "View Company Subscription"),
        Perm("20000000-0000-0000-0007-000000000006", CatSubscriptionId, "company_subscription.manage","Manage Company Subscription"),

        Perm("20000000-0000-0000-0007-000000000007", CatSubscriptionId, "billing_invoices.view",      "View Billing Invoices"),
        Perm("20000000-0000-0000-0007-000000000008", CatSubscriptionId, "billing_payments.view",      "View Billing Payments"),

        // ── REPORTS & AUDIT ────────────────────────────────────────────────────
        Perm("20000000-0000-0000-0008-000000000001", CatReportsId, "reports.view",      "View Reports"),
        Perm("20000000-0000-0000-0008-000000000002", CatReportsId, "reports.export",    "Export Reports"),
        Perm("20000000-0000-0000-0008-000000000003", CatReportsId, "audit_logs.view",   "View Audit Logs"),
        Perm("20000000-0000-0000-0008-000000000004", CatReportsId, "audit_logs.export", "Export Audit Logs"),
    ];

    // ─── Helpers ───────────────────────────────────────────────────────────────
    private static Permission Perm(string id, Guid categoryId, string key, string displayName) =>
        Make<Permission>(new Guid(id), p =>
        {
            p.CategoryId  = categoryId;
            p.Key         = key;
            p.DisplayName = displayName;
            p.Description = displayName;
        });

    /// <summary>
    /// Creates an entity with a deterministic Id using reflection
    /// (bypasses the protected setter on BaseEntity).
    /// </summary>
    private static T Make<T>(Guid id, Action<T> configure) where T : AuditableEntity, new()
    {
        var entity = new T
        {
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };
        // Set the protected Id via reflection
        typeof(T).BaseType!  // AuditableEntity
            .BaseType!        // BaseEntity
            .GetProperty("Id")!
            .SetValue(entity, id);
        configure(entity);
        return entity;
    }
}

