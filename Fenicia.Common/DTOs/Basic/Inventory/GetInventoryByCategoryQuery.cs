using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

public class GetInventoryByCategoryQuery()
{
    public GetInventoryByCategoryQuery(Guid categoryId, int page = 1, int perPage = 10)
        : this()
    {
        CategoryId = categoryId;
        Page = page;
        PerPage = perPage;
    }

    [Required]
    public Guid CategoryId { get; set; }

    public int Page { get; set; }

    public int PerPage { get; set; }
}
