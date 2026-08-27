using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Customer.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Customer.Queries;

public record GetAllCustomerQuery(int Page = 1, int PerPage = 10) : IRequest<Pagination<List<GetAllCustomerResponse>>>;
