using Fenicia.Common.Data.Requests.Basic;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.Basic;

public class EmployeeRequestValidation : AbstractValidator<EmployeeRequest>
{
    public EmployeeRequestValidation()
    {
        RuleFor(x => x.Person)
            .NotNull()
            .WithMessage("Person cannot be null");

        RuleFor(x => x.PositionId)
            .NotNull()
            .WithMessage("PositionId cannot be null");
    }
}