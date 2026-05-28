using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class OrderType : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Color { get; set; }
    public ICollection<LocationOrderType> LocationOrderTypes { get; set; } = [];
}
