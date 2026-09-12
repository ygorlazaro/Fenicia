using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

public record TopPerformerResponse(
    [Required] Guid EmployeeId,
    [Required] [MaxLength(200)] string EmployeeName,
    [Required] [MaxLength(200)] string PositionName,
    decimal TotalSales,
    int TotalOrders,
    [Required] [MaxLength(200)] string PerformanceLevel);