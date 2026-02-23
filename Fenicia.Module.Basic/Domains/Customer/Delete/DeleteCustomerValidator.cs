using FluentValidation;

namespace Fenicia.Module.Basic.Domains.Customer.Delete;

public class DeleteCustomerValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}
