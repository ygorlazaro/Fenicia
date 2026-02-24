using FluentValidation;

namespace Fenicia.Module.Basic.Domains.Position.Add;

public class AddPositionValidator : AbstractValidator<AddPositionCommand>
{
    public AddPositionValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}