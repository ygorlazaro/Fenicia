using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record UpdateSupplierCommand(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
    [MaxLength(200)] string? Email,
    [MaxLength(200)] string? Document,
    [MaxLength(200)] string? PhoneNumber,
    [MaxLength(200)] string? Cnpj,
    AddressDTO? Address);