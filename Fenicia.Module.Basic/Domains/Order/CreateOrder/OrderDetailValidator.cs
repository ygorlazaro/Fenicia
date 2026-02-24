using FluentValidation;

namespace Fenicia.Module.Basic.Domains.Order.CreateOrder;

public class OrderDetailValidator : AbstractValidator<OrderDetailCommand>
{
    public OrderDetailValidator()
    {
        RuleFor(d => d.ProductId).NotEmpty();
        RuleFor(d => d.Price).GreaterThan(0);
        RuleFor(d => d.Quantity).GreaterThan(0);
    }
}