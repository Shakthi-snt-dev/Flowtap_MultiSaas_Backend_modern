using Flowtap_Domain.SharedKernel;
namespace Flowtap_Repair.Domain.Entities;
public class ServiceCategory : TenantEntity
{
    public Guid? ParentCategoryId { get; set; }
    public ServiceCategory? ParentCategory { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public string? IconUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSubCategoryExist { get; set; }
    public bool IsDirectProductExist { get; set; }
    public bool IsBrandExist { get; set; }
    public ICollection<ServiceCategory> SubCategories { get; set; } = [];
    public ICollection<Service> Services { get; set; } = [];
}
