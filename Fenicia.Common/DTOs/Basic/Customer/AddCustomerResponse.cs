using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Customer;

public class AddCustomerResponse()
{
    public AddCustomerResponse(Guid id, Guid personId)
        : this()
    {
        Id = id;
        PersonId = personId;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid PersonId { get; set; }
}
