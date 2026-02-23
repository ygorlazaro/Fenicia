namespace Fenicia.Module.Basic.Domains.Product.GetAll;

public record GetAllProductQuery(int Page = 1, int PerPage = 10);
public record GetProductByIdQuery(Guid Id);
public record GetProductsByCategoryIdQuery(Guid CategoryId, int Page = 1, int PerPage = 10);
