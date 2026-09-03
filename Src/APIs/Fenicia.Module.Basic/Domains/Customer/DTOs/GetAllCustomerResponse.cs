using System.ComponentModel.DataAnnotations;
using Fenicia.Module.Basic.Domains.Address.DTOs;

namespace Fenicia.Module.Basic.Domains.Customer.DTOs;

public record GetAllCustomerResponse(
    [Required] Guid Id,
    [Required] Guid PersonId,
    [Required] [MaxLength(200)] string Name,
    string? Email,
    string? PhoneNumber,
    string? Document,
    AddressResponse? Address);