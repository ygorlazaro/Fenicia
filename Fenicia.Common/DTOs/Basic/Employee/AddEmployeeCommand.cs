using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

public class AddEmployeeCommand()
{
    public AddEmployeeCommand(
        Guid id,
        Guid positionId,
        string name,
        string? email,
        string? document,
        string? phoneNumber,
        AddressDTO? address)
        : this()
    {
        Id = id;
        PositionId = positionId;
        Name = name;
        Email = email;
        Document = document;
        PhoneNumber = phoneNumber;
        Address = address;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid PositionId { get; set; }

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

    public AddressDTO? Address { get; set; }
}
