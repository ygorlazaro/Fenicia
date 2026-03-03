using FluentValidation;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask.Add;

public class AddProjectSubtaskValidator : AbstractValidator<AddProjectSubtaskCommand>
{
    public AddProjectSubtaskValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.TaskId).NotEmpty();
        RuleFor(c => c.Title).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Order).GreaterThanOrEqualTo(0);
    }
}
