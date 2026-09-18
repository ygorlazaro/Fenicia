using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.ProductCategory;

public class GetProductCategoryByIdQuery
{
    public GetProductCategoryByIdQuery()
    {
    }

    public GetProductCategoryByIdQuery(Guid id)
    {
        Id = id;
    }

    [Required]
    public Guid Id { get; set; }
}
