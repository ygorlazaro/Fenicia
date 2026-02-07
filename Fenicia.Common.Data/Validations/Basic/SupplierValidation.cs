using Fenicia.Common.Data.Requests.Basic;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.Basic;

public class SupplierValidation : AbstractValidator<SupplierRequest>
{
    public SupplierValidation()
    {
        RuleFor(x => x.Person)
            .NotNull()
            .WithMessage("Person cannot be null");
    }
}