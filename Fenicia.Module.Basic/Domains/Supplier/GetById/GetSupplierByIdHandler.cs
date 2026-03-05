using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.GetById;

public class GetSupplierByIdHandler(DefaultContext context)
{
    public async Task<GetSupplierByIdResponse?> Handle(GetSupplierByIdQuery query, CancellationToken ct)
    {
        var supplier = await context.BasicSuppliers
            .Include(s => s.Person)
            .FirstOrDefaultAsync(s => s.Id == query.Id, ct);

        if (supplier is null)
            return null;

        return new GetSupplierByIdResponse(
            supplier.Id,
            supplier.PersonId,
            supplier.Person.Name,
            supplier.Person.Email,
            supplier.Person.PhoneNumber,
            supplier.Person.Document,
            supplier.Person.Street,
            supplier.Person.Number,
            supplier.Person.Complement,
            supplier.Person.Neighborhood,
            supplier.Person.ZipCode,
            supplier.Person.StateId,
            supplier.Person.City);
    }
}
