using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Queries;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

public class GetAllSupplierForDataSourceHandler(DefaultContext db) : IRequestHandler<GetAllSupplierForDataSourceQuery, List<GetAllSupplierForDataSourceResponse>>
{

    public async Task<List<GetAllSupplierForDataSourceResponse>> Handle(GetAllSupplierForDataSourceQuery query, CancellationToken ct)
    {
        return await db.BasicSuppliers.OrderBy(s => s.Person.Name).Select(s => new GetAllSupplierForDataSourceResponse(s.Id, s.Person.Name)).ToListAsync(ct);
    }
}
