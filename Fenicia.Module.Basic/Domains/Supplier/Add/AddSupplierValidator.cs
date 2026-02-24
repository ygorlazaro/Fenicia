using FluentValidation;

namespace Fenicia.Module.Basic.Domains.Supplier.Add;

public class AddSupplierValidator : AbstractValidator<AddSupplierCommand>
{
    public AddSupplierValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}