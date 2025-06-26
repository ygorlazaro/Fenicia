using Fenicia.Auth.Domains.OrderDetail;
using Fenicia.Auth.Domains.OrderDetail.Data;

using FluentValidation;

namespace Fenicia.Auth.Domains.Order.Data;

public class OrderDetailRequestValidator : AbstractValidator<OrderDetailRequest>
{
    public OrderDetailRequestValidator()
    {
        RuleFor(x => x.ModuleId)
            .NotEmpty().WithMessage("ModuleId is required.");
    }
}
