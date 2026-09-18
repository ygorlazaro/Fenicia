using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Customer;

public class UpdateCustomerResponse()
{
    public UpdateCustomerResponse(Guid id, Guid personId)
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
