using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Employee.DTOs;

public record UpdateEmployeeCommand(
    [Required] Guid Id,
    [Required] Guid PositionId,
    [Required][MaxLength(200)] string Name,
    string? Email,
    string? Document,
    string? PhoneNumber,
    AddressDTO? Address);
