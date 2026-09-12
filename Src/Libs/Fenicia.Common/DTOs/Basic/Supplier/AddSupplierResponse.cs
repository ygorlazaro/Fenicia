using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Supplier;

public record AddSupplierResponse(
    [Required] Guid Id,
    string? Cnpj);