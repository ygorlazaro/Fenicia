namespace Fenicia.Module.Basic.Domains.Product;

public record ProductResponse(
    Guid Id,
    string Name,
    decimal? CostPrice,
    decimal SalesPrice,
    double Quantity,
    Guid CategoryId,
    string CategoryName);
