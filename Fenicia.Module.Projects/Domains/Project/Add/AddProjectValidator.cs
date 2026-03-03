using FluentValidation;

namespace Fenicia.Module.Projects.Domains.Project.Add;

public class AddProjectValidator : AbstractValidator<AddProjectCommand>
{
    public AddProjectValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Title).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Description).MaximumLength(1000).When(c => c.Description != null);
        RuleFor(c => c.Status).NotEmpty();
        RuleFor(c => c.Owner).NotEmpty();
        RuleFor(c => c.StartDate).LessThanOrEqualTo(c => c.EndDate).When(c => c.StartDate.HasValue && c.EndDate.HasValue);
    }
}
