using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Product.DTOs;

public record NeverSoldProductResponse(

    [Required] Guid ProductId,

    [Required][MaxLength(200)] string ProductName,

    [Required][MaxLength(200)] string CategoryName,

    string? SupplierName,

    double CurrentStock,

    decimal CostValue,

    DateTime? LastStockMovement);
