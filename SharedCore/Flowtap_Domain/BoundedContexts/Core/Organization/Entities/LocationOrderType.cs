using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class LocationOrderType : BaseEntity
{
    public Guid LocationId { get; set; }
    public Store Location { get; set; } = null!;
    public Guid OrderTypeId { get; set; }
    public OrderType OrderType { get; set; } = null!;
}
