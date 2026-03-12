using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Product.Queries;
using Fenicia.Module.Basic.Domains.Product.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.Handlers;

public class GetAllProductHandler(DefaultContext db)
{
    public async Task<Pagination<List<GetAllProductResponse>>> Handle(GetAllProductQuery query, CancellationToken ct)
    {
        var request = from p in db.BasicProducts
                      join c in db.BasicProductCategories on p.CategoryId equals c.Id
                      join s in db.BasicSuppliers on p.SupplierId equals s.Id into ps
                      from s in ps.DefaultIfEmpty()
                      select new GetAllProductResponse
                      (
                          p.Id,
                          p.Name,
                          p.CostPrice,
                          p.SalesPrice,
                          p.Quantity,
                          p.CategoryId,
                          c.Name,
                          p.SupplierId,
                          s != null ? s.Person.Name : string.Empty
                      );

        var total = await request.CountAsync(ct);

        var products = await request
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        return new Pagination<List<GetAllProductResponse>>(products,
            total,
            query.Page,
            query.PerPage);
    }
}
