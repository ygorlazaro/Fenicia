using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.GetAll;

public class GetAllSupplierHandler(BasicContext context)
{
    public async Task<List<SupplierResponse>> Handle(GetAllSupplierQuery query, CancellationToken ct)
    {
        var suppliers = await context.Suppliers
            .Include(s => s.Person)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        return suppliers.Select(s => new SupplierResponse(
            s.Id,
            s.Cnpj,
            new PersonResponse(
                s.Person.Name,
                s.Person.Email,
                s.Person.Document,
                s.Person.PhoneNumber,
                new AddressResponse(
                    s.Person.City,
                    s.Person.Complement,
                    s.Person.Neighborhood,
                    s.Person.Number,
                    s.Person.StateId,
                    s.Person.Street,
                    s.Person.ZipCode)))).ToList();
    }
}
