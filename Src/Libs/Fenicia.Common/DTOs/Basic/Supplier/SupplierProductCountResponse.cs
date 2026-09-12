using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Supplier;

public record SupplierProductCountResponse(
    [Required] Guid SupplierId,
    [Required] [MaxLength(200)] string SupplierName,
    int ProductCount,
    decimal TotalStockValue,
    decimal TotalRevenue);