using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

public record EmployeeOrderCountResponse(
    [Required] Guid EmployeeId,
    [Required] [MaxLength(200)] string EmployeeName,
    [Required] [MaxLength(200)] string PositionName,
    int OrderCount,
    decimal TotalValue,
    [Required] DateTime FirstOrderDate,
    [Required] DateTime LastOrderDate);