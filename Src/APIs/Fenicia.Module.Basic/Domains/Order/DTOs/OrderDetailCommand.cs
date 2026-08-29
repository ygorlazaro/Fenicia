namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record OrderDetailCommand(

    Guid ProductId,

    decimal Price,

    double Quantity,

    decimal DiscountAmount = 0);
