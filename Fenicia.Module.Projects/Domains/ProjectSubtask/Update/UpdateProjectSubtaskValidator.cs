using FluentValidation;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask.Update;

public class UpdateProjectSubtaskValidator : AbstractValidator<UpdateProjectSubtaskCommand>
{
    public UpdateProjectSubtaskValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.TaskId).NotEmpty();
        RuleFor(c => c.Title).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Order).GreaterThanOrEqualTo(0);
    }
}
