using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.PersonAddress;

public class PersonAddressRequest()
{
    public PersonAddressRequest(Guid personId, Guid addressId)
        : this()
    {
        PersonId = personId;
        AddressId = addressId;
    }

    [Required]
    public Guid PersonId { get; set; }

    [Required]
    public Guid AddressId { get; set; }
}
