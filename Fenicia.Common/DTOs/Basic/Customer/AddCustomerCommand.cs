using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Basic.Address;

namespace Fenicia.Common.DTOs.Basic.Customer;

public class AddCustomerCommand()
{
    public AddCustomerCommand(string name, string? email, string? document, string? phoneNumber, AddressCommand? address)
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

    public AddressCommand? Address { get; set; }
}
