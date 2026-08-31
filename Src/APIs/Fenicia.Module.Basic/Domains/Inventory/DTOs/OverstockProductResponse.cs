using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Inventory.DTOs;

public record OverstockProductResponse(

    [Required] Guid ProductId,

    [Required][MaxLength(200)] string ProductName,

    [Required][MaxLength(200)] string CategoryName,

    double CurrentQuantity,

    double RecommendedQuantity,

    decimal ExcessValue,

    decimal CostPrice);
