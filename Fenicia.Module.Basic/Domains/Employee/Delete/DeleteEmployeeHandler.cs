using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.Delete;

public class DeleteEmployeeHandler(BasicContext context)
{
    public async Task Handle(DeleteEmployeeCommand command, CancellationToken ct)
    {
        var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (employee is null) return;

        employee.Deleted = DateTime.Now;

        context.Employees.Update(employee);

        await context.SaveChangesAsync(ct);
    }
}
