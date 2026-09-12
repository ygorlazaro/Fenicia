using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

public record AddEmployeeResponse([Required] Guid Id, [Required] Guid PositionId, [Required] Guid PersonId);