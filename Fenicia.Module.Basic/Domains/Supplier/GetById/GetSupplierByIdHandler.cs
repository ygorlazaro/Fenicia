using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.GetById;

public class GetSupplierByIdHandler(BasicContext context)
{
    public async Task<SupplierResponse?> Handle(GetSupplierByIdQuery query, CancellationToken ct)
    {
        var supplier = await context.Suppliers
            .Include(s => s.Person)
            .FirstOrDefaultAsync(s => s.Id == query.Id, ct);

        if (supplier is null) return null;

        return new SupplierResponse(
            supplier.Id,
            supplier.Cnpj,
            new PersonResponse(
                supplier.Person.Name,
                supplier.Person.Email,
                supplier.Person.Cpf,
                supplier.Person.PhoneNumber,
                new AddressResponse(
                    supplier.Person.City,
                    supplier.Person.Complement,
                    supplier.Person.Neighborhood,
                    supplier.Person.Number,
                    supplier.Person.StateId,
                    supplier.Person.Street,
                    supplier.Person.ZipCode)));
    }
}
