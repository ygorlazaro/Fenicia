using FluentValidation;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Update;

public class UpdateProductCategoryValidator : AbstractValidator<UpdateProductCategoryCommand>
{
    public UpdateProductCategoryValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}