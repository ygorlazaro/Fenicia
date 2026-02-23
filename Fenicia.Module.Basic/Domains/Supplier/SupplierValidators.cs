using FluentValidation;

namespace Fenicia.Module.Basic.Domains.Supplier;

public class AddSupplierValidator : AbstractValidator<AddSupplierCommand>
{
    public AddSupplierValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}

public class UpdateSupplierValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}
