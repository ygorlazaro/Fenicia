namespace Fenicia.Module.Basic.Domains.Product.GetByCategoryId;

public record GetProductsByCategoryIdResponse(Guid Id, string Name, decimal? CostPrice, decimal  SalesPrice, double Quantity, Guid CategoryId, string CategoryName);