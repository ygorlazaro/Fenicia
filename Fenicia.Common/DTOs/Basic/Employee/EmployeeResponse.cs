using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.Address;

namespace Fenicia.Common.DTOs.Basic.Employee;

/// <summary>
/// Response DTO containing employee details.
/// </summary>
public class EmployeeResponse() : ICrudItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeResponse"/> class with the specified employee details.
    /// </summary>
    /// <param name="id">The unique identifier of the employee.</param>
    /// <param name="positionId">The unique identifier of the position.</param>
    /// <param name="personId">The unique identifier of the associated person.</param>
    /// <param name="name">The employee name.</param>
    /// <param name="email">The employee email (optional).</param>
    /// <param name="phoneNumber">The employee phone number (optional).</param>
    /// <param name="document">The employee document (optional).</param>
    /// <param name="positionName">The position name (optional).</param>
    /// <param name="address">The employee address (optional).</param>
    public EmployeeResponse(
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

    /// <summary>
    /// Gets or sets the unique identifier of the employee.
    /// </summary>
    [Required]
    public Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the unique identifier of the position.
    /// </summary>
    [Required]
    public Guid PositionId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the associated person.
    /// </summary>
    [Required]
    public Guid PersonId { get; set; }

    /// <summary>
    /// Gets or sets the employee name.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the employee email.
    /// </summary>
    [MaxLength(50)]
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the employee phone number.
    /// </summary>
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the employee document.
    /// </summary>
    [MaxLength(14)]
    public string? Document { get; set; }

    /// <summary>
    /// Gets or sets the position name.
    /// </summary>
    [MaxLength(200)]
    public string? PositionName { get; set; }

    /// <summary>
    /// Gets or sets the employee address.
    /// </summary>
    public AddressResponse? Address { get; set; }
}