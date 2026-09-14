using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

public record GetEmployeesByPositionIdQuery([Required] Guid PositionId, int Page = 1, int PerPage = 10);