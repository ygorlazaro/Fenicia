namespace Fenicia.Module.Basic.Domains.Order.CreateOrder;

public record OrderDetailCommand(
    Guid ProductId,
    decimal Price,
    double Quantity);