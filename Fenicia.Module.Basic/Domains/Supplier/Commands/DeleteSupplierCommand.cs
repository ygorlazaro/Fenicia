using MediatR;

namespace Fenicia.Module.Basic.Domains.Supplier.Commands;

public record DeleteSupplierCommand(

    Guid Id) : IRequest;