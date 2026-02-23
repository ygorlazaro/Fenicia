using FluentValidation;

namespace Fenicia.Module.Basic.Domains.Employee.Update;

public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.PositionId).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}
