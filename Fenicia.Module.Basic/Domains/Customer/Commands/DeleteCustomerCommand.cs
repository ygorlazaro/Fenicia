using MediatR;

namespace Fenicia.Module.Basic.Domains.Customer.Commands;

public record DeleteCustomerCommand(Guid Id) : IRequest;
