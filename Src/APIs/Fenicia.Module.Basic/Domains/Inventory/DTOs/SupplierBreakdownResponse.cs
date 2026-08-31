using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Inventory.DTOs;

public record SupplierBreakdownResponse(

    [Required] Guid SupplierId,

    [Required][MaxLength(200)] string SupplierName,

    decimal TotalCostValue,

    decimal TotalSalesValue,

    double TotalQuantity);
