using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Queries;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

/// <summary>
///     Handler responsible for retrieving all product categories for datasource purposes.
///     Returns product categories ordered alphabetically by name.
/// </summary>
public class GetAllProductCategoryForDataSourceHandler(DefaultContext db) : IRequestHandler<GetAllProductCategoryForDataSourceQuery, List<GetAllProductCategoryForDataSourceResponse>>
{
    /// <summary>
    ///     Retrieves all product categories ordered by name.
    /// </summary>
    /// <param name="query">Datasource query marker.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of product categories with ID and name.</returns>
    public async Task<List<GetAllProductCategoryForDataSourceResponse>> Handle(GetAllProductCategoryForDataSourceQuery query, CancellationToken ct)
    {
        return await db.BasicProductCategories.OrderBy(pc => pc.Name).Select(pc => new GetAllProductCategoryForDataSourceResponse(pc.Id, pc.Name)).ToListAsync(ct);
    }
}
