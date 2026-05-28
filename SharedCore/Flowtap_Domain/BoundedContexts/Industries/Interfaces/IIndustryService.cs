using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;

namespace Flowtap_Domain.BoundedContexts.Industries.Interfaces;

public interface IIndustryService
{
    Task SeedDataAsync(Tenant tenant, CancellationToken cancellationToken = default);
    IEnumerable<string> GetDefaultModules();
    bool Validate(Tenant tenant);
}
