using System.ComponentModel.DataAnnotations;
using Fenicia.Module.Basic.Domains.Address.DTOs;

namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record GetAllSupplierResponse(
    [Required] Guid Id,
    [Required] Guid PersonId,
    [Required] [MaxLength(200)] string Name,
    [MaxLength(200)] string? Email,
    [MaxLength(200)] string? PhoneNumber,
    [MaxLength(200)] string? Document,
    AddressResponse? Address);