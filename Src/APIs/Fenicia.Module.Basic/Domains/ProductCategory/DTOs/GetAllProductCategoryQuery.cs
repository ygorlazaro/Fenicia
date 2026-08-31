namespace Fenicia.Module.Basic.Domains.ProductCategory.DTOs;

public record GetAllProductCategoryQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);
