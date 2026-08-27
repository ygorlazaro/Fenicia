using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.DTOs.Queries;
using Fenicia.Module.Basic.Domains.DataSource.DTOs.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

public class GetAllProductForDataSourceHandler(DefaultContext db) : IRequestHandler<GetAllProductForDataSourceQuery, List<GetAllProductForDataSourceResponse>>
{

    public async Task<List<GetAllProductForDataSourceResponse>> Handle(GetAllProductForDataSourceQuery query, CancellationToken ct)
    {
        return await db.BasicProducts.OrderBy(p => p.Name).Select(p => new GetAllProductForDataSourceResponse(p.Id, p.Name)).ToListAsync(ct);
    }
}
