using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Inventory.DTOs;

public record ZeroMovementProductResponse(

    [Required] Guid ProductId,

    [Required][MaxLength(200)] string ProductName,

    [Required][MaxLength(200)] string CategoryName,

    string? SupplierName,

    double CurrentStock,

    decimal StockValue,

    DateTime? LastMovementDate,

    int DaysWithoutMovement);
