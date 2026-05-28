using FluentValidation;

namespace Flowtap_Application.Features.Organization.Tenant.Commands.CreateTenant;

public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.SubDomain).NotEmpty().MaximumLength(50).Matches("^[a-z0-9-]+$")
            .WithMessage("SubDomain can only contain lowercase letters, numbers, and hyphens.");
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
        RuleFor(x => x.IndustryType).NotEmpty();
        RuleFor(x => x.BusinessType).NotEmpty();
    }
}
