using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Product;

public class DeleteProductRequest()
{
    public DeleteProductRequest(Guid id)
        : this()
    {
        Id = id;
    }

    [Required]
    public Guid Id { get; set; }
}
