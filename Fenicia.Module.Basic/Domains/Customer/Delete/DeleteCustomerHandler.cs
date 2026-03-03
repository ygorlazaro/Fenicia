using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.Delete;

public class DeleteCustomerHandler(DefaultContext context)
{
    public async Task Handle(DeleteCustomerCommand command, CancellationToken ct)
    {
        var customer = await context.BasicCustomers.FirstOrDefaultAsync(c => c.Id == command.Id, ct);

        if (customer is null)
        {
            return;
        }

        customer.Deleted = DateTime.Now;

        context.BasicCustomers.Update(customer);

        await context.SaveChangesAsync(ct);
    }
}
