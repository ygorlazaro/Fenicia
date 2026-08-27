using Fenicia.Module.Basic.Domains.Customer.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Customer.Queries;

public record GetCustomerByIdQuery(Guid Id) : IRequest<GetCustomerByIdResponse?>;
