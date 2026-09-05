using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.DataSource.DTOs;

public record GetAllDashboardProductForDataSourceResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
    string? SKU,
    string? Barcode,
    decimal? CostPrice,
    decimal SalesPrice,
    double Quantity,
    string? UnitOfMeasure,
    [Required] Guid CategoryId,
    [Required] [MaxLength(200)] string CategoryName,
    bool IsActive);
