using FluentValidation;

namespace Fenicia.Module.Basic.Domains.StockMovement.Add;

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