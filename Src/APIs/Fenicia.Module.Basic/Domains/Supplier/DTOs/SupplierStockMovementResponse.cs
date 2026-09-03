using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record SupplierStockMovementResponse(
    [Required] Guid MovementId,
    [Required] Guid ProductId,
    [Required] [MaxLength(200)] string ProductName,
    double Quantity,
    decimal Price,
    [Required] DateTime Date,
    [Required] [MaxLength(200)] string MovementType);