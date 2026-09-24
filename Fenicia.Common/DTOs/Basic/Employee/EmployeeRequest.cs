using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.Address;

namespace Fenicia.Common.DTOs.Basic.Employee;

/// <summary>
/// Request DTO for creating or updating an employee.
/// </summary>
public class EmployeeRequest()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeRequest"/> class with the specified employee details.
    /// </summary>
    /// <param name="id">The unique identifier of the employee.</param>
    /// <param name="positionId">The unique identifier of the position.</param>
    /// <param name="name">The employee name.</param>
    /// <param name="email">The employee email (optional).</param>
    /// <param name="document">The employee document (optional).</param>
    /// <param name="phoneNumber">The employee phone number (optional).</param>
    /// <param name="address">The employee address (optional).</param>
    public EmployeeRequest(
        Guid id,
        Guid positionId,
        string name,
        string? email,
        string? document,
        string? phoneNumber,
        AddressRequest? address)
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

    /// <summary>
    /// Gets or sets the unique identifier of the employee.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the position.
    /// </summary>
    [Required]
    public Guid PositionId { get; set; }

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
    /// Gets or sets the employee document.
    /// </summary>
    [MaxLength(14)]
    public string? Document { get; set; }

    /// <summary>
    /// Gets or sets the employee phone number.
    /// </summary>
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the employee address.
    /// </summary>
    public AddressRequest? Address { get; set; }
}