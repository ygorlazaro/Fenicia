using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Responses;
using Fenicia.Module.Basic.Domains.Supplier.Queries;
using Fenicia.Module.Basic.Domains.Supplier.Responses;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.Handlers;

/// <summary>
///     Handler responsible for retrieving a specific supplier by its ID.
/// </summary>
public class GetSupplierByIdHandler(DefaultContext db) : IRequestHandler<GetSupplierByIdQuery, GetSupplierByIdResponse?>
{
    /// <summary>
    ///     Retrieves a supplier by its ID.
    /// </summary>
    /// <param name="query">The query containing the supplier ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The supplier details if found, otherwise null.</returns>
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