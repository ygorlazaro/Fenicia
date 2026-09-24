using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Customer;

public class DeleteCustomerRequest()
{
    public DeleteCustomerRequest(Guid id)
        : this()
    {
        Id = id;
    }

    [Required]
    public Guid Id { get; set; }
}
