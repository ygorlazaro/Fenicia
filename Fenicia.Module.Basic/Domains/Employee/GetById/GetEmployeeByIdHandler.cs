using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.GetById;

public class GetEmployeeByIdHandler(DefaultContext context)
{
    public async Task<GetEmployeeByIdResponse?> Handle(GetEmployeeByIdQuery query, CancellationToken ct)
    {
        var employee = await context.BasicEmployees
            .Include(e => e.PersonModel)
            .FirstOrDefaultAsync(e => e.Id == query.Id, ct);

        if (employee is null)
            return null;

        return new GetEmployeeByIdResponse(
            employee.Id,
            employee.PositionId,
            employee.PersonId,
            employee.PersonModel.Name,
            employee.PersonModel.Email,
            employee.PersonModel.PhoneNumber,
            employee.PersonModel.Document,
            employee.PersonModel.Street,
            employee.PersonModel.Number,
            employee.PersonModel.Complement,
            employee.PersonModel.Neighborhood,
            employee.PersonModel.ZipCode,
            employee.PersonModel.StateId,
            employee.PersonModel.City);
    }
}
