using MediatR;

namespace Fenicia.Module.Basic.Domains.Customer.Commands;

/// <summary>
///     Command record for deleting (soft delete) a customer.
/// </summary>
public record DeleteCustomerCommand(Guid Id) : IRequest;
