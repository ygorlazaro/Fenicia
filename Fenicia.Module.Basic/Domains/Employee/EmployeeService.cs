using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Address;
using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Common.DTOs.Basic.Employee;
using Fenicia.Common.DTOs.Basic.Person;
using Fenicia.Common.DTOs.Basic.PersonAddress;
using Fenicia.Common.Exceptions;
using Fenicia.Module.Basic.Domains.Address.Interfaces;
using Fenicia.Module.Basic.Domains.Employee.Interfaces;
using Fenicia.Module.Basic.Domains.Order.Interfaces;
using Fenicia.Module.Basic.Domains.Person.Interfaces;
using Fenicia.Module.Basic.Domains.PersonAddress.Interfaces;

namespace Fenicia.Module.Basic.Domains.Employee;

public sealed class EmployeeService(
    IEmployeeRepository repository,
    IPersonService personService,
    IAddressService addressService,
    IPersonAddressService personAddressService,
    IOrderService orderService) : IEmployeeService
{
    public EmployeeService()
        : this(null!, null!, null!, null!, null!)
    {
    }

    public async Task<Pagination<List<GetAllEmployeeResponse>>> GetAllAsync(
        GetAllEmployeeQuery query,
        CancellationToken cancellationToken = default)
    {
        var total = await repository.CountAsync(cancellationToken);
        var employees = await repository.GetAllWithDetailsAsync(query.Page, query.PerPage, cancellationToken);

        var response = employees.Select(e =>
        {
            var address = e.Person.PersonAddresses.FirstOrDefault()?.Address;
            return new GetAllEmployeeResponse(
                e.Id,
                e.PositionId,
                e.PersonId,
                e.Person.Name,
                e.Person.Email,
                e.Person.PhoneNumber,
                e.Person.Document,
                e.Position?.Name,
                address != null
                    ? new AddressResponse(
                        address.Id,
                        address.Street,
                        address.Number,
                        address.Complement,
                        address.Neighborhood,
                        address.ZipCode!,
                        address.StateId,
                        address.State?.Name,
                        address.City,
                        address.Country)
                    : null);
        }).ToList();

        return new Pagination<List<GetAllEmployeeResponse>>(response, total, query.Page, query.PerPage);
    }

    public async Task<List<GetAllEmployeeForDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default)
    {
        var employees = await repository.GetAllWithDetailsAsync(cancellationToken: cancellationToken);

        return [.. employees.Select(e => new GetAllEmployeeForDataSourceResponse(e.Id, e.Person.Name))];
    }

    public async Task<GetEmployeeByIdResponse?> GetByIdAsync(
        GetEmployeeByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var employee = await repository.GetByIdWithDetailsAsync(query.Id, cancellationToken);

        if (employee is null)
        {
            return null;
        }

        var address = employee.Person.PersonAddresses.FirstOrDefault()?.Address;
        return new GetEmployeeByIdResponse(
            employee.Id,
            employee.PositionId,
            employee.PersonId,
            employee.Person.Name,
            employee.Person.Email,
            employee.Person.PhoneNumber,
            employee.Person.Document,
            address != null
                ? new AddressResponse(
                    address.Id,
                    address.Street,
                    address.Number,
                    address.Complement,
                    address.Neighborhood,
                    address.ZipCode!,
                    address.StateId,
                    address.State?.Name,
                    address.City,
                    address.Country)
                : null);
    }

    public async Task<AddEmployeeResponse> AddAsync(
        AddEmployeeRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var personCommand = new UpsertPersonRequest(
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

        var employee = new EmployeeModel
        {
            Id = command.Id,
            PositionId = command.PositionId,
            PersonId = personResponse.Id,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(employee, cancellationToken);

        if (!addressId.HasValue)
        {
            return new AddEmployeeResponse(created.Id, created.PositionId, created.PersonId);
        }

        var personAddressCommand = new AddPersonAddressRequest(personResponse.Id, addressId.Value);
        await personAddressService.InsertAsync(personAddressCommand, companyId, cancellationToken);

        return new AddEmployeeResponse(created.Id, created.PositionId, created.PersonId);
    }

    public async Task<UpdateEmployeeResponse?> UpdateAsync(
        UpdateEmployeeRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var employee = await repository.GetByIdWithDetailsAsync(command.Id, cancellationToken);

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
                var personAddressCommand = new AddPersonAddressRequest(newPersonAddress.PersonId, newPersonAddress.AddressId);
                await personAddressService.InsertAsync(personAddressCommand, companyId, cancellationToken);
            }
        }

        var personCommand = new UpsertPersonRequest(
            employee.Person.Name,
            employee.Person.Document,
            employee.Person.Email,
            employee.Person.PhoneNumber,
            null,
            null,
            null);

        await personService.UpdateAsync(employee.Person.Id, personCommand, companyId, cancellationToken);
        var updated = await repository.UpdateAsync(command.Id, employee, cancellationToken) ??
                      throw new ForbiddenException();
        return new UpdateEmployeeResponse(updated.Id, updated.PositionId, employee.PersonId);
    }

    public async Task DeleteAsync(
        DeleteEmployeeRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }

    public async Task<EmployeePerformanceResponse> GetPerformanceAsync(
        GetEmployeePerformanceQuery query,
        CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-query.Days);

        var orders = await orderService.GetEmployeePerformanceOrdersAsync(startDate, endDate, cancellationToken);
        var employees = await GetAllEmployeesAsync(cancellationToken);

        var summary = GetEmployeePerformanceSummary(orders, employees);
        var salesByEmployee = GetSalesByEmployee(orders);
        var ordersByEmployee = GetOrdersByEmployee(orders, employees);
        var topPerformers = GetTopPerformer(query, salesByEmployee, summary);

        return new EmployeePerformanceResponse
        {
            Summary = summary,
            SalesByEmployee = salesByEmployee,
            OrdersByEmployee = ordersByEmployee,
            TopPerformers = topPerformers
        };
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

    public async Task<Pagination<List<GetEmployeesByPositionIdResponse>>> GetByPositionIdAsync(
        GetEmployeesByPositionIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var total = await repository.CountAsync(e => e.PositionId == query.PositionId, cancellationToken);

        var employees = await repository.GetByPositionIdAsync(
            query.PositionId,
            query.Page,
            query.PerPage,
            cancellationToken);

        var response = employees.Select(e =>
        {
            var address = e.Person.PersonAddresses.FirstOrDefault()?.Address;
            return new GetEmployeesByPositionIdResponse(
                e.Id,
                e.PositionId,
                e.PersonId,
                e.Person.Name,
                e.Person.Email,
                e.Person.PhoneNumber,
                e.Person.Document,
                e.Position?.Name,
                address != null
                    ? new AddressResponse(
                        address.Id,
                        address.Street,
                        address.Number,
                        address.Complement,
                        address.Neighborhood,
                        address.ZipCode!,
                        address.StateId,
                        address.State?.Name,
                        address.City,
                        address.Country)
                    : null);
        }).ToList();

        return new Pagination<List<GetEmployeesByPositionIdResponse>>(response, total, query.Page, query.PerPage);
    }

    private static EmployeePerformanceSummaryResponse GetEmployeePerformanceSummary(
        IEnumerable<Fenicia.Common.Data.Models.Basic.OrderModel> orders,
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

    private static List<TopPerformerResponse> GetTopPerformer(
        GetEmployeePerformanceQuery query,
        IEnumerable<EmployeeSalesResponse> salesByEmployee,
        EmployeePerformanceSummaryResponse summary)
    {
        var topPerformers = salesByEmployee.Take(query.TopLimit).Select(e =>
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
