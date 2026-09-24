using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.ProductCategory;

public class AddProductCategoryRequest()
{
    public AddProductCategoryRequest(Guid id, string name)
        : this()
    {
        Id = id;
        Name = name;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
