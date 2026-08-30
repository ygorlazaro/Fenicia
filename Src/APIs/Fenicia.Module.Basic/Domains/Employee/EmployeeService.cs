using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
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

public class EmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly PersonService _personService;
    private readonly AddressService _addressService;
    private readonly PersonAddressService _personAddressService;
    private readonly OrderService _orderService;

    public EmployeeService()
        : this(null!, null!, null!, null!, null!)
    {
    }

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        PersonService personService,
        AddressService addressService,
        PersonAddressService personAddressService,
        OrderService orderService)
    {
        _employeeRepository = employeeRepository;
        _personService = personService;
        _addressService = addressService;
        _personAddressService = personAddressService;
        _orderService = orderService;
    }

    public virtual async Task<Pagination<List<GetAllEmployeeResponse>>> GetAllAsync(GetAllEmployeeQuery query, CancellationToken ct)
    {
        var total = await _employeeRepository.CountAsync(ct);

        var employees = await _employeeRepository.GetAllWithDetailsAsync(query.Page, query.PerPage, ct);

        var response = employees.Select(e => e.MapToGetAllEmployeeResponse()).ToList();

        return new Pagination<List<GetAllEmployeeResponse>>(response, total, query.Page, query.PerPage);
    }

    public virtual async Task<List<GetAllEmployeeForDataSourceResponse>> GetAllForDataSourceAsync(CancellationToken ct)
    {
        var employees = await _employeeRepository.GetAllWithDetailsAsync(ct: ct);

        return employees.Select(e => new GetAllEmployeeForDataSourceResponse(e.Id, e.Person.Name)).ToList();
    }

    public virtual async Task<GetEmployeeByIdResponse?> GetByIdAsync(GetEmployeeByIdQuery query, CancellationToken ct)
    {
        var employee = await _employeeRepository.GetByIdWithDetailsAsync(query.Id, ct);

        if (employee == null)
        {
            return null;
        }

        return employee.MapToGetEmployeeByIdResponse();
    }

    public virtual async Task<AddEmployeeResponse> AddAsync(AddEmployeeCommand command, Guid companyId, CancellationToken ct)
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
            var createdAddress = await _addressService.AddAsync(addressCommand, ct);

            var personAddress = new PersonAddressModel
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                AddressId = createdAddress.Id,
            };
            await _personAddressService.InsertAsync(personAddress, companyId, ct);
        }

        var employee = new EmployeeModel
        {
            Id = command.Id,
            PositionId = command.PositionId,
            Person = person,
            PersonId = person.Id,
            CompanyId = companyId
        };

        await _personService.InsertAsync(person, companyId, ct);
        var created = await _employeeRepository.InsertAsync(employee, ct);

        return new AddEmployeeResponse(created.Id, created.PositionId, created.PersonId);
    }

    public virtual async Task<UpdateEmployeeResponse?> UpdateAsync(UpdateEmployeeCommand command, Guid companyId, CancellationToken ct)
    {
        var employee = await _employeeRepository.GetByIdWithDetailsAsync(command.Id, ct);

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
                await _addressService.UpdateAsync(existingPersonAddress.Address.Id, addressCommand, ct);
            }
            else
            {
                var addressCommand = new AddressCommand(command.Address.Street, command.Address.Number, command.Address.Complement, command.Address.Neighborhood, command.Address.ZipCode, command.Address.StateId, command.Address.City, command.Address.Country);
                var createdAddress = await _addressService.AddAsync(addressCommand, ct);

                var newPersonAddress = new PersonAddressModel
                {
                    Id = Guid.NewGuid(),
                    PersonId = employee.PersonId,
                    AddressId = createdAddress.Id,
                };
                await _personAddressService.InsertAsync(newPersonAddress, companyId, ct);
            }
        }

        await _personService.UpdateAsync(employee.Person.Id, employee.Person, companyId, ct);
        var updated = await _employeeRepository.UpdateAsync(command.Id, employee, ct) ?? throw new ItemNotExistsException();
        return new UpdateEmployeeResponse(updated.Id, updated.PositionId, employee.PersonId);
    }

    public virtual async Task DeleteAsync(DeleteEmployeeCommand command, Guid companyId, CancellationToken ct)
    {
        await _employeeRepository.DeleteAsync(command.Id, ct);
    }

    public virtual async Task<EmployeePerformanceResponse> GetPerformanceAsync(GetEmployeePerformanceQuery query, CancellationToken ct)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-query.Days);

        var orders = await _orderService.GetEmployeePerformanceOrdersAsync(startDate, endDate, ct);
        var employees = await GetAllEmployeesAsync(ct);

        var summary = await GetEmployeePerformanceSummaryAsync(orders, employees, ct);
        var salesByEmployee = GetSalesByEmployee(orders, employees);
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

    public virtual async Task<int> GetCountAsync(CancellationToken ct)
    {
        return await _employeeRepository.CountAsync(ct);
    }

    public virtual async Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken ct)
    {
        return await _employeeRepository.GetAllEmployeesAsync(ct);
    }

    public virtual async Task<int> GetTotalEmployeesAsync(CancellationToken ct)
    {
        return await _employeeRepository.CountAsync(ct);
    }

    public virtual async Task<Pagination<List<GetEmployeesByPositionIdResponse>>> GetByPositionIdAsync(GetEmployeesByPositionIdQuery query, CancellationToken ct)
    {
        var total = await _employeeRepository.CountAsync(e => e.PositionId == query.PositionId, ct);

        var employees = await _employeeRepository.GetByPositionIdAsync(query.PositionId, query.Page, query.PerPage, ct);

        var response = employees.Select(e => e.MapToGetEmployeesByPositionIdResponse()).ToList();

        return new Pagination<List<GetEmployeesByPositionIdResponse>>(response, total, query.Page, query.PerPage);
    }

    private List<TopPerformerResponse> GetTopPerformer(GetEmployeePerformanceQuery query, List<EmployeeSalesResponse> salesByEmployee, EmployeePerformanceSummaryResponse summary)
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

    private List<EmployeeOrderCountResponse> GetOrdersByEmployee(IEnumerable<Fenicia.Common.Data.Models.Basic.OrderModel> orders, IEnumerable<EmployeeModel> employees)
    {
        var ordersList = orders.Where(o => o.EmployeeId.HasValue).ToList();

        var ordersByEmployee = ordersList.GroupBy(o => o.EmployeeId!.Value).Select(g =>
        {
            var employee = employees.First(e => e.Id == g.Key);
            return new EmployeeOrderCountResponse(g.Key, employee.Person.Name, employee.Position.Name, g.Count(), g.Sum(o => o.TotalAmount), g.Min(o => o.SaleDate), g.Max(o => o.SaleDate));
        }).OrderByDescending(e => e.OrderCount).ToList();

        return ordersByEmployee;
    }

    private List<EmployeeSalesResponse> GetSalesByEmployee(IEnumerable<Fenicia.Common.Data.Models.Basic.OrderModel> orders, IEnumerable<EmployeeModel> employees)
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
