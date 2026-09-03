using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("product_categories", Schema = "basic")]
public class ProductCategoryModel : BaseCompanyModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public List<ProductModel> Products { get; init; } = [];
}