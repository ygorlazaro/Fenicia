using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Auth.Address;
using Fenicia.Common.DTOs.Auth.Person;
using Fenicia.Common.DTOs.Auth.PersonAddress;
using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Common.DTOs.Basic.Employee;
using Fenicia.Common.Exceptions;
using Fenicia.Auth.Domains.Address.Interfaces;
using Fenicia.Module.Basic.Domains.Employee.Interfaces;
using Fenicia.Module.Basic.Domains.Order.Interfaces;
using Fenicia.Auth.Domains.Person.Interfaces;
using Fenicia.Auth.Domains.PersonAddress.Interfaces;

namespace Fenicia.Module.Basic.Domains.Employee;

/// <summary>
/// Service implementation for managing employees.
/// </summary>
/// <param name="repository">The employee repository.</param>
/// <param name="personService">The person service.</param>
/// <param name="addressService">The address service.</param>
/// <param name="personAddressService">The person address service.</param>
/// <param name="orderService">The order service.</param>
public sealed class EmployeeService(
    IEmployeeRepository repository,
    IPersonService personService,
    IAddressService addressService,
    IPersonAddressService personAddressService,
    IOrderService orderService) : IEmployeeService
{
    /// <inheritdoc />
    public async Task<Pagination<List<EmployeeResponse>>> GetAllAsync(
        EmployeeQuery query,
        CancellationToken cancellationToken = default)
    {
        var total = await repository.CountAsync(cancellationToken);
        var employees = await repository.GetAllWithDetailsAsync(query.Page, query.PerPage, cancellationToken);

        var response = employees.Select(EmployeeMapper.MapToResponse).ToList();

        return new Pagination<List<EmployeeResponse>>(response, total, query);
    }

    /// <inheritdoc />
    public async Task<List<EmployeeDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default)
    {
        var employees = await repository.GetAllEmployeesAsync(cancellationToken);

        return [.. employees.Select(EmployeeMapper.MapToDataSourceResponse)];
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task DeleteAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<EmployeePerformanceResponse> GetPerformanceAsync(
        EmployeePerformanceRequest request,
        CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-request.Days);

        var orders = await orderService.GetEmployeePerformanceOrdersAsync(startDate, endDate, cancellationToken);
        var employees = await GetAllEmployeesAsync(cancellationToken);

        var summary = GetEmployeePerformanceSummary(orders, employees);
        var salesByEmployee = GetSalesByEmployee(orders);
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
        List<Fenicia.Common.DTOs.Basic.Order.OrderResponse> orders)
    {
        return orders
            .GroupBy(o => o.EmployeeId)
            .Where(g => g.Key.HasValue)
            .Select(g => new EmployeeSalesResponse(
                g.Key.Value,
                g.First().EmployeeName ?? string.Empty,
                g.Sum(o => o.TotalAmount),
                g.Count()))
            .ToList();
    }

    private static List<EmployeeOrderCountResponse> GetOrdersByEmployee(
        List<Fenicia.Common.DTOs.Basic.Order.OrderResponse> orders,
        List<EmployeeModel> employees)
    {
        return orders
            .GroupBy(o => o.EmployeeId)
            .Where(g => g.Key.HasValue)
            .Select(g => new EmployeeOrderCountResponse(
                g.Key.Value,
                g.First().EmployeeName ?? string.Empty,
                g.Count()))
            .ToList();
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

    /// <inheritdoc />
    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return repository.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetAllEmployeesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> GetTotalEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return repository.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
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

    /// <summary>
    /// Calculates the employee performance summary.
    /// </summary>
    /// <param name="orders">The orders.</param>
    /// <param name="employees">The employees.</param>
    /// <returns>The performance summary response.</returns>
    private static EmployeePerformanceSummaryResponse GetEmployeePerformanceSummary(
        IEnumerable<Fenicia.Common.DTOs.Basic.Order.OrderResponse> orders,
        IEnumerable<EmployeeModel> employees)
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

    /// <summary>
    /// Gets the top performers.
    /// </summary>
    /// <param name="request">The performance request.</param>
    /// <param name="salesByEmployee">The sales by employee.</param>
    /// <param name="summary">The performance summary.</param>
    /// <returns>A list of top performer responses.</returns>
    private static List<TopPerformerResponse> GetTopPerformer(
        EmployeePerformanceRequest request,
        IEnumerable<EmployeeSalesResponse> salesByEmployee,
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

    /// <summary>
    /// Gets orders by employee.
    /// </summary>
    /// <param name="orders">The orders.</param>
    /// <param name="employees">The employees.</param>
    /// <returns>A list of employee order count responses.</returns>
    private static List<EmployeeOrderCountResponse> GetOrdersByEmployee(
        IEnumerable<Fenicia.Common.Data.Models.Basic.OrderModel> orders,
        IEnumerable<EmployeeModel> employees)
    {
        var ordersList = orders.Where(o => o.EmployeeId.HasValue).ToList();

        var ordersByEmployee = ordersList.GroupBy(o => o.EmployeeId!.Value).Select(g =>
        {
            var employee = employees.First(e => e.Id == g.Key);
            return new EmployeeOrderCountResponse(
                g.Key,
                employee.Person.Name,
                employee.Position.Name,
                g.Count(),
                g.Sum(o => o.TotalAmount),
                g.Min(o => o.SaleDate),
                g.Max(o => o.SaleDate));
        }).OrderByDescending(e => e.OrderCount).ToList();

        return ordersByEmployee;
    }

    /// <summary>
    /// Gets sales by employee.
    /// </summary>
    /// <param name="orders">The orders.</param>
    /// <returns>A list of employee sales responses.</returns>
    private static List<EmployeeSalesResponse> GetSalesByEmployee(IEnumerable<Fenicia.Common.Data.Models.Basic.OrderModel> orders)
    {
        var ordersList = orders.Where(o => o.Employee != null).ToList();

        var data = ordersList.GroupBy(o => o.Employee!.Id).Select(g =>
        {
            var employee = g.First().Employee!;
            return new EmployeeSalesResponse(
                employee.Id,
                employee.Person.Name,
                employee.Position.Name,
                g.Sum(o => o.TotalAmount),
                g.Count(),
                g.Sum(o => o.TotalAmount),
                0);
        }).ToList();

        for (var i = 0; i < data.Count; i++)
        {
            data[i].Rank = i + 1;
        }

        return data;
    }
}
