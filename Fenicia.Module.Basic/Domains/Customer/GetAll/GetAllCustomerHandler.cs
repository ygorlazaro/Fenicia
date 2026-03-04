using Fenicia.Common;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.GetAll;

public class GetAllCustomerHandler(DefaultContext context)
{
    public async Task<Pagination<List<GetAllCustomerResponse>>> Handle(GetAllCustomerQuery query, CancellationToken ct)
    {
        var total = await context.BasicCustomers.CountAsync(ct);

        var customers = await context.BasicCustomers
            .Include(c => c.PersonModel)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        var response = customers.Select(c => new GetAllCustomerResponse(
            c.Id,
            c.PersonId,
            c.PersonModel.Name,
            c.PersonModel.Email,
            c.PersonModel.PhoneNumber,
            c.PersonModel.Document,
            c.PersonModel.Street,
            c.PersonModel.Number,
            c.PersonModel.Complement,
            c.PersonModel.Neighborhood,
            c.PersonModel.ZipCode,
            c.PersonModel.StateId,
            c.PersonModel.City)).ToList();

        return new Pagination<List<GetAllCustomerResponse>>(response, total, query.Page, query.PerPage);
    }
}
