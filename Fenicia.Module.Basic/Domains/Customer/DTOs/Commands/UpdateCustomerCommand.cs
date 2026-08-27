using Fenicia.Module.Basic.Domains.Customer.Common;
using Fenicia.Module.Basic.Domains.Customer.DTOs.Responses;


namespace Fenicia.Module.Basic.Domains.Customer.DTOs.Commands;

public record UpdateCustomerCommand(
    Guid Id,
    string Name,
    string? Email,
    string? Document,
    string? PhoneNumber,
    AddressCommand? Address);
