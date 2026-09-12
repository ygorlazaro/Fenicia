using System.ComponentModel.DataAnnotations;
using Fenicia.Common;
using Fenicia.Common.DTOs.Basic.Address;

namespace Fenicia.Common.DTOs.Basic.Supplier;

public record GetAllSupplierResponse([Required] Guid Id,
    [Required] Guid PersonId,
    [Required] [MaxLength(200)] string Name,
    [MaxLength(200)] string? Email,
    [MaxLength(200)] string? PhoneNumber,
    [MaxLength(200)] string? Document,
    AddressResponse? Address) : ICrudItem;
