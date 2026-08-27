using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Queries;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

public class GetAllProductCategoryForDataSourceHandler(DefaultContext db) : IRequestHandler<GetAllProductCategoryForDataSourceQuery, List<GetAllProductCategoryForDataSourceResponse>>
{

    public async Task<List<GetAllProductCategoryForDataSourceResponse>> Handle(GetAllProductCategoryForDataSourceQuery query, CancellationToken ct)
    {
        return await db.BasicProductCategories.OrderBy(pc => pc.Name).Select(pc => new GetAllProductCategoryForDataSourceResponse(pc.Id, pc.Name)).ToListAsync(ct);
    }
}
