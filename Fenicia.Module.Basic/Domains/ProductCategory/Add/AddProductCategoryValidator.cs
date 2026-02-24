using FluentValidation;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Add;

public class AddProductCategoryValidator : AbstractValidator<AddProductCategoryCommand>
{
    public AddProductCategoryValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}