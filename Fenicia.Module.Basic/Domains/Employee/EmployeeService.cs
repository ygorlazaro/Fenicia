using Fenicia.Common.Database.Converters.Basic;
using Fenicia.Common.Database.Requests.Basic;
using Fenicia.Common.Database.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Employee;

public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
{
    public async Task<List<EmployeeResponse>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int perPage = 1)
    {
        var employees = await employeeRepository.GetAllAsync(cancellationToken, page, perPage);

        return EmployeeConverter.Convert(employees);
    }

    public async Task<EmployeeResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(id, cancellationToken);

        return employee is null ? null : EmployeeConverter.Convert(employee);
    }

    public async Task<EmployeeResponse?> AddAsync(EmployeeRequest request, CancellationToken cancellationToken)
    {
        var employee = EmployeeConverter.Convert(request);

        employeeRepository.Add(employee);

        await employeeRepository.SaveChangesAsync(cancellationToken);

        return EmployeeConverter.Convert(employee);
    }

    public async Task<EmployeeResponse?> UpdateAsync(EmployeeRequest request, CancellationToken cancellationToken)
    {
        var employee = EmployeeConverter.Convert(request);

        employeeRepository.Update(employee);

        await employeeRepository.SaveChangesAsync(cancellationToken);

        return EmployeeConverter.Convert(employee);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        employeeRepository.Delete(id);

        await employeeRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<EmployeeResponse>> GetByPositionIdAsync(Guid positionId, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        var employees = await employeeRepository.GetByPositionIdAsync(positionId, cancellationToken, page, perPage);

        return EmployeeConverter.Convert(employees);
    }
}
