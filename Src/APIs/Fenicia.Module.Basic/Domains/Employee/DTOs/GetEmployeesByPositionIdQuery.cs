using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Employee.DTOs;

public record GetEmployeesByPositionIdQuery([Required] Guid PositionId, int Page = 1, int PerPage = 10);