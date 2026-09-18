using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.PersonAddress;

public class AddPersonAddressCommand()
{
    public AddPersonAddressCommand(Guid personId, Guid addressId)
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
