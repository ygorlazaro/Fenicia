using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.ProductCategory;

public class GetAllProductCategoryResponse() : ICrudItem
{
    public GetAllProductCategoryResponse(Guid id, string name)
        : this()
    {
        Id = id;
        Name = name;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
