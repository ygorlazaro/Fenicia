using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Supplier;

public record UpdateSupplierResponse(
    [Required] Guid Id,
    string? Cnpj);