using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Requests.Basic;

namespace Fenicia.Common.Data.Models.Basic;

[Table("product_categories")]
public class ProductCategoryModel : BaseModel
{
    public ProductCategoryModel(ProductCategoryRequest request)
    {
        this.Id = request.Id;
        this.Name = request.Name;
    }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    public List<ProductModel> Products { get; set; } = null!;
}