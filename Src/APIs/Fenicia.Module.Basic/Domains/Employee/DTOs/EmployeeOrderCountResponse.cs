using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Employee.DTOs;

public record EmployeeOrderCountResponse(
    [Required] Guid EmployeeId,
    [Required] [MaxLength(200)] string EmployeeName,
    [Required] [MaxLength(200)] string PositionName,
    int OrderCount,
    decimal TotalValue,
    [Required] DateTime FirstOrderDate,
    [Required] DateTime LastOrderDate);