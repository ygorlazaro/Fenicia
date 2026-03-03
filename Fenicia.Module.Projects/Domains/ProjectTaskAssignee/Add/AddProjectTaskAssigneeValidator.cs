using FluentValidation;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Add;

public class AddProjectTaskAssigneeValidator : AbstractValidator<AddProjectTaskAssigneeCommand>
{
    public AddProjectTaskAssigneeValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.TaskId).NotEmpty();
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.Role).NotEmpty();
        RuleFor(c => c.AssignedAt).NotEmpty();
    }
}
