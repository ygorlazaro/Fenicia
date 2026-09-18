using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

public class GetInventoryByProductQuery()
{
    public GetInventoryByProductQuery(Guid productId, int page = 1, int perPage = 10)
        : this()
    {
        ProductId = productId;
        Page = page;
        PerPage = perPage;
    }

    [Required]
    public Guid ProductId { get; set; }

    public int Page { get; set; }

    public int PerPage { get; set; }
}
