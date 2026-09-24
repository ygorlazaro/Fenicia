using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.Address;

namespace Fenicia.Common.DTOs.Basic.Supplier;

public class GetAllSupplierResponse() : ICrudItem
{
    public GetAllSupplierResponse(
        Guid id,
        Guid personId,
        string name,
        string? email,
        string? phoneNumber,
        string? document,
        AddressResponse? address)
        : this()
    {
        Id = id;
        PersonId = personId;
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        Document = document;
        Address = address;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    public Guid PersonId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(14)]
    public string? Document { get; set; }

    public AddressResponse? Address { get; set; }
}
