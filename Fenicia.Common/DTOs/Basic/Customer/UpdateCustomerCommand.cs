using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Basic.Address;

namespace Fenicia.Common.DTOs.Basic.Customer;

public class UpdateCustomerCommand()
{
    public UpdateCustomerCommand(Guid id, string name, string? email, string? document, string? phoneNumber, AddressCommand? address)
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

    public AddressCommand? Address { get; set; }
}
