namespace Flowtap_Domain.SharedKernel.ValueObjects;

public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency required.", nameof(currency));
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency) throw new InvalidOperationException("Cannot add different currencies.");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency) throw new InvalidOperationException("Cannot subtract different currencies.");
        return new Money(Amount - other.Amount, Currency);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:F2} {Currency}";
}
