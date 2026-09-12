using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Supplier;

public record SupplierCostComparisonResponse(
    [Required] [MaxLength(200)] string ProductName,
    List<ProductSupplierPriceResponse> Suppliers);