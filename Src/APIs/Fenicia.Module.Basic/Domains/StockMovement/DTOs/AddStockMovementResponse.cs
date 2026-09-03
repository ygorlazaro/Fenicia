using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.StockMovement.DTOs;

public record AddStockMovementResponse(
    [Required] Guid Id,
    [Required] Guid ProductId,
    double Quantity,
    DateTime? Date,
    decimal? Price,
    [Required] StockMovementType Type,
    Guid? CustomerId,
    Guid? SupplierId,
    Guid? EmployeeId,
    Guid? OrderId,
    string? Reason);