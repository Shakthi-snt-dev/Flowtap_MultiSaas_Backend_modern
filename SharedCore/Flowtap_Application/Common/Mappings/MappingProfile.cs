using AutoMapper;
using Flowtap_Application.Features.Identity.DTOs;
using Flowtap_Application.Features.Inventory.DTOs;
using Flowtap_Application.Features.Organization.Employee.DTOs;
using Flowtap_Application.Features.Organization.Store.DTOs;
using Flowtap_Application.Features.Organization.Tenant.DTOs;
using Flowtap_Application.Features.Purchase.DTOs;
using Flowtap_Application.Features.Sales.DTOs;
using Flowtap_Application.Features.Subscription.DTOs;
using Flowtap_Domain.BoundedContexts.Core.Identity.Entities;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Subscription.Entities;

namespace Flowtap_Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Identity
        CreateMap<UserAccount, UserDto>()
            .ForMember(d => d.Name,        o => o.MapFrom(s => s.Profile != null ? s.Profile.Name : string.Empty))
            .ForMember(d => d.Phone,       o => o.MapFrom(s => s.Profile != null ? s.Profile.Phone : null))
            .ForMember(d => d.HasPassword, o => o.MapFrom(s => s.PasswordHash != null));
        CreateMap<UserSession, SessionDto>();

        // Organization
        CreateMap<Tenant, TenantDto>();
        CreateMap<Store, StoreDto>();
        CreateMap<Store, StoreListItemDto>();
        CreateMap<Employee, EmployeeDto>();
        CreateMap<Employee, EmployeeListItemDto>();
        CreateMap<Permission, PermissionDto>();

        // Inventory
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductListItemDto>();
        CreateMap<ProductVariant, ProductVariantDto>();
        CreateMap<Warehouse, WarehouseDto>();
        CreateMap<WarehouseStock, StockLevelDto>();
        CreateMap<InventoryTransfer, TransferDto>();
        CreateMap<InventoryWriteOff, WriteOffDto>();
        CreateMap<ReorderAlert, ReorderAlertDto>();
        CreateMap<ProductLocationPrice, ProductLocationPriceDto>();

        // Sales
        CreateMap<Client, ClientDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.Address, o => o.MapFrom(s => s.AddressLine1));
        CreateMap<Sale, SaleDto>();
        CreateMap<SaleItem, SaleItemDto>();
        CreateMap<Payment, PaymentDto>();
        CreateMap<Campaign, CampaignDto>();
        CreateMap<Offer, OfferDto>();

        // Purchase
        CreateMap<PurchaseOrder, PurchaseOrderDto>();
        CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>();
        CreateMap<Supplier, SupplierDto>();
        CreateMap<PurchaseReturn, PurchaseReturnDto>();

        // Subscription
        CreateMap<CompanySubscription, SubscriptionDto>();
        CreateMap<SubscriptionPlan, PlanDto>();
        CreateMap<BillingInvoice, InvoiceDto>();
        CreateMap<TrialPlan, TrialDto>();
    }
}
