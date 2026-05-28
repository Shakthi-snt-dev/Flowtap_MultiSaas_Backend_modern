namespace Flowtap_Domain.Exceptions;

public class TenantNotFoundException : DomainException
{
    public TenantNotFoundException(Guid tenantId)
        : base($"Tenant '{tenantId}' was not found.") { }
}
