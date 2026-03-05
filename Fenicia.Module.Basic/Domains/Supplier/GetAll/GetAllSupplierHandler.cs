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
            .Include(s => s.Person)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        var response = suppliers.Select(s => new GetAllSupplierResponse(
            s.Id,
            s.PersonId,
            s.Person.Name,
            s.Person.Email,
            s.Person.PhoneNumber,
            s.Person.Document,
            s.Person.Street,
            s.Person.Number,
            s.Person.Complement,
            s.Person.Neighborhood,
            s.Person.ZipCode,
            s.Person.StateId,
            s.Person.City)).ToList();

        return new Pagination<List<GetAllSupplierResponse>>(response, total, query.Page, query.PerPage);
    }
}
