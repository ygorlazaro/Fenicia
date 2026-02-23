using FluentValidation;

namespace Fenicia.Module.Basic.Domains.StockMovement;

public class AddStockMovementValidator : AbstractValidator<AddStockMovementCommand>
{
    public AddStockMovementValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Quantity).GreaterThan(0);
        RuleFor(c => c.Price).GreaterThan(0);
        RuleFor(c => c.ProductId).NotEmpty();
    }
}

public class UpdateStockMovementValidator : AbstractValidator<UpdateStockMovementCommand>
{
    public UpdateStockMovementValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Quantity).GreaterThan(0);
        RuleFor(c => c.Price).GreaterThan(0);
        RuleFor(c => c.ProductId).NotEmpty();
    }
}
