using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record UpdateSupplierResponse(
    [Required] Guid Id,
    string? Cnpj);