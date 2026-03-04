using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.GetById;

public class GetCustomerByIdHandler(DefaultContext context)
{
    public async Task<GetCustomerByIdResponse?> Handle(GetCustomerByIdQuery query, CancellationToken ct)
    {
        var customer = await context.BasicCustomers
            .Include(c => c.PersonModel)
            .FirstOrDefaultAsync(c => c.Id == query.Id, ct);

        if (customer is null)
            return null;

        return new GetCustomerByIdResponse(
            customer.Id,
            customer.PersonId,
            customer.PersonModel.Name,
            customer.PersonModel.Email,
            customer.PersonModel.PhoneNumber,
            customer.PersonModel.Document,
            customer.PersonModel.Street,
            customer.PersonModel.Number,
            customer.PersonModel.Complement,
            customer.PersonModel.Neighborhood,
            customer.PersonModel.ZipCode,
            customer.PersonModel.StateId,
            customer.PersonModel.City);
    }
}
