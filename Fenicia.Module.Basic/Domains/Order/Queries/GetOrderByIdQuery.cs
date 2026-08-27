using Fenicia.Module.Basic.Domains.Order.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Order.Queries;

public record GetOrderByIdQuery(Guid Id) : IRequest<GetOrderByIdResponse?>;
