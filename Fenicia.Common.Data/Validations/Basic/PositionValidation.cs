using Fenicia.Common.Data.Requests.Basic;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.Basic;

public class PositionValidation : AbstractValidator<PositionRequest>
{
    public PositionValidation()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .WithMessage("Name cannot be null")
            .MaximumLength(50)
            .WithMessage("Name cannot exceed 50 characters");
    }
}