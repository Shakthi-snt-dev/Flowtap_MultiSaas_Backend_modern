using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Tenant.Commands.SeedIndustryData;

public record SeedIndustryDataCommand(Guid CompanyId, string IndustryType, string BusinessType)
    : IRequest<Result<bool>>;
