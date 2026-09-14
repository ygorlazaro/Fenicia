using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.DTOs.Basic.StockMovement;

public record AddStockMovementCommand(
    [Required] Guid Id,
    double Quantity,
    DateTime? Date,
    decimal? Price,
    [Required] StockMovementType Type,
    [Required] Guid ProductId,
    Guid? CustomerId,
    Guid? SupplierId,
    Guid? EmployeeId,
    Guid? OrderId,
    string? Reason);