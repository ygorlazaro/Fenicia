using System.ComponentModel.DataAnnotations;
using Fenicia.Module.Basic.Domains.Address.DTOs;

namespace Fenicia.Module.Basic.Domains.Employee.DTOs;

public record GetEmployeeByIdResponse([Required] Guid Id, [Required] Guid PositionId, [Required] Guid PersonId, [Required][MaxLength(200)] string Name, string? Email, string? PhoneNumber, string? Document, AddressResponse? Address);
