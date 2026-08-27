namespace Fenicia.Module.Basic.Domains.Order.DTOs.Commands;

public record OrderDetailCommand(

    Guid ProductId,

    decimal Price,

    double Quantity,

    decimal DiscountAmount = 0);