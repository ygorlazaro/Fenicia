using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
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

        var suppliers = await db.BasicSuppliers.Include(s => s.Person).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);

        var response = suppliers.Select(s => new GetAllSupplierResponse(s.Id, s.PersonId, s.Person.Name, s.Person.Email, s.Person.PhoneNumber, s.Person.Document, s.Person.Street, s.Person.Number, s.Person.Complement, s.Person.Neighborhood, s.Person.ZipCode, s.Person.StateId, s.Person.City)).ToList();

        return new Pagination<List<GetAllSupplierResponse>>(response, total, query.Page, query.PerPage);
    }
}