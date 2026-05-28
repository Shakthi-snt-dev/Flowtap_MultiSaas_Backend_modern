using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;

namespace Flowtap_Infrastructure.Services;

public interface ITaxCalculationService
{
    TaxCalculationResult Calculate(decimal grossAmount, TaxSlab slab, bool isInclusive);
}

public class TaxCalculationResult
{
    public decimal TotalTax { get; set; }
    public decimal NetAmount { get; set; }
    public List<TaxComponentBreakdown> Components { get; set; } = [];
}

public class TaxComponentBreakdown
{
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
}

public class TaxCalculationService : ITaxCalculationService
{
    public TaxCalculationResult Calculate(decimal amount, TaxSlab slab, bool isInclusive)
    {
        var result = new TaxCalculationResult();
        var totalRate = slab.RateRules.Sum(r => r.Rate);

        if (totalRate == 0)
        {
            result.NetAmount = amount;
            result.TotalTax = 0;
            return result;
        }

        if (isInclusive)
        {
            // Amount = Net + (Net * TotalRate / 100)
            // Amount = Net * (1 + TotalRate / 100)
            // Net = Amount / (1 + TotalRate / 100)
            result.NetAmount = Math.Round(amount / (1 + totalRate / 100m), 2);
            result.TotalTax = amount - result.NetAmount;
        }
        else
        {
            result.NetAmount = amount;
            result.TotalTax = Math.Round(amount * totalRate / 100m, 2);
        }

        // Breakdown into rules (CGST, SGST, etc)
        foreach (var rule in slab.RateRules)
        {
            decimal ruleAmount;
            if (isInclusive)
            {
                // Rule proportion of the total tax
                ruleAmount = Math.Round(result.TotalTax * (rule.Rate / totalRate), 2);
            }
            else
            {
                ruleAmount = Math.Round(amount * rule.Rate / 100m, 2);
            }

            result.Components.Add(new TaxComponentBreakdown
            {
                Name = rule.ComponentName,
                Rate = rule.Rate,
                Amount = ruleAmount
            });
        }

        return result;
    }
}
