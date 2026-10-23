using Fenicia.Auth.Domains.Address.Interfaces;
using Fenicia.Auth.Domains.Person.Interfaces;
using Fenicia.Auth.Domains.PersonAddress.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Auth.Address;
using Fenicia.Common.DTOs.Auth.Person;
using Fenicia.Common.DTOs.Auth.PersonAddress;
using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Common.DTOs.Basic.Employee;
using Fenicia.Common.DTOs.Basic.Order;
using Fenicia.Common.Exceptions;
using Fenicia.Module.Basic.Domains.Employee.Interfaces;
using Fenicia.Module.Basic.Domains.Order.Interfaces;

namespace Fenicia.Module.Basic.Domains.Employee;

public sealed class EmployeeService(
    IEmployeeRepository repository,
    IPersonService personService,
    IAddressService addressService,
    IPersonAddressService personAddressService,
    IOrderService orderService) : IEmployeeService
{
    public async Task<Pagination<List<EmployeeResponse>>> GetAllAsync(
        EmployeeQuery query,
        CancellationToken cancellationToken = default)
    {
        var total = await repository.CountAsync(cancellationToken);
        var employees = await repository.GetAllWithDetailsAsync(query.Page, query.PerPage, cancellationToken);

        var response = employees.Select(EmployeeMapper.MapToResponse).ToList();

        return new Pagination<List<EmployeeResponse>>(response, total, query);
    }

    public async Task<List<EmployeeDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default)
    {
        var employees = await repository.GetAllEmployeesAsync(cancellationToken);

        return [.. employees.Select(EmployeeMapper.MapToDataSourceResponse)];
    }

    public async Task<EmployeeResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var employee = await repository.GetByIdWithDetailsAsync(id, cancellationToken);

        if (employee is null)
        {
            return null;
        }

        return EmployeeMapper.MapToResponse(employee);
    }

    public async Task<EmployeeResponse> AddAsync(
        EmployeeRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var personCommand = new PersonRequest(
            command.Name,
            command.Document,
            command.Email,
            command.PhoneNumber,
            null,
            null,
            null);

        Guid? addressId = null;

        if (command.Address != null)
        {
            var addressCommand = new AddressRequest(
                command.Address.Street,
                command.Address.Number,
                command.Address.Complement,
                command.Address.Neighborhood,
                command.Address.ZipCode,
                command.Address.StateId,
                command.Address.City,
                command.Address.Country);
            var createdAddress = await addressService.AddAsync(addressCommand, cancellationToken);
            addressId = createdAddress.Id;
        }

        var personResponse = await personService.InsertAsync(personCommand, companyId, cancellationToken);

        var employee = EmployeeMapper.MapToModel(command, companyId, personResponse.Id);

        var created = await repository.InsertAsync(employee, cancellationToken);

        if (addressId.HasValue)
        {
            var personAddressCommand = new PersonAddressRequest(personResponse.Id, addressId.Value);
            await personAddressService.InsertAsync(personAddressCommand, companyId, cancellationToken);
        }

        return EmployeeMapper.MapToResponse(created);
    }

    public async Task<EmployeeResponse?> UpdateAsync(
        Guid id,
        EmployeeRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var employee = await repository.GetByIdWithDetailsAsync(id, cancellationToken);

        if (employee is null)
        {
            return null;
        }

        employee.PositionId = command.PositionId;
        employee.Person.Name = command.Name;
        employee.Person.Email = command.Email;
        employee.Person.Document = command.Document;
        employee.Person.PhoneNumber = command.PhoneNumber;

        if (command.Address != null)
        {
            var existingPersonAddress = employee.Person.PersonAddresses.FirstOrDefault();

            if (existingPersonAddress?.Address != null)
            {
                var addressCommand = new AddressRequest(
                    command.Address.Street,
                    command.Address.Number,
                    command.Address.Complement,
                    command.Address.Neighborhood,
                    command.Address.ZipCode,
                    command.Address.StateId,
                    command.Address.City,
                    command.Address.Country);
                await addressService.UpdateAsync(existingPersonAddress.Address.Id, addressCommand, cancellationToken);
            }
            else
            {
                var addressCommand = new AddressRequest(
                    command.Address.Street,
                    command.Address.Number,
                    command.Address.Complement,
                    command.Address.Neighborhood,
                    command.Address.ZipCode,
                    command.Address.StateId,
                    command.Address.City,
                    command.Address.Country);
                var createdAddress = await addressService.AddAsync(addressCommand, cancellationToken);

                var newPersonAddress = new Fenicia.Common.Data.Models.Auth.PersonAddressModel
                {
                    Id = Guid.NewGuid(),
                    PersonId = employee.PersonId,
                    AddressId = createdAddress.Id
                };
                var personAddressCommand = new PersonAddressRequest(newPersonAddress.PersonId, newPersonAddress.AddressId);
                await personAddressService.InsertAsync(personAddressCommand, companyId, cancellationToken);
            }
        }

        var personCommand = new PersonRequest(
            employee.Person.Name,
            employee.Person.Document,
            employee.Person.Email,
            employee.Person.PhoneNumber,
            null,
            null,
            null);

        await personService.UpdateAsync(employee.Person.Id, personCommand, companyId, cancellationToken);
        var updated = await repository.UpdateAsync(id, employee, cancellationToken) ??
                      throw new ForbiddenException();

        return EmployeeMapper.MapToResponse(updated);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<EmployeePerformanceResponse> GetPerformanceAsync(
        EmployeePerformanceRequest request,
        CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-request.Days);

        var orders = await orderService.GetEmployeePerformanceOrdersAsync(startDate, endDate, cancellationToken);
        var employees = await GetAllEmployeesAsync(cancellationToken);

        var summary = GetEmployeePerformanceSummary(orders, employees);
        var salesByEmployee = GetSalesByEmployee(orders, employees);
        var ordersByEmployee = GetOrdersByEmployee(orders, employees);
        var topPerformers = GetTopPerformer(request, salesByEmployee, summary);

        return new EmployeePerformanceResponse
        {
            Summary = summary,
            SalesByEmployee = salesByEmployee,
            OrdersByEmployee = ordersByEmployee,
            TopPerformers = topPerformers
        };
    }

    private static List<EmployeeSalesResponse> GetSalesByEmployee(
        List<OrderResponse> orders,
        List<EmployeeModel> employees)
    {
        return [.. orders
            .GroupBy(o => o.EmployeeId)
            .Where(g => g.Key.HasValue)
            .Select(g =>
            {
                var employee = employees.FirstOrDefault(e => e.Id == g.Key!.Value);
                return new EmployeeSalesResponse(
                    g.Key!.Value,
                    employee?.Person.Name ?? string.Empty,
                    employee?.Position.Name ?? string.Empty,
                    g.Sum(o => o.TotalAmount),
                    g.Count(),
                    g.Any() ? g.Sum(o => o.TotalAmount) / g.Count() : 0,
                    0);
            })];
    }

    private static List<EmployeeOrderCountResponse> GetOrdersByEmployee(
        List<OrderResponse> orders,
        List<EmployeeModel> employees)
    {
        return [.. orders
            .GroupBy(o => o.EmployeeId)
            .Where(g => g.Key.HasValue)
            .Select(g =>
            {
                var employee = employees.FirstOrDefault(e => e.Id == g.Key!.Value);
                var orderList = g.ToList();
                return new EmployeeOrderCountResponse(
                    g.Key!.Value,
                    employee?.Person.Name ?? string.Empty,
                    employee?.Position.Name ?? string.Empty,
                    orderList.Count,
                    orderList.Sum(o => o.TotalAmount),
                    orderList.Min(o => o.SaleDate),
                    orderList.Max(o => o.SaleDate));
            })];
    }

    private static List<TopPerformerResponse> GetTopPerformer(
        EmployeePerformanceRequest request,
        List<EmployeeSalesResponse> salesByEmployee,
        EmployeePerformanceSummaryResponse summary)
    {
        var topPerformers = salesByEmployee.Take(request.TopLimit).Select(e =>
        {
            var performanceLevel = "Standard";
            if (e.TotalSales >= summary.AverageSalesPerEmployee * 2)
            {
                performanceLevel = "Excellent";
            }
            else if (e.TotalSales >= summary.AverageSalesPerEmployee * 1.5M)
            {
                performanceLevel = "Very Good";
            }
            else if (e.TotalSales >= summary.AverageSalesPerEmployee)
            {
                performanceLevel = "Good";
            }

            return new TopPerformerResponse(
                e.EmployeeId,
                e.EmployeeName,
                e.PositionName,
                e.TotalSales,
                e.TotalOrders,
                performanceLevel);
        }).ToList();

        return topPerformers;
    }

    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return repository.CountAsync(cancellationToken);
    }

    public Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetAllEmployeesAsync(cancellationToken);
    }

    public Task<int> GetTotalEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return repository.CountAsync(cancellationToken);
    }

    public async Task<Pagination<List<EmployeeResponse>>> GetByPositionIdAsync(
        EmployeesByPositionIdRequest query,
        CancellationToken cancellationToken = default)
    {
        var total = await repository.CountAsync(e => e.PositionId == query.PositionId, cancellationToken);

        var employees = await repository.GetByPositionIdAsync(
            query.PositionId,
            query.Page,
            query.PerPage,
            cancellationToken);

        var response = employees.Select(EmployeeMapper.MapToResponse).ToList();

        return new Pagination<List<EmployeeResponse>>(response, total, query);
    }

    private static EmployeePerformanceSummaryResponse GetEmployeePerformanceSummary(
        List<OrderResponse> orders,
        List<EmployeeModel> employees)
    {
        var ordersList = orders.Where(o => o.EmployeeId.HasValue).ToList();

        var employeesWithOrders = ordersList.Select(o => o.EmployeeId!.Value).Distinct().Count();

        var totalSales = ordersList.Sum(o => o.TotalAmount);
        var totalOrders = ordersList.Count;

        var totalEmployees = employees.Count();

        var summary = new EmployeePerformanceSummaryResponse
        {
            TotalEmployees = totalEmployees,
            ActiveEmployees = employeesWithOrders,
            TotalSales = totalSales,
            TotalOrders = totalOrders,
            AverageSalesPerEmployee = employeesWithOrders > 0 ? totalSales / employeesWithOrders : 0,
            AverageOrdersPerEmployee = employeesWithOrders > 0 ? (decimal)totalOrders / employeesWithOrders : 0
        };
        return summary;
    }
}