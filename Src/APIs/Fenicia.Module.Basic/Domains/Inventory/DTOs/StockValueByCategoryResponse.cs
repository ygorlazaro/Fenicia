using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Inventory.DTOs;

public record StockValueByCategoryResponse(

    [Required] Guid CategoryId,

    [Required][MaxLength(200)] string CategoryName,

    int ProductCount,

    decimal TotalStockValue,

    double Percentage);
