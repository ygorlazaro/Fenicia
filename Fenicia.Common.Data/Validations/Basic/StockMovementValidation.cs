using Fenicia.Common.Data.Requests.Basic;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.Basic;

public class StockMovementValidation : AbstractValidator<StockMovementRequest>
{
    public StockMovementValidation()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .NotNull()
            .WithMessage("Quantity cannot be null");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0")
            .NotNull()
            .WithMessage("Price cannot be null");

        RuleFor(x => x.Type)
            .NotNull()
            .WithMessage("Type cannot be null");

        RuleFor(x => x.ProductId)
            .NotNull()
            .WithMessage("ProductId cannot be null");
    }
}