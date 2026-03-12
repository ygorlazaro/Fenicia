namespace Fenicia.Module.Basic.Domains.Order.Commands;

public record OrderDetailCommand(
    Guid ProductId,
    decimal Price,
    double Quantity);
