using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Employee.DTOs;

public record AddEmployeeResponse([Required] Guid Id, [Required] Guid PositionId, [Required] Guid PersonId);