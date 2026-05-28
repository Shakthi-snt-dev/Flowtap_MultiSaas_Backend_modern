using FluentValidation;

namespace Flowtap_Application.Features.Sales.Commands.CreateSale;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        // ClientId is intentionally optional — null means walk-in customer
        RuleFor(x => x.Items).NotEmpty().WithMessage("Sale must have at least one item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId).NotEmpty();
            item.RuleFor(x => x.Quantity).GreaterThan(0);
            item.RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        });
    }
}
