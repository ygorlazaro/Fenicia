using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Employee.Commands;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.Handlers;

/// <summary>
///     Handler responsible for deleting (soft delete) an employee.
///     Performs a soft delete by setting the Deleted timestamp.
/// </summary>
public class DeleteEmployeeHandler(DefaultContext db) : IRequestHandler<DeleteEmployeeCommand>
{
    /// <summary>
    ///     Soft deletes an employee by setting the Deleted timestamp to current time.
    /// </summary>
    /// <param name="command">The delete command containing the employee ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task Handle(DeleteEmployeeCommand command, CancellationToken ct)
    {
        var employee = await db.BasicEmployees.FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (employee is null)
        {
            return;
        }

        employee.Deleted = DateTime.Now;

        db.BasicEmployees.Update(employee);

        await db.SaveChangesAsync(ct);
    }
}
