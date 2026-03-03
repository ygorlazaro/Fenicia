using FluentValidation;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment.Add;

public class AddProjectAttachmentValidator : AbstractValidator<AddProjectAttachmentCommand>
{
    public AddProjectAttachmentValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.TaskId).NotEmpty();
        RuleFor(c => c.FileName).NotEmpty().MaximumLength(255);
        RuleFor(c => c.FileUrl).NotEmpty();
        RuleFor(c => c.FileSize).GreaterThanOrEqualTo(0);
        RuleFor(c => c.UploadedBy).NotEmpty();
    }
}
