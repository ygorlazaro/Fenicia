using FluentValidation;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Update;

public class UpdateProjectTaskAssigneeValidator : AbstractValidator<UpdateProjectTaskAssigneeCommand>
{
    public UpdateProjectTaskAssigneeValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.TaskId).NotEmpty();
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.Role).NotEmpty();
        RuleFor(c => c.AssignedAt).NotEmpty();
    }
}
