using FluentValidation;
using Fenicia.Auth.Domains.OrderDetail;

namespace Fenicia.Auth.Domains.Order;

public class OrderRequestValidator : AbstractValidator<OrderRequest>
{
    public OrderRequestValidator()
    {
        RuleFor(x => x.Details)
            .NotEmpty().WithMessage("Details are required.")
            .ForEach(detail => detail.SetValidator(new OrderDetailRequestValidator()));
    }
}
