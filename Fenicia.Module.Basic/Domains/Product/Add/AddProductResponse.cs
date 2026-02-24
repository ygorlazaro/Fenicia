namespace Fenicia.Module.Basic.Domains.Product.Add;

public record AddProductResponse(
    Guid Id,
    string Name,
    decimal? CostPrice,
    decimal SalesPrice,
    double Quantity,
    Guid CategoryId,
    string CategoryName);
