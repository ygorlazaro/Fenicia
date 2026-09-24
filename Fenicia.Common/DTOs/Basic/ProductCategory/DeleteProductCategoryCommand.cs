using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.ProductCategory;

public class DeleteProductCategoryRequest()
{
    public DeleteProductCategoryRequest(Guid id)
        : this()
    {
        Id = id;
    }

    [Required]
    public Guid Id { get; set; }
}
