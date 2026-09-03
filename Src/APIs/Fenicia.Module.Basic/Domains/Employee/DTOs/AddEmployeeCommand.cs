using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Employee.DTOs;

public record AddEmployeeCommand(
    Guid Id,
    [Required] Guid PositionId,
    [Required] string Name,
    [EmailAddress] string? Email,
    string? Document,
    string? PhoneNumber,
    AddressDTO? Address);