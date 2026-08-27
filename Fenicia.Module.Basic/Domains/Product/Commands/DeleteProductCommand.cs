using MediatR;

namespace Fenicia.Module.Basic.Domains.Product.Commands;

public record DeleteProductCommand(

    Guid Id) : IRequest;