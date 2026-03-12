namespace Fenicia.Module.Basic.Domains.Product.Responses;

public record GetProductsByCategoryIdResponse(Guid Id, string Name, decimal? CostPrice, decimal  SalesPrice, double Quantity, Guid CategoryId, string CategoryName);