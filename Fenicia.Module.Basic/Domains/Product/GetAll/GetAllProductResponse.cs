namespace Fenicia.Module.Basic.Domains.Product.GetAll;

public record GetAllProductResponse(
    Guid Id,
    string Name,
    decimal? CostPrice,
    decimal SalesPrice,
    double Quantity,
    Guid CategoryId,
    string CategoryName);
