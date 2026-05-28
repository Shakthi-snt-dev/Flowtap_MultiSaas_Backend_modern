using FluentValidation;

namespace Flowtap_Application.Features.Sales.Commands.CreateClient;

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.DiscountPercent).InclusiveBetween(0, 100);
    }
}
