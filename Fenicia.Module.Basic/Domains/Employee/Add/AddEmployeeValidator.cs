using FluentValidation;

namespace Fenicia.Module.Basic.Domains.Employee.Add;

public class AddEmployeeValidator : AbstractValidator<AddEmployeeCommand>
{
    public AddEmployeeValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.PositionId).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}
