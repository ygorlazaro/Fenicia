using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Exceptions;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Fenicia.Module.Basic.Domains.Dashboard;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.Employee.DTOs;
using Fenicia.Module.Basic.Domains.Person;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee;

public class EmployeeService(
    EmployeeRepository employeeRepository,
    PersonService personService,
    AddressService addressService,
    PersonAddressService personAddressService,
    DashboardService dashboardService)
{
    public async Task<Pagination<List<GetAllEmployeeResponse>>> GetAllAsync(GetAllEmployeeQuery query, CancellationToken ct)
    {
        var total = await employeeRepository.CountAsync(ct);

        var employees = await employeeRepository.GetAllWithDetailsAsync(query.Page, query.PerPage, ct);

        var response = employees.Select(e => e.MapToGetAllEmployeeResponse()).ToList();

        return new Pagination<List<GetAllEmployeeResponse>>(response, total, query.Page, query.PerPage);
    }

    public async Task<List<GetAllEmployeeForDataSourceResponse>> GetAllForDataSourceAsync(CancellationToken ct)
    {
        var employees = await employeeRepository.GetAllWithDetailsAsync(ct: ct);

        return employees.Select(e => new GetAllEmployeeForDataSourceResponse(e.Id, e.Person.Name)).ToList();
    }

    public async Task<GetEmployeeByIdResponse?> GetByIdAsync(GetEmployeeByIdQuery query, CancellationToken ct)
    {
        var employee = await employeeRepository.GetByIdWithDetailsAsync(query.Id, ct);

        if (employee == null)
        {
            return null;
        }

        return employee.MapToGetEmployeeByIdResponse();
    }

    public async Task<AddEmployeeResponse> AddAsync(AddEmployeeCommand command, Guid companyId, CancellationToken ct)
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
            var createdAddress = await addressService.AddAsync(addressCommand, ct);

            var personAddress = new PersonAddressModel
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                AddressId = createdAddress.Id,
            };
            await personAddressService.InsertAsync(personAddress, companyId, ct);
        }

        var employee = new EmployeeModel
        {
            Id = command.Id,
            PositionId = command.PositionId,
            Person = person,
            PersonId = person.Id,
            CompanyId = companyId
        };

        await personService.InsertAsync(person, companyId, ct);
        var created = await employeeRepository.InsertAsync(employee, ct);

        return new AddEmployeeResponse(created.Id, created.PositionId, created.PersonId);
    }

    public async Task<UpdateEmployeeResponse?> UpdateAsync(UpdateEmployeeCommand command, Guid companyId, CancellationToken ct)
    {
        var employee = await employeeRepository.GetByIdWithDetailsAsync(command.Id, ct);

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
                await addressService.UpdateAsync(existingPersonAddress.Address.Id, addressCommand, ct);
            }
            else
            {
                var addressCommand = new AddressCommand(command.Address.Street, command.Address.Number, command.Address.Complement, command.Address.Neighborhood, command.Address.ZipCode, command.Address.StateId, command.Address.City, command.Address.Country);
                var createdAddress = await addressService.AddAsync(addressCommand, ct);

                var newPersonAddress = new PersonAddressModel
                {
                    Id = Guid.NewGuid(),
                    PersonId = employee.PersonId,
                    AddressId = createdAddress.Id,
                };
                await personAddressService.InsertAsync(newPersonAddress, companyId, ct);
            }
        }

        await personService.UpdateAsync(employee.Person.Id, employee.Person, companyId, ct);
        var updated = await employeeRepository.UpdateAsync(command.Id, employee, ct) ?? throw new ItemNotExistsException();
        return new UpdateEmployeeResponse(updated.Id, updated.PositionId, employee.PersonId);
    }

    public async Task DeleteAsync(DeleteEmployeeCommand command, CancellationToken ct)
    {
        var employee = await employeeRepository.GetByIdAsync(command.Id, ct);

        if (employee is null)
        {
            return;
        }

        employee.Deleted = DateTime.UtcNow;

        await employeeRepository.UpdateAsync(command.Id, employee, ct);
    }

    public async Task<EmployeePerformanceResponse> GetPerformanceAsync(GetEmployeePerformanceQuery query, CancellationToken ct)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-query.Days);

        var orders = await dashboardService.GetEmployeePerformanceOrdersAsync(startDate, endDate, ct);
        var employees = await dashboardService.GetAllEmployeesAsync(ct);

        var summary = await GetEmployeePerformanceSummaryAsync(orders, employees, ct);
        var salesByEmployee = GetSalesByEmployeeAsync(orders, employees);
        var ordersByEmployee = GetOrdersByEmployeeAsync(orders, employees);
        var topPerformers = GetTopPerformerAsync(query, salesByEmployee, summary);

        return new EmployeePerformanceResponse
        {
            Summary = summary,
            SalesByEmployee = salesByEmployee,
            OrdersByEmployee = ordersByEmployee,
            TopPerformers = topPerformers
        };
    }

    public async Task<int> GetCountAsync(CancellationToken ct)
    {
        return await employeeRepository.CountAsync(ct);
    }

    public async Task<Pagination<List<GetEmployeesByPositionIdResponse>>> GetByPositionIdAsync(GetEmployeesByPositionIdQuery query, CancellationToken ct)
    {
        var total = await employeeRepository.CountAsync(e => e.PositionId == query.PositionId, ct);

        var employees = await employeeRepository.GetByPositionIdAsync(query.PositionId, query.Page, query.PerPage, ct);

        var response = employees.Select(e => e.MapToGetEmployeesByPositionIdResponse()).ToList();

        return new Pagination<List<GetEmployeesByPositionIdResponse>>(response, total, query.Page, query.PerPage);
    }

    private List<TopPerformerResponse> GetTopPerformerAsync(GetEmployeePerformanceQuery query, List<EmployeeSalesResponse> salesByEmployee, EmployeePerformanceSummaryResponse summary)
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

    private List<EmployeeOrderCountResponse> GetOrdersByEmployeeAsync(IEnumerable<Fenicia.Common.Data.Models.Basic.OrderModel> orders, IEnumerable<EmployeeModel> employees)
    {
        var ordersList = orders.Where(o => o.EmployeeId.HasValue).ToList();

        var ordersByEmployee = ordersList.GroupBy(o => o.EmployeeId!.Value).Select(g =>
        {
            var employee = employees.First(e => e.Id == g.Key);
            return new EmployeeOrderCountResponse(g.Key, employee.Person.Name, employee.Position.Name, g.Count(), g.Sum(o => o.TotalAmount), g.Min(o => o.SaleDate), g.Max(o => o.SaleDate));
        }).OrderByDescending(e => e.OrderCount).ToList();

        return ordersByEmployee;
    }

    private List<EmployeeSalesResponse> GetSalesByEmployeeAsync(IEnumerable<Fenicia.Common.Data.Models.Basic.OrderModel> orders, IEnumerable<EmployeeModel> employees)
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

    private async Task<EmployeePerformanceSummaryResponse> GetEmployeePerformanceSummaryAsync(IEnumerable<Fenicia.Common.Data.Models.Basic.OrderModel> orders, IEnumerable<EmployeeModel> employees, CancellationToken ct)
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
            AverageOrdersPerEmployee = employeesWithOrders > 0 ? totalOrders / employeesWithOrders : 0
        };
        return summary;
    }
}
