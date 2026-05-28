using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
public class PaymentMethodMapping : TenantEntity
{
    public Guid LocationId { get; set; }
    public PaymentMethod Method { get; set; }
    public Guid PaymentAccountId { get; set; }
    public PaymentAccount PaymentAccount { get; set; } = null!;
}
