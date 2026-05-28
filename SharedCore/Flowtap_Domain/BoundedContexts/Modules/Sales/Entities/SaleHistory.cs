using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
public class SaleHistory : BaseEntity
{
    public Guid SaleId { get; set; }
    public Sale Sale { get; set; } = null!;
    public string Message { get; set; } = string.Empty;
    public string? PerformedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
