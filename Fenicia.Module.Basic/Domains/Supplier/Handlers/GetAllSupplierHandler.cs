using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Responses;
using Fenicia.Module.Basic.Domains.Supplier.Queries;
using Fenicia.Module.Basic.Domains.Supplier.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.Handlers;

/// <summary>
///     Handler responsible for retrieving all suppliers with pagination.
///     Returns a paginated list of suppliers including their contact information.
/// </summary>
public class GetAllSupplierHandler(DefaultContext db)
{
    /// <summary>
    ///     Retrieves paginated suppliers.
    /// </summary>
    /// <param name="query">The query containing page number and items per page.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated response containing suppliers.</returns>
    public async Task<Pagination<List<GetAllSupplierResponse>>> Handle(GetAllSupplierQuery query, CancellationToken ct)
    {
        var total = await db.BasicSuppliers.CountAsync(ct);

        var suppliers = await db.BasicSuppliers
            .Include(s => s.Person)
            .Include(s => s.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        var response = suppliers.Select(s =>
        {
            var personAddress = s.Person.PersonAddresses.FirstOrDefault();
            var address = personAddress?.Address;

            return new GetAllSupplierResponse(
                s.Id,
                s.PersonId,
                s.Person.Name,
                s.Person.Email,
                s.Person.PhoneNumber,
                s.Person.Document,
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
        }).ToList();

        return new Pagination<List<GetAllSupplierResponse>>(response, total, query.Page, query.PerPage);
    }
}