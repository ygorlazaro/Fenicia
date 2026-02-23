using FluentValidation;

namespace Fenicia.Module.Basic.Domains.Employee.Delete;

public class DeleteEmployeeValidator : AbstractValidator<DeleteEmployeeCommand>
{
    public DeleteEmployeeValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}
