using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Basic.Address;

namespace Fenicia.Common.DTOs.Basic.Customer;

public record AddCustomerCommand(
    [Required] [MaxLength(200)] string Name,
    string? Email,
    string? Document,
    string? PhoneNumber,
    AddressCommand? Address);