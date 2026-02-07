using Fenicia.Common.Data.Requests.Auth;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.Auth;

public class OrderValidation : AbstractValidator<OrderRequest>
{
    public OrderValidation()
    {
        RuleFor(x => x.Details)
            .NotEmpty()
            .WithMessage("Details is required")
            .Must(x => x.Any())
            .WithMessage("Details must contain at least one item");
    }
}