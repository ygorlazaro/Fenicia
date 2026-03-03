using FluentValidation;

namespace Fenicia.Module.Projects.Domains.ProjectStatus.Add;

public class AddProjectStatusValidator : AbstractValidator<AddProjectStatusCommand>
{
    public AddProjectStatusValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Color).NotEmpty().MaximumLength(20);
        RuleFor(c => c.Order).GreaterThanOrEqualTo(0);
    }
}
