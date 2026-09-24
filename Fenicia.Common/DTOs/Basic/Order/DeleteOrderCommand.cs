using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

public class DeleteOrderRequest()
{
    public DeleteOrderRequest(Guid id)
        : this()
    {
        Id = id;
    }

    [Required]
    public Guid Id { get; set; }
}
