using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Flowtap_Domain.BoundedContexts.Industries.Interfaces;

namespace Flowtap_Domain.BoundedContexts.Industries.Repair;

public class RepairIndustryService : IIndustryService
{
    public Task SeedDataAsync(Tenant tenant, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public IEnumerable<string> GetDefaultModules()
        => ["ServiceTickets", "Inventory", "Sales", "Clients", "Tasks", "Reports"];

    public bool Validate(Tenant tenant) => true;
}
