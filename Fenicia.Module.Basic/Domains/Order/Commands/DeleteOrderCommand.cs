using MediatR;

namespace Fenicia.Module.Basic.Domains.Order.Commands;

public record DeleteOrderCommand(Guid Id) : IRequest;
