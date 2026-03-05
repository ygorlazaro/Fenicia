namespace Fenicia.Module.Basic.Domains.Product.Add;

public record AddProductCommand(
    Guid Id,
    string Name,
    decimal? CostPrice,
    decimal SalesPrice,
    double Quantity,
    Guid CategoryId,
    Guid? SupplierId);
