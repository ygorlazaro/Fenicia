using FluentValidation;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

public class AddProductCategoryValidator : AbstractValidator<AddProductCategoryCommand>
{
    public AddProductCategoryValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}

public class UpdateProductCategoryValidator : AbstractValidator<UpdateProductCategoryCommand>
{
    public UpdateProductCategoryValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}
