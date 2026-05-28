namespace Flowtap_Domain.Exceptions;

public class UnauthorizedTenantAccessException : DomainException
{
    public UnauthorizedTenantAccessException()
        : base("Access to this tenant's resource is not authorized.") { }
}
