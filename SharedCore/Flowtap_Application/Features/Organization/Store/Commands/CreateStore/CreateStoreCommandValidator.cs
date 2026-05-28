using FluentValidation;

namespace Flowtap_Application.Features.Organization.Store.Commands.CreateStore;

public class CreateStoreCommandValidator : AbstractValidator<CreateStoreCommand>
{
    public CreateStoreCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.CountryCode).NotEmpty().Length(2);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
    }
}
