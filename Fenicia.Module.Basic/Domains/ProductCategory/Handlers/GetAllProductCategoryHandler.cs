using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs.Queries;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs.Responses;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Handlers;

public class GetAllProductCategoryHandler(DefaultContext db) : IRequestHandler<GetAllProductCategoryQuery, Pagination<List<GetAllProductCategoryResponse>>>
{

    public async Task<Pagination<List<GetAllProductCategoryResponse>>> Handle(GetAllProductCategoryQuery query, CancellationToken ct)
    {
        var total = await db.BasicProductCategories.CountAsync(ct);

        var categories = await db.BasicProductCategories.Select(pc => new GetAllProductCategoryResponse(pc.Id, pc.Name)).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);

        return new Pagination<List<GetAllProductCategoryResponse>>(categories, total, query.Page, query.PerPage);
    }
}