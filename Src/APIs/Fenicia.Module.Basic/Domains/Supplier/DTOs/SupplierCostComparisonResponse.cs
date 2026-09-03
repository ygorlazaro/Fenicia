using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record SupplierCostComparisonResponse(
    [Required] [MaxLength(200)] string ProductName,
    List<ProductSupplierPriceResponse> Suppliers);