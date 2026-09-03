using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.StockMovement.DTOs;

public record GetStockMovementResponse(
    [Required] Guid Id,
    [Required] Guid ProductId,
    [Required] [MaxLength(200)] string ProductName,
    double Quantity,
    DateTime? Date,
    decimal? Price,
    [Required] StockMovementType Type,
    Guid? CustomerId,
    string? CustomerName,
    Guid? SupplierId,
    string? SupplierName,
    Guid? EmployeeId,
    string? EmployeeName,
    Guid? OrderId,
    string? Reason);