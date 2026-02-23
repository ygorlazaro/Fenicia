using FluentValidation;

namespace Fenicia.Module.Basic.Domains.Product.Add;

public class AddProductValidator : AbstractValidator<AddProductCommand>
{
    public AddProductValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.SellingPrice).GreaterThan(0);
        RuleFor(c => c.CategoryId).NotEmpty();
    }
}
