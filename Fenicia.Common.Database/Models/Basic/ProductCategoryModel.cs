using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Database.Models.Basic;

[Table("product_categories")]
public class ProductCategoryModel : BaseModel
{
    [Required]
    [MaxLength(length: 50)]
    public string Name { get; set; } = null!;

    public virtual List<ProductModel> Products { get; set; } = null!;
}
