using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Product.Queries;
using Fenicia.Module.Basic.Domains.Product.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.Handlers;

/// <summary>
///     Handler responsible for retrieving all products with pagination.
///     Returns a paginated list of products including their category and supplier information.
/// </summary>
public class GetAllProductHandler(DefaultContext db)
{
    /// <summary>
    ///     Retrieves paginated products.
    /// </summary>
    /// <param name="query">The query containing page number and items per page.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated response containing products with category and supplier details.</returns>
    public async Task<Pagination<List<GetAllProductResponse>>> Handle(GetAllProductQuery query, CancellationToken ct)
    {
        var request = from p in db.BasicProducts join c in db.BasicProductCategories on p.CategoryId equals c.Id join s in db.BasicSuppliers on p.SupplierId equals s.Id into ps from s in ps.DefaultIfEmpty() select new GetAllProductResponse(p.Id, p.Name, p.CostPrice, p.SalesPrice, p.Quantity, p.CategoryId, c.Name, p.SupplierId, s != null ? s.Person.Name : string.Empty);

        var total = await request.CountAsync(ct);

        var products = await request.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);

        return new Pagination<List<GetAllProductResponse>>(products, total, query.Page, query.PerPage);
    }
}