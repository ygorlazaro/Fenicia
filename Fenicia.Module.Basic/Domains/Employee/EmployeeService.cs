using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Fenicia.Module.Basic.Domains.Dashboard;
using Fenicia.Module.Basic.Domains.Employee.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee;

public class EmployeeService(
    EmployeeRepository employeeRepository,
    PersonRepository personRepository,
    AddressRepository addressRepository,
    PersonAddressRepository personAddressRepository,
    PositionRepository positionRepository,
    DashboardRepository dashboardRepository)
{
    public async Task<Pagination<List<GetAllEmployeeResponse>>> GetAllAsync(GetAllEmployeeQuery query, CancellationToken ct)
    {
        var total = await employeeRepository.CountAsync(ct);

        var employees = await employeeRepository.GetAllWithDetailsAsync(query.Page, query.PerPage, ct);

        var response = employees.Select(e =>
    {
        var personAddress = e.Person.PersonAddresses.FirstOrDefault();
        var address = personAddress?.Address;

            return new GetAllEmployeeResponse(
                e.Id,
                e.PositionId,
                e.PersonId,
                e.Person.Name,
                e.Person.Email,
                e.Person.PhoneNumber,
                e.Person.Document,
                e.Position.Name,
                address != null ? new AddressResponse(
                    address.Id,
                    address.Street,
                    address.Number,
                    address.Complement,
                    address.Neighborhood,
                    address.ZipCode,
                    address.StateId,
                    address.State?.Name,
                    address.City,
                    address.Country
                ) : null
            );
        }).ToList();

        return new Pagination<List<GetAllEmployeeResponse>>(response, total, query.Page, query.PerPage);
    }

    public async Task<GetEmployeeByIdResponse?> GetByIdAsync(GetEmployeeByIdQuery query, CancellationToken ct)
    {
        var employee = await employeeRepository.GetByIdWithDetailsAsync(query.Id, ct);

        if (employee == null)
{
    return null;
}

        var personAddress = employee.Person.PersonAddresses.FirstOrDefault();
        var address = personAddress?.Address;

        return new GetEmployeeByIdResponse(
            employee.Id,
            employee.PositionId,
            employee.PersonId,
            employee.Person.Name,
            employee.Person.Email,
            employee.Person.PhoneNumber,
            employee.Person.Document,
            address != null ? new AddressResponse(
                address.Id,
                address.Street,
                address.Number,
                address.Complement,
                address.Neighborhood,
                address.ZipCode,
                address.StateId,
                address.State?.Name,
                address.City,
                address.Country
            ) : null
        );
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

        AddressModel? address = null;

        if (command.Address != null)
        {
            address = new AddressModel
            {
                Id = Guid.NewGuid(),
                Street = command.Address.Street,
                Number = command.Address.Number,
                Complement = command.Address.Complement,
                Neighborhood = command.Address.Neighborhood,
                ZipCode = command.Address.ZipCode,
                StateId = command.Address.StateId,
                City = command.Address.City,
                Country = command.Address.Country
            };
            await addressRepository.InsertAsync(address, ct);
        }

        var employee = new EmployeeModel
        {
            Id = command.Id,
            PositionId = command.PositionId,
            Person = person,
            PersonId = person.Id,
            CompanyId = companyId
        };

        if (address != null)
        {
            var personAddress = new PersonAddressModel
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                AddressId = address.Id,
            };
            await personAddressRepository.InsertAsync(personAddress, ct);
        }

        await personRepository.InsertAsync(person, ct);
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
                existingPersonAddress.Address.Street = command.Address.Street;
                existingPersonAddress.Address.Number = command.Address.Number;
                existingPersonAddress.Address.Complement = command.Address.Complement;
                existingPersonAddress.Address.Neighborhood = command.Address.Neighborhood;
                existingPersonAddress.Address.ZipCode = command.Address.ZipCode;
                existingPersonAddress.Address.StateId = command.Address.StateId;
                existingPersonAddress.Address.City = command.Address.City;
                existingPersonAddress.Address.Country = command.Address.Country;
                await addressRepository.UpdateAsync(existingPersonAddress.Address.Id, existingPersonAddress.Address, ct);
            }
            else
            {
                var newAddress = new AddressModel
                {
                    Id = Guid.NewGuid(),
                    Street = command.Address.Street,
                    Number = command.Address.Number,
                    Complement = command.Address.Complement,
                    Neighborhood = command.Address.Neighborhood,
                    ZipCode = command.Address.ZipCode,
                    StateId = command.Address.StateId,
                    City = command.Address.City,
                    Country = command.Address.Country
                };
                await addressRepository.InsertAsync(newAddress, ct);

                var newPersonAddress = new PersonAddressModel
                {
                    Id = Guid.NewGuid(),
                    PersonId = employee.PersonId,
                    AddressId = newAddress.Id,
                };
                await personAddressRepository.InsertAsync(newPersonAddress, ct);
            }
        }

        await personRepository.UpdateAsync(employee.Person.Id, employee.Person, ct);
        var updated = await employeeRepository.UpdateAsync(command.Id, employee, ct);

        return new UpdateEmployeeResponse(updated.Id, updated.PositionId, employee.PersonId);
    }

    public async Task DeleteAsync(DeleteEmployeeCommand command, CancellationToken ct)
    {
        var employee = await employeeRepository.GetByIdAsync(command.Id, ct);

        if (employee is null)
        {
            return;
        }

        employee.Deleted = DateTime.Now;

        await employeeRepository.UpdateAsync(command.Id, employee, ct);
    }

    public async Task<EmployeePerformanceResponse> GetPerformanceAsync(GetEmployeePerformanceQuery query, CancellationToken ct)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-query.Days);

        var orders = await dashboardRepository.GetEmployeePerformanceOrdersAsync(startDate, endDate, ct);
        var employees = await dashboardRepository.GetAllEmployeesAsync(ct);

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

    public async Task<Pagination<List<GetEmployeesByPositionIdResponse>>> GetByPositionIdAsync(GetEmployeesByPositionIdQuery query, CancellationToken ct)
    {
        var total = await employeeRepository.CountAsync(e => e.PositionId == query.PositionId, ct);

        var employees = await employeeRepository.GetByPositionIdAsync(query.PositionId, query.Page, query.PerPage, ct);

        var response = employees.Select(e =>
    {
        var personAddress = e.Person.PersonAddresses.FirstOrDefault();
        var address = personAddress?.Address;

            return new GetEmployeesByPositionIdResponse(
                e.Id,
                e.PositionId,
                e.PersonId,
                e.Person.Name,
                e.Person.Email,
                e.Person.PhoneNumber,
                e.Person.Document,
                e.Position.Name,
                address != null ? new AddressResponse(
                    address.Id,
                    address.Street,
                    address.Number,
                    address.Complement,
                    address.Neighborhood,
                    address.ZipCode,
                    address.StateId,
                    address.State?.Name,
                    address.City,
                    address.Country
                ) : null
            );
        }).ToList();

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
            else if (e.TotalSales >= summary.AverageSalesPerEmployee * (decimal)1.5)
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
