using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Employee.DTOs;

public record EmployeeSalesResponse(
    [Required] Guid EmployeeId,
    [Required] [MaxLength(200)] string EmployeeName,
    [Required] [MaxLength(200)] string PositionName,
    decimal TotalSales,
    int TotalOrders,
    decimal AverageOrderValue,
    int Rank);