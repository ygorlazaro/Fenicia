using System.ComponentModel.DataAnnotations;
using Fenicia.Common;
using Fenicia.Common.DTOs.Auth.Address;

namespace Fenicia.Common.DTOs.Basic.Employee;

/// <summary>
/// Response DTO containing employee details.
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
public record EmployeeResponse(
    [Required] Guid Id,
    [Required] Guid PositionId,
    [Required] Guid PersonId,
    [Required][MaxLength(50)] string Name,
    [MaxLength(50)][EmailAddress] string? Email,
    [MaxLength(20)] string? PhoneNumber,
    [MaxLength(14)] string? Document,
    [MaxLength(200)] string? PositionName,
    AddressResponse? Address) : ICrudItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeResponse"/> class with default values.
    /// </summary>
    public EmployeeResponse()
        : this(Guid.Empty, Guid.Empty, Guid.Empty, string.Empty, null, null, null, null, null)
    {
    }
}