using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Supplier;

public record ProductSupplierPriceResponse(
    [Required] Guid SupplierId,
    [Required] [MaxLength(200)] string SupplierName,
    decimal CostPrice,
    decimal SalesPrice,
    decimal ProfitMargin);