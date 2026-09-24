using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.Address;

namespace Fenicia.Common.DTOs.Basic.Customer;

public class AddCustomerRequest()
{
    public AddCustomerRequest(string name, string? email, string? document, string? phoneNumber, AddressRequest? address)
        : this()
    {
        Name = name;
        Email = email;
        Document = document;
        PhoneNumber = phoneNumber;
        Address = address;
    }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(14)]
    public string? Document { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public AddressRequest? Address { get; set; }
}
