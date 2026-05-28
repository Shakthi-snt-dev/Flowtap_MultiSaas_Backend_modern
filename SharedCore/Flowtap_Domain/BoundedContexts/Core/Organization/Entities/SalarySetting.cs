using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class SalarySetting : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public decimal TicketSalaryPercent { get; set; }
    public decimal ServicesSalaryPercent { get; set; }
    public decimal PartsSalaryPercent { get; set; }
    public decimal SalesSalaryPercent { get; set; }
    public decimal? FixedSalary { get; set; }
}
