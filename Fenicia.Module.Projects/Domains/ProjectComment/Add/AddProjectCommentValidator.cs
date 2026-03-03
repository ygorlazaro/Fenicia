using FluentValidation;

namespace Fenicia.Module.Projects.Domains.ProjectComment.Add;

public class AddProjectCommentValidator : AbstractValidator<AddProjectCommentCommand>
{
    public AddProjectCommentValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.TaskId).NotEmpty();
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.Content).NotEmpty().MaximumLength(1000);
    }
}
