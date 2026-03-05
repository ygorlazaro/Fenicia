using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.StockMovement.GetMovement;

public record GetStockMovementResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    double Quantity,
    DateTime? Date,
    decimal? Price,
    StockMovementType Type,
    Guid? CustomerId,
    string? CustomerName,
    Guid? SupplierId,
    string? SupplierName,
    Guid? EmployeeId,
    string? EmployeeName,
    Guid? OrderId,
    string? Reason);
