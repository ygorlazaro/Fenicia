using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Basic.Address;

namespace Fenicia.Common.DTOs.Basic.Employee;

public record GetEmployeesByPositionIdResponse(
    [Required] Guid Id,
    [Required] Guid PositionId,
    [Required] Guid PersonId,
    [Required] [MaxLength(200)] string Name,
    string? Email,
    string? PhoneNumber,
    string? Document,
    string? PositionName,
    AddressResponse? Address);