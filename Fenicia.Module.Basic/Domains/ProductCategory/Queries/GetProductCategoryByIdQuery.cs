using MediatR;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Queries;

public record GetProductCategoryByIdQuery(

    Guid Id) : IRequest<GetProductCategoryByIdResponse?>;