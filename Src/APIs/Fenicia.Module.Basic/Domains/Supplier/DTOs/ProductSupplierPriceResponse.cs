using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record ProductSupplierPriceResponse(

    [Required] Guid SupplierId,

    [Required][MaxLength(200)] string SupplierName,

    decimal CostPrice,

    decimal SalesPrice,

    decimal ProfitMargin);
