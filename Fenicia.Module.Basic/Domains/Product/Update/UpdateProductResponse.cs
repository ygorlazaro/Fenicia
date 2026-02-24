namespace Fenicia.Module.Basic.Domains.Product.Update;

public record UpdateProductResponse(Guid Id, string Name, decimal? CostPrice, decimal SalesPrice, double Quantity, Guid CategoryId, string CategoryName);