using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Exceptions;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.Employee.DTOs;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Person;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee;

public class EmployeeService(
    IEmployeeRepository employeeRepository,
    PersonService personService,
    AddressService addressService,
    PersonAddressService personAddressService,
    OrderService orderService)
{
    public virtual async Task<Pagination<List<GetAllEmployeeResponse>>> GetAllAsync(GetAllEmployeeQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = employeeRepository.Query()
            .Include(e => e.Person)
            .Include(e => e.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .Include(e => e.Position);

        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);

        var total = await filteredQuery.CountAsync(cancellationToken);

        var employees = await filteredQuery
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);

        var response = employees.Select(e => e.MapToGetAllEmployeeResponse()).ToList();

        return new Pagination<List<GetAllEmployeeResponse>>(response, total, query.Page, query.PerPage);
    }

    public virtual async Task<List<GetAllEmployeeForDataSourceResponse>> GetAllForDataSourceAsync(CancellationToken cancellationToken = default)
    {
        var employees = await employeeRepository.GetAllWithDetailsAsync(cancellationToken: cancellationToken);

        return [.. employees.Select(e => new GetAllEmployeeForDataSourceResponse(e.Id, e.Person.Name))];
    }

    public virtual async Task<GetEmployeeByIdResponse?> GetByIdAsync(GetEmployeeByIdQuery query, CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdWithDetailsAsync(query.Id, cancellationToken);

        return employee?.MapToGetEmployeeByIdResponse();
    }

    public virtual async Task<AddEmployeeResponse> AddAsync(AddEmployeeCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            Document = command.Document,
            PhoneNumber = command.PhoneNumber,
            CompanyId = companyId
        };

        if (command.Address != null)
        {
            var addressCommand = new AddressCommand(command.Address.Street, command.Address.Number, command.Address.Complement, command.Address.Neighborhood, command.Address.ZipCode, command.Address.StateId, command.Address.City, command.Address.Country);
            var createdAddress = await addressService.AddAsync(addressCommand, cancellationToken);

            var personAddress = new PersonAddressModel
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                AddressId = createdAddress.Id,
            };
            await personAddressService.InsertAsync(personAddress, companyId, cancellationToken);
        }

        var employee = new EmployeeModel
        {
            Id = command.Id,
            PositionId = command.PositionId,
            Person = person,
            PersonId = person.Id,
            CompanyId = companyId
        };

        await personService.InsertAsync(person, companyId, cancellationToken);
        var created = await employeeRepository.InsertAsync(employee, cancellationToken);

        return new AddEmployeeResponse(created.Id, created.PositionId, created.PersonId);
    }

    public virtual async Task<UpdateEmployeeResponse?> UpdateAsync(UpdateEmployeeCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdWithDetailsAsync(command.Id, cancellationToken);

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
                var addressCommand = new AddressCommand(command.Address.Street, command.Address.Number, command.Address.Complement, command.Address.Neighborhood, command.Address.ZipCode, command.Address.StateId, command.Address.City, command.Address.Country);
                await addressService.UpdateAsync(existingPersonAddress.Address.Id, addressCommand, cancellationToken);
            }
            else
            {
                var addressCommand = new AddressCommand(command.Address.Street, command.Address.Number, command.Address.Complement, command.Address.Neighborhood, command.Address.ZipCode, command.Address.StateId, command.Address.City, command.Address.Country);
                var createdAddress = await addressService.AddAsync(addressCommand, cancellationToken);

                var newPersonAddress = new PersonAddressModel
                {
                    Id = Guid.NewGuid(),
                    PersonId = employee.PersonId,
                    AddressId = createdAddress.Id,
                };
                await personAddressService.InsertAsync(newPersonAddress, companyId, cancellationToken);
            }
        }

        await personService.UpdateAsync(employee.Person.Id, employee.Person, companyId, cancellationToken);
        var updated = await employeeRepository.UpdateAsync(command.Id, employee, cancellationToken) ?? throw new ItemNotExistsException();
        return new UpdateEmployeeResponse(updated.Id, updated.PositionId, employee.PersonId);
    }

    public virtual async Task DeleteAsync(DeleteEmployeeCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        await employeeRepository.DeleteAsync(command.Id, cancellationToken);
    }

    public virtual async Task<EmployeePerformanceResponse> GetPerformanceAsync(GetEmployeePerformanceQuery query, CancellationToken cancellationToken = default)
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

    public virtual async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await employeeRepository.CountAsync(cancellationToken);
    }

    public virtual async Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return await employeeRepository.GetAllEmployeesAsync(cancellationToken);
    }

    public virtual async Task<int> GetTotalEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return await employeeRepository.CountAsync(cancellationToken);
    }

    public virtual async Task<Pagination<List<GetEmployeesByPositionIdResponse>>> GetByPositionIdAsync(GetEmployeesByPositionIdQuery query, CancellationToken cancellationToken = default)
    {
        var total = await employeeRepository.CountAsync(e => e.PositionId == query.PositionId, cancellationToken);

        var employees = await employeeRepository.GetByPositionIdAsync(query.PositionId, query.Page, query.PerPage, cancellationToken);

        var response = employees.Select(e => e.MapToGetEmployeesByPositionIdResponse()).ToList();

        return new Pagination<List<GetEmployeesByPositionIdResponse>>(response, total, query.Page, query.PerPage);
    }

    private static EmployeePerformanceSummaryResponse GetEmployeePerformanceSummary(IEnumerable<OrderModel> orders, IEnumerable<EmployeeModel> employees)
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

    private static List<TopPerformerResponse> GetTopPerformer(GetEmployeePerformanceQuery query, List<EmployeeSalesResponse> salesByEmployee, EmployeePerformanceSummaryResponse summary)
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

            return new TopPerformerResponse(e.EmployeeId, e.EmployeeName, e.PositionName, e.TotalSales, e.TotalOrders, performanceLevel);
        }).ToList();
        return topPerformers;
    }

    private static List<EmployeeOrderCountResponse> GetOrdersByEmployee(IEnumerable<OrderModel> orders, IEnumerable<EmployeeModel> employees)
    {
        var ordersList = orders.Where(o => o.EmployeeId.HasValue).ToList();

        var ordersByEmployee = ordersList.GroupBy(o => o.EmployeeId!.Value).Select(g =>
        {
            var employee = employees.First(e => e.Id == g.Key);
            return new EmployeeOrderCountResponse(g.Key, employee.Person.Name, employee.Position.Name, g.Count(), g.Sum(o => o.TotalAmount), g.Min(o => o.SaleDate), g.Max(o => o.SaleDate));
        }).OrderByDescending(e => e.OrderCount).ToList();

        return ordersByEmployee;
    }

    private static List<EmployeeSalesResponse> GetSalesByEmployee(IEnumerable<OrderModel> orders)
    {
        var ordersList = orders.Where(o => o.Employee != null).ToList();

        var data = ordersList.GroupBy(o => o.Employee!.Id).Select(g =>
        {
            var employee = g.First().Employee!;
            return new EmployeeSalesResponse(employee.Id, employee.Person.Name, employee.Position.Name, g.Sum(o => o.TotalAmount), g.Count(), g.Sum(o => o.TotalAmount), 0);
        }).ToList();

        for (var i = 0; i < data.Count; i++)
        {
            data[i] = data[i] with { Rank = i + 1 };
        }

        return data;
    }
}
