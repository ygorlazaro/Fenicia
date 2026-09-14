using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Basic.Address;

namespace Fenicia.Common.DTOs.Basic.Customer;

public record UpdateCustomerCommand(
    Guid Id,
    [Required] string Name,
    [EmailAddress] string? Email,
    string? Document,
    string? PhoneNumber,
    AddressCommand? Address);