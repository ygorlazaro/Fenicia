using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Database;

namespace Fenicia.Module.Basic.Contexts.Models;

[Table("product_categories")]
public class ProductCategoryModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    public virtual List<ProductModel> Products { get; set; } = null!;
}