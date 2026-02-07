using Fenicia.Common.Data.Requests.Auth;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.Auth;

public class OrderDetailValidation : AbstractValidator<OrderDetailRequest>
{
    public OrderDetailValidation()
    {
        RuleFor(x => x.ModuleId)
            .NotEmpty()
            .WithMessage("ModuleId is required");
    }
}