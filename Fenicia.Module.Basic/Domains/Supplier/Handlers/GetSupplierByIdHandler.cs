using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Supplier.Queries;
using Fenicia.Module.Basic.Domains.Supplier.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.Handlers;

/// <summary>
/// Handler responsible for retrieving a specific supplier by its ID.
/// </summary>
public class GetSupplierByIdHandler(DefaultContext db)
{
    /// <summary>
    /// Retrieves a supplier by its ID.
    /// </summary>
    /// <param name="query">The query containing the supplier ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The supplier details if found, otherwise null.</returns>
    public async Task<GetSupplierByIdResponse?> Handle(GetSupplierByIdQuery query, CancellationToken ct)
    {
        var supplier = await db.BasicSuppliers
            .Include(s => s.Person)
            .FirstOrDefaultAsync(s => s.Id == query.Id,
                ct);

        return supplier switch
        {
            null => null,
            _ => new GetSupplierByIdResponse(supplier.Id,
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
                supplier.Person.City)
        };

    }
}
