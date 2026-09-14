using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Supplier;

public record DeleteSupplierCommand([Required] Guid Id);