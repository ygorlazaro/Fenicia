using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Commands;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.Handlers;

/// <summary>
/// Handler responsible for deleting (soft delete) a customer.
/// Performs a soft delete by setting the Deleted timestamp.
/// </summary>
public class DeleteCustomerHandler(DefaultContext db)
{
    /// <summary>
    /// Soft deletes a customer by setting the Deleted timestamp to current time.
    /// </summary>
    /// <param name="command">The delete command containing the customer ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task Handle(DeleteCustomerCommand command, CancellationToken ct)
    {
        var customer = await db.BasicCustomers.FirstOrDefaultAsync(c => c.Id == command.Id,
            ct);

        if (customer is null)
        {
            return;
        }

        customer.Deleted = DateTime.Now;

        db.BasicCustomers.Update(customer);

        await db.SaveChangesAsync(ct);
    }
}
