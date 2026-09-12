using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

public record UpdateEmployeeCommand(
    [Required] Guid Id,
    [Required] Guid PositionId,
    [Required] [MaxLength(200)] string Name,
    string? Email,
    string? Document,
    string? PhoneNumber,
    AddressDTO? Address);