using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.DTOs.Responses;
using Fenicia.Module.Basic.Domains.Supplier.DTOs.Queries;
using Fenicia.Module.Basic.Domains.Supplier.DTOs.Responses;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.Handlers;

public class GetSupplierByIdHandler(DefaultContext db) : IRequestHandler<GetSupplierByIdQuery, GetSupplierByIdResponse?>
{

    public async Task<GetSupplierByIdResponse?> Handle(GetSupplierByIdQuery query, CancellationToken ct)
    {
        var supplier = await db.BasicSuppliers
            .Include(s => s.Person)
            .Include(s => s.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .FirstOrDefaultAsync(s => s.Id == query.Id, ct);

        if (supplier is null)
        {
            return null;
        }

        var personAddress = supplier.Person.PersonAddresses.FirstOrDefault();
        var address = personAddress?.Address;

        return new GetSupplierByIdResponse(
            supplier.Id,
            supplier.PersonId,
            supplier.Person.Name,
            supplier.Person.Email,
            supplier.Person.PhoneNumber,
            supplier.Person.Document,
            address != null ? new AddressResponse(
                address.Id,
                address.Street,
                address.Number,
                address.Complement,
                address.Neighborhood,
                address.ZipCode,
                address.StateId,
                address.State?.Name,
                address.City,
                address.Country
            ) : null
        );
    }
}