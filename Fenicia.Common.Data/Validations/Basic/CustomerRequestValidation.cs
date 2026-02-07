using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Enums.Basic;

using FluentValidation;

using Microsoft.AspNetCore.Http.Timeouts;

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