namespace Flowtap_Repair.Domain.Entities;
public class ServiceFinancials
{
    public decimal EstimatedCost { get; set; }
    public decimal Prepayment { get; set; }
    public string? PrepaymentMethod { get; set; }   // Cash | Card | UPI | ...
    public DateTime? PrepaymentPaidAt { get; set; }
    public decimal TotalCost { get; set; }
    public bool IsPaid { get; set; }
}
