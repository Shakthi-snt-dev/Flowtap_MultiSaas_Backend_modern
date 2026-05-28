using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
public class PaymentAccount : TenantEntity
{
    public Guid? LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public PaymentAccountType Type { get; set; }
    public new bool IsActive { get; set; } = true;
    public ICollection<Payment> Payments { get; set; } = [];
    public ICollection<PaymentMethodMapping> MethodMappings { get; set; } = [];
}

