using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Product;

public class GetProductsByCategoryIdQuery
{
    public GetProductsByCategoryIdQuery()
    {
    }

    public GetProductsByCategoryIdQuery(Guid categoryId, int page = 1, int perPage = 10)
    {
        CategoryId = categoryId;
        Page = page;
        PerPage = perPage;
    }

    [Required]
    public Guid CategoryId { get; set; }

    public int Page { get; set; } = 1;

    public int PerPage { get; set; } = 10;
}
