using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Employee;

public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
{
    public async Task<List<EmployeeResponse>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 1)
    {
        var employees = await employeeRepository.GetAllAsync(ct, page, perPage);

        return [.. employees.Select(e => new EmployeeResponse(e))];
    }

    public async Task<EmployeeResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var employee = await employeeRepository.GetByIdAsync(id, ct);

        return employee is null ? null : new EmployeeResponse(employee);
    }

    public async Task<EmployeeResponse?> AddAsync(EmployeeRequest request, CancellationToken ct)
    {
        var employee = new EmployeeModel(request);

        employeeRepository.Add(employee);

        await employeeRepository.SaveChangesAsync(ct);

        return new EmployeeResponse(employee);
    }

    public async Task<EmployeeResponse?> UpdateAsync(EmployeeRequest request, CancellationToken ct)
    {
        var employee = new EmployeeModel(request);

        employeeRepository.Update(employee);

        await employeeRepository.SaveChangesAsync(ct);

        return new EmployeeResponse(employee);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await employeeRepository.DeleteAsync(id, ct);

        await employeeRepository.SaveChangesAsync(ct);
    }

    public async Task<List<EmployeeResponse>> GetByPositionIdAsync(
        Guid positionId,
        CancellationToken ct,
        int page = 1,
        int perPage = 10)
    {
        var employees = await employeeRepository.GetByPositionIdAsync(positionId, ct, page, perPage);

        return [.. employees.Select(e => new EmployeeResponse(e))];
    }
}