using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record AddSupplierResponse(
    [Required] Guid Id,
    string? Cnpj);