using Fenicia.Common.Enums.Basic;

using FluentValidation;

namespace Fenicia.Module.Basic.Domains.StockMovement.Update;

public class UpdateStockMovementValidator : AbstractValidator<UpdateStockMovementCommand>
{
    public UpdateStockMovementValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Quantity).GreaterThan(0);
        RuleFor(c => c.Price).GreaterThan(0);
        RuleFor(c => c.ProductId).NotEmpty();
        RuleFor(c => c.Reason).MaximumLength(255).When(c => !string.IsNullOrEmpty(c.Reason));
        RuleFor(c => c.OrderId).NotEmpty().When(c => c.Type == StockMovementType.Out);
    }
}
