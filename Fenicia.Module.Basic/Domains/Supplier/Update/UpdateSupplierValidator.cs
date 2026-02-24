using FluentValidation;

namespace Fenicia.Module.Basic.Domains.Supplier.Update;

public class UpdateSupplierValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}