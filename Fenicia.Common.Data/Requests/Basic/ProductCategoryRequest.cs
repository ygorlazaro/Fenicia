using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data.Requests.Basic;

public class ProductCategoryRequest
{
    public Guid Id
    {
        get;
        set;
    }

    [Required]
    [MaxLength(50)]
    public string Name
    {
        get;
        set;
    }
}