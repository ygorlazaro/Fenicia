using Fenicia.Common;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.GetAll;

public class GetAllSupplierHandler(DefaultContext context)
{
    public async Task<Pagination<List<GetAllSupplierResponse>>> Handle(GetAllSupplierQuery query, CancellationToken ct)
    {
        var total = await context.BasicSuppliers.CountAsync(ct);

        var suppliers = await context.BasicSuppliers
            .Include(s => s.PersonModel)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        var response = suppliers.Select(s => new GetAllSupplierResponse(
            s.Id,
            s.PersonId,
            s.PersonModel.Name,
            s.PersonModel.Email,
            s.PersonModel.PhoneNumber,
            s.PersonModel.Document,
            s.PersonModel.Street,
            s.PersonModel.Number,
            s.PersonModel.Complement,
            s.PersonModel.Neighborhood,
            s.PersonModel.ZipCode,
            s.PersonModel.StateId,
            s.PersonModel.City)).ToList();

        return new Pagination<List<GetAllSupplierResponse>>(response, total, query.Page, query.PerPage);
    }
}
