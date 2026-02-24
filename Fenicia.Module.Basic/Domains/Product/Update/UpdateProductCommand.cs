namespace Fenicia.Module.Basic.Domains.Product.Update;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    decimal? CostPrice,
    decimal SellingPrice,
    double Quantity,
    Guid CategoryId);