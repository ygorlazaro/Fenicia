using Fenicia.Common.Data.Requests.Basic;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.Basic;

public class OrderDetailRequestValidation : AbstractValidator<OrderDetailRequest>
{
    public OrderDetailRequestValidation()
    {
        RuleFor(x => x.ProductId)
            .NotNull()
            .WithMessage("ProductId cannot be null");

        RuleFor(x => x.Quantity)
            .NotNull()
            .WithMessage("Quantity cannot be null")
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Price)
            .NotNull()
            .WithMessage("Price cannot be null")
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");
    }
}