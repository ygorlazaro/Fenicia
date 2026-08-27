using MediatR;
using Fenicia.Module.Basic.Domains.Product.Responses;

namespace Fenicia.Module.Basic.Domains.Product.Queries;

public record GetProductByIdQuery(

    Guid Id) : IRequest<GetProductByIdResponse?>;