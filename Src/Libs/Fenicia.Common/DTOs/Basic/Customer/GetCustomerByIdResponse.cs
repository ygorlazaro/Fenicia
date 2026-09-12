using System.ComponentModel.DataAnnotations;
using Fenicia.Common;
using Fenicia.Common.DTOs.Basic.Address;

namespace Fenicia.Common.DTOs.Basic.Customer;

public record GetCustomerByIdResponse([Required] Guid Id,
    [Required] Guid PersonId,
    [Required] [MaxLength(200)] string Name,
    string? Email,
    string? PhoneNumber,
    string? Document,
    AddressResponse? Address) : ICrudItem;