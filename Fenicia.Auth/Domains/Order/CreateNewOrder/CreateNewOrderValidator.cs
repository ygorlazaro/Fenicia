using FluentValidation;

namespace Fenicia.Auth.Domains.Order.CreateNewOrder;

public sealed class CreateNewOrderValidator : AbstractValidator<CreateNewOrderCommand>
{
    public CreateNewOrderValidator()
    {
        RuleFor(c => c.Modules).NotEmpty();
        RuleForEach(c => c.Modules).NotEmpty();
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.CompanyId).NotEmpty();
    }
}