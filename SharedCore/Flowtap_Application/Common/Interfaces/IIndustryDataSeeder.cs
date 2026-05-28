using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;

namespace Flowtap_Application.Common.Interfaces;

public interface IIndustryDataSeeder
{
    IndustryType Industry { get; }
    Task SeedAsync(IApplicationDbContext context, Guid companyId, string businessType, CancellationToken ct);
}
