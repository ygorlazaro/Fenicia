using Fenicia.Module.Basic.Domains.Address.DTOs;

namespace Fenicia.Module.Basic.Domains.Customer.DTOs;

public record AddCustomerCommand(
    string Name,
    string? Email,
    string? Document,
    string? PhoneNumber,
    AddressCommand? Address);
