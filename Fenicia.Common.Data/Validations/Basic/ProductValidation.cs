using Fenicia.Common.Data.Requests.Basic;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.Basic;

public class ProductValidation : AbstractValidator<ProductRequest>
{
    public ProductValidation()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .WithMessage("Name cannot be null")
            .MaximumLength(50)
            .WithMessage("Name cannot exceed 50 characters");

        RuleFor(x => x.CostPrice)
            .GreaterThan(0)
            .WithMessage("CostPrice must be greater than 0");

        RuleFor(x => x.SellingPrice)
            .GreaterThan(0)
            .WithMessage("SellingPrice must be greater than 0")
            .NotNull()
            .WithMessage("SellingPrice cannot be null");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .NotNull()
            .WithMessage("Quantity cannot be null");

        RuleFor(x => x.CategoryId)
            .NotNull()
            .WithMessage("CategoryId cannot be null");
    }
}