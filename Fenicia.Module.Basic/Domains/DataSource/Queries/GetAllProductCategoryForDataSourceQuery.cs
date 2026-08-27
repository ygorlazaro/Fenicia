using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.DataSource.Queries;

public record GetAllProductCategoryForDataSourceQuery : IRequest<List<GetAllProductCategoryForDataSourceResponse>>;
