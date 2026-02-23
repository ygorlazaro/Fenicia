using FluentValidation;

namespace Fenicia.Module.Basic.Domains.Product.Update;

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.SellingPrice).GreaterThan(0);
        RuleFor(c => c.CategoryId).NotEmpty();
    }
}
