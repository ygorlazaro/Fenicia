using FluentValidation;

namespace Fenicia.Auth.Domains.OrderDetail;

public class OrderDetailRequestValidator : AbstractValidator<OrderDetailRequest>
{
    public OrderDetailRequestValidator()
    {
        RuleFor(x => x.ModuleId)
            .NotEmpty().WithMessage("ModuleId is required.");
    }
}
