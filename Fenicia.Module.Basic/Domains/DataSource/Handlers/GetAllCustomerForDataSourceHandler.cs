using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.DTOs.Queries;
using Fenicia.Module.Basic.Domains.DataSource.DTOs.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

public class GetAllCustomerForDataSourceHandler(DefaultContext db) : IRequestHandler<GetAllCustomerForDataSourceQuery, List<GetAllCustomerForDataSourceResponse>>
{

    public async Task<List<GetAllCustomerForDataSourceResponse>> Handle(GetAllCustomerForDataSourceQuery query, CancellationToken ct)
    {
        return await db.BasicCustomers.AsNoTracking().OrderBy(c => c.Person.Name).Select(c => new GetAllCustomerForDataSourceResponse(c.Id, c.Person.Name)).ToListAsync(ct);
    }
}
