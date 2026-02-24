using FluentValidation;

namespace Fenicia.Module.Basic.Domains.Order.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.CustomerId).NotEmpty();
        RuleFor(c => c.Details).NotEmpty();
        RuleForEach(c => c.Details).SetValidator(new OrderDetailValidator());
    }
}