using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.DTOs.Commands;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.Handlers;

public class DeleteCustomerHandler(DefaultContext db) : IRequestHandler<DeleteCustomerCommand>
{

    public async Task Handle(DeleteCustomerCommand command, CancellationToken ct)
    {
        var customer = await db.BasicCustomers.FirstOrDefaultAsync(c => c.Id == command.Id, ct);

        if (customer is null)
        {
            return;
        }

        customer.Deleted = DateTime.Now;

        db.BasicCustomers.Update(customer);

        await db.SaveChangesAsync(ct);
    }
}
