using FluentValidation;

namespace Flowtap_Application.Features.Organization.Tenant.Commands.SeedIndustryData;

public class SeedIndustryDataCommandValidator : AbstractValidator<SeedIndustryDataCommand>
{
    public SeedIndustryDataCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.BusinessType).NotEmpty();
    }
}
