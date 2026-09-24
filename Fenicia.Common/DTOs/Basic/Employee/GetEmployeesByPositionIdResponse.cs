using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.Address;

namespace Fenicia.Common.DTOs.Basic.Employee;

public class GetEmployeesByPositionIdResponse()
{
    public GetEmployeesByPositionIdResponse(
        Guid id,
        Guid positionId,
        Guid personId,
        string name,
        string? email,
        string? phoneNumber,
        string? document,
        string? positionName,
        AddressResponse? address)
        : this()
    {
        Id = id;
        PositionId = positionId;
        PersonId = personId;
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        Document = document;
        PositionName = positionName;
        Address = address;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid PositionId { get; set; }

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

    [MaxLength(200)]
    public string? PositionName { get; set; }

    public AddressResponse? Address { get; set; }
}
