using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Employee.DTOs;

public record TopPerformerResponse(
    [Required] Guid EmployeeId,
    [Required] [MaxLength(200)] string EmployeeName,
    [Required] [MaxLength(200)] string PositionName,
    decimal TotalSales,
    int TotalOrders,
    [Required] [MaxLength(200)] string PerformanceLevel);