using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

public record UpdateEmployeeResponse([Required] Guid Id, [Required] Guid PositionId, [Required] Guid PersonId);