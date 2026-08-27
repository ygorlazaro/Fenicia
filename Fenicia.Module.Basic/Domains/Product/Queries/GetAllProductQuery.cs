using MediatR;
using Fenicia.Module.Basic.Domains.Product.Responses;
using Fenicia.Common;

namespace Fenicia.Module.Basic.Domains.Product.Queries;

public record GetAllProductQuery(

    int Page = 1,

    int PerPage = 10) : IRequest<Pagination<List<GetAllProductResponse>>>;