using Fenicia.Common.Data.Requests.Basic;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.Basic;

public class CustomerRequestValidation : AbstractValidator<CustomerRequest>
{
    public CustomerRequestValidation()
    {
        RuleFor(x => x.Person)
            .NotNull()
            .WithMessage("Person cannot be null");
    }
}