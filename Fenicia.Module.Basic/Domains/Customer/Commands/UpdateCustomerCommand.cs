using Fenicia.Module.Basic.Domains.Customer.Common;
using Fenicia.Module.Basic.Domains.Customer.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Customer.Commands;

public record UpdateCustomerCommand(
    Guid Id,
    string Name,
    string? Email,
    string? Document,
    string? PhoneNumber,
    AddressCommand? Address) : IRequest<UpdateCustomerResponse?>;
