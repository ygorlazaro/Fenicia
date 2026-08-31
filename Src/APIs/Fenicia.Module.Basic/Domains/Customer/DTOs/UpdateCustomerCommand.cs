using System.ComponentModel.DataAnnotations;
using Fenicia.Module.Basic.Domains.Address.DTOs;

namespace Fenicia.Module.Basic.Domains.Customer.DTOs;

public record UpdateCustomerCommand(
    Guid Id,
    [Required] string Name,
    [EmailAddress] string? Email,
    string? Document,
    string? PhoneNumber,
    AddressCommand? Address);
