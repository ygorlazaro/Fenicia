using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record SupplierProductCountResponse(

    [Required] Guid SupplierId,

    [Required][MaxLength(200)] string SupplierName,

    int ProductCount,

    decimal TotalStockValue,

    decimal TotalRevenue);
