using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data.Requests.Basic;

public class ProductRequest
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

    public decimal? CostPrice
    {
        get;
        set;
    }

    [Required]
    public decimal SellingPrice
    {
        get;
        set;
    }

    public int Quantity
    {
        get;
        set;
    }

    [Required]
    public Guid CategoryId
    {
        get;
        set;
    }
}
