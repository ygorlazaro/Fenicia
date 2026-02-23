using FluentValidation;

namespace Fenicia.Module.Basic.Domains.Position;

public class AddPositionValidator : AbstractValidator<AddPositionCommand>
{
    public AddPositionValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}

public class UpdatePositionValidator : AbstractValidator<UpdatePositionCommand>
{
    public UpdatePositionValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}
