using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Product.Queries;
using Fenicia.Module.Basic.Domains.Product.Responses;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.Handlers;

public class GetAllProductHandler(DefaultContext db) : IRequestHandler<GetAllProductQuery, Pagination<List<GetAllProductResponse>>>
{
    public async Task<Pagination<List<GetAllProductResponse>>> Handle(GetAllProductQuery query, CancellationToken ct)
    {
        var request = from p in db.BasicProducts
            join c in db.BasicProductCategories on p.CategoryId equals c.Id
            join s in db.BasicSuppliers on p.SupplierId equals s.Id into ps
            from s in ps.DefaultIfEmpty()
            select new GetAllProductResponse(
                p.Id,
                p.Name,
                p.SKU,
                p.Barcode,
                p.Description,
                p.CostPrice,
                p.SalesPrice,
                p.Quantity,
                p.MinStockLevel,
                p.MaxStockLevel,
                p.ImageUrl,
                p.Weight,
                p.Dimensions,
                p.UnitOfMeasure,
                p.CategoryId,
                c.Name,
                p.SupplierId,
                s != null ? s.Person.Name : string.Empty,
                p.IsActive);

        var total = await request.CountAsync(ct);

        var products = await request.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);

        return new Pagination<List<GetAllProductResponse>>(products, total, query.Page, query.PerPage);
    }
}