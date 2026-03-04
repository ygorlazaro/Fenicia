using Fenicia.Common;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.GetAll;

public class GetAllEmployeeHandler(DefaultContext context)
{
    public async Task<Pagination<List<GetAllEmployeeResponse>>> Handle(GetAllEmployeeQuery query, CancellationToken ct)
    {
        var total = await context.BasicEmployees.CountAsync(ct);

        var employees = await context.BasicEmployees
            .Include(e => e.PersonModel)
            .ThenInclude(p => p.StateModel)
            .Include(e => e.PositionModel)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        var response = employees.Select(e => new GetAllEmployeeResponse(
            e.Id,
            e.PositionId,
            e.PersonId,
            e.PersonModel.Name,
            e.PersonModel.Email,
            e.PersonModel.PhoneNumber,
            e.PersonModel.Document,
            e.PersonModel.Street,
            e.PersonModel.Number,
            e.PersonModel.Complement,
            e.PersonModel.Neighborhood,
            e.PersonModel.ZipCode,
            e.PersonModel.StateId,
            e.PersonModel.City,
            e.PositionModel.Name,
            e.PersonModel.StateModel != null ? e.PersonModel.StateModel.Name : null)).ToList();

        return new Pagination<List<GetAllEmployeeResponse>>(response, total, query.Page, query.PerPage);
    }
}
