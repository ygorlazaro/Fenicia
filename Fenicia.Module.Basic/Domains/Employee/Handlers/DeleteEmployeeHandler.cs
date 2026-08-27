using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Employee.DTOs.Commands;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.Handlers;

public class DeleteEmployeeHandler(DefaultContext db) : IRequestHandler<DeleteEmployeeCommand>
{

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
