using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.PersonAddress;

public class GetPersonAddressResponse()
{
    public GetPersonAddressResponse(Guid id, Guid personId, Guid addressId, string personName)
        : this()
    {
        Id = id;
        PersonId = personId;
        AddressId = addressId;
        PersonName = personName;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    public Guid PersonId { get; set; }

    [Required]
    public Guid AddressId { get; set; }

    [Required]
    [MaxLength(50)]
    public string PersonName { get; set; } = string.Empty;
}
