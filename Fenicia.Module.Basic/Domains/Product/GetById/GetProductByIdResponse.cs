namespace Fenicia.Module.Basic.Domains.Product.GetById;

public record GetProductByIdResponse(
    Guid Id,
    string Name,
    decimal? CostPrice,
    decimal SalesPrice,
    double Quantity,
    Guid CategoryId,
    string CategoryName);