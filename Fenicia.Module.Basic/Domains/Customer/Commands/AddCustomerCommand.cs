using Fenicia.Module.Basic.Domains.Customer.Common;
using Fenicia.Module.Basic.Domains.Customer.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Customer.Commands;

/// <summary>
///     Command record for creating a new customer.
///     Contains all necessary information to create a customer and their associated person record.
/// </summary>
public record AddCustomerCommand(
    string Name,
    string? Email,
    string? Document,
    string? PhoneNumber,
    AddressCommand? Address) : IRequest<AddCustomerResponse>;
