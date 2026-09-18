using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Product;

public class GetProductByIdQuery
{
    public GetProductByIdQuery()
    {
    }

    public GetProductByIdQuery(Guid id)
    {
        Id = id;
    }

    [Required]
    public Guid Id { get; set; }
}
