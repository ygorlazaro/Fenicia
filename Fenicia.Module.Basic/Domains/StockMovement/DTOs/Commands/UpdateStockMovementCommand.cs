using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs.Responses;

namespace Fenicia.Module.Basic.Domains.StockMovement.DTOs.Commands;

public record UpdateStockMovementCommand(

    Guid Id,

    double Quantity,

    DateTime? Date,

    decimal Price,

    StockMovementType Type,

    Guid ProductId,

    Guid? CustomerId,

    Guid? SupplierId,

    Guid? EmployeeId,

    Guid? OrderId,

    string? Reason);