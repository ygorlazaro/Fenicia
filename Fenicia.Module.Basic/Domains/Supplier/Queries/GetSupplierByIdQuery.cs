using MediatR;
using Fenicia.Module.Basic.Domains.Supplier.Responses;

namespace Fenicia.Module.Basic.Domains.Supplier.Queries;

public record GetSupplierByIdQuery(

    Guid Id) : IRequest<GetSupplierByIdResponse?>;