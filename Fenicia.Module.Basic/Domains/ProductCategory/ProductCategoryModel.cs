namespace Fenicia.Module.Basic.Domains.ProductCategory;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Common.Database;

using Product;

[Table(name: "product_categories")]
public class ProductCategoryModel : BaseModel
{
    [Required]
    [MaxLength(length: 50)]
    public string Name { get; set; } = null!;

    public virtual List<ProductModel> Products { get; set; } = null!;
}
