using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

public record AddEmployeeCommand(
    Guid Id,
    [Required] Guid PositionId,
    [Required] string Name,
    [EmailAddress] string? Email,
    string? Document,
    string? PhoneNumber,
    AddressDTO? Address);