using FluentValidation;

namespace Fenicia.Module.Projects.Domains.ProjectComment.Update;

public class UpdateProjectCommentValidator : AbstractValidator<UpdateProjectCommentCommand>
{
    public UpdateProjectCommentValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Content).NotEmpty().MaximumLength(1000);
    }
}
