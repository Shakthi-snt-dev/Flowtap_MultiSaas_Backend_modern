namespace Flowtap_Domain.SharedKernel.ValueObjects;

public class Address : ValueObject
{
    public string Line1 { get; }
    public string? Line2 { get; }
    public string City { get; }
    public string? State { get; }
    public string Country { get; }
    public string? PostalCode { get; }

    public Address(string line1, string city, string country, string? line2 = null, string? state = null, string? postalCode = null)
    {
        Line1 = line1;
        Line2 = line2;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Line1;
        yield return Line2;
        yield return City;
        yield return State;
        yield return Country;
        yield return PostalCode;
    }
}
