using MediatR;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;
using Fenicia.Common;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Queries;

public record GetAllProductCategoryQuery(

    int Page = 1,

    int PerPage = 10) : IRequest<Pagination<List<GetAllProductCategoryResponse>>>;