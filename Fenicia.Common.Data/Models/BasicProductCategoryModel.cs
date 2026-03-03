using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("product_categories", Schema = "basic")]
public class BasicProductCategoryModel : BaseCompanyModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    public List<BasicProductModel> Products { get; set; } = null!;
}
