namespace Fenicia.Module.Basic.Domains.Product.DTOs;

public record GetProductsByCategoryIdQuery(Guid CategoryId, int Page = 1, int PerPage = 10);
