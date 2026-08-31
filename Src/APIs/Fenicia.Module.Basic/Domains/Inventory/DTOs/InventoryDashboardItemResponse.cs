using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Inventory.DTOs;

public record InventoryDashboardItemResponse(

    [Required] Guid Id,

    [Required][MaxLength(200)] string Name,

    double Quantity,

    decimal? CostPrice,

    decimal SalesPrice,

    [Required] Guid CategoryId,

    [Required][MaxLength(200)] string CategoryName);
