using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.StockMovement.DTOs;

public record UpdateStockMovementCommand(
    [Required] Guid Id,
    double Quantity,
    DateTime? Date,
    decimal Price,
    [Required] StockMovementType Type,
    [Required] Guid ProductId,
    Guid? CustomerId,
    Guid? SupplierId,
    Guid? EmployeeId,
    Guid? OrderId,
    string? Reason);