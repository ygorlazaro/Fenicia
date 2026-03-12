namespace Fenicia.Module.Basic.Domains.Product.Commands;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    decimal? CostPrice,
    decimal SalesPrice,
    double Quantity,
    Guid CategoryId,
    Guid? SupplierId);
