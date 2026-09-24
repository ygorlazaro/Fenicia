using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.Address;

namespace Fenicia.Common.DTOs.Basic.Customer;

public class UpdateCustomerRequest()
{
    public UpdateCustomerRequest(Guid id, string name, string? email, string? document, string? phoneNumber, AddressRequest? address)
        : this()
    {
        Id = id;
        Name = name;
        Email = email;
        Document = document;
        PhoneNumber = phoneNumber;
        Address = address;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(50)]
    public string? Email { get; set; }

    [MaxLength(14)]
    public string? Document { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public AddressRequest? Address { get; set; }
}
