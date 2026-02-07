using Fenicia.Common.Data.Requests.Basic;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.Basic;

public class ProductCategoryValidation : AbstractValidator<ProductCategoryRequest>
{
    public ProductCategoryValidation()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .WithMessage("Name cannot be null")
            .MaximumLength(50)
            .WithMessage("Name cannot exceed 50 characters");
    }
}