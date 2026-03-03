using FluentValidation;

namespace Fenicia.Module.Projects.Domains.ProjectTask.Add;

public class AddProjectTaskValidator : AbstractValidator<AddProjectTaskCommand>
{
    public AddProjectTaskValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.StatusId).NotEmpty();
        RuleFor(c => c.Title).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Description).MaximumLength(1000).When(c => c.Description != null);
        RuleFor(c => c.Priority).NotEmpty();
        RuleFor(c => c.Type).NotEmpty();
        RuleFor(c => c.Order).GreaterThanOrEqualTo(0);
        RuleFor(c => c.EstimatePoints).GreaterThanOrEqualTo(0).When(c => c.EstimatePoints.HasValue);
        RuleFor(c => c.CreatedBy).NotEmpty();
    }
}
