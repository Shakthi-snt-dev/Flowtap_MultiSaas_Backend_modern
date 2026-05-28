using FluentValidation;

namespace Flowtap_Application.Features.Inventory.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Kind).NotEmpty();
        RuleFor(x => x.DefaultCostPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DefaultSalePrice).GreaterThanOrEqualTo(0);
    }
}
