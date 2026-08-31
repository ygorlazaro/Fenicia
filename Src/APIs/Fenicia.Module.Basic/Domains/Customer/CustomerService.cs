using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Exceptions;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Person;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Fenicia.Module.Basic.Domains.Product;

namespace Fenicia.Module.Basic.Domains.Customer;

public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly PersonService _personService;
    private readonly AddressService _addressService;
    private readonly PersonAddressService _personAddressService;
    private readonly OrderService _orderService;
    private readonly ProductService _productService;

    public CustomerService()
        : this(null!, null!, null!, null!, null!, null!)
    {
    }

    public CustomerService(
        ICustomerRepository customerRepository,
        PersonService personService,
        AddressService addressService,
        PersonAddressService personAddressService,
        OrderService orderService,
        ProductService productService)
    {
        _customerRepository = customerRepository;
        _personService = personService;
        _addressService = addressService;
        _personAddressService = personAddressService;
        _orderService = orderService;
        _productService = productService;
    }

    public virtual async Task<Pagination<List<GetAllCustomerResponse>>> GetAllAsync(GetAllCustomerQuery query, CancellationToken ct)
    {
        var total = await _customerRepository.CountAsync(ct);

        var customers = await _customerRepository.GetAllWithDetailsAsync(query.Page, query.PerPage, ct);

        var response = customers.Select(c => c.MapToGetAllCustomerResponse()).ToList();

        return new Pagination<List<GetAllCustomerResponse>>(response, total, query.Page, query.PerPage);
    }

    public virtual async Task<List<GetAllCustomerForDataSourceResponse>> GetAllForDataSourceAsync(CancellationToken ct)
    {
        var customers = await _customerRepository.GetAllWithDetailsAsync(ct: ct);

        return [.. customers.Select(c => new GetAllCustomerForDataSourceResponse(c.Id, c.Person.Name))];
    }

    public virtual async Task<GetCustomerByIdResponse?> GetByIdAsync(GetCustomerByIdQuery query, CancellationToken ct)
    {
        var customer = await _customerRepository.GetByIdWithDetailsAsync(query.Id, ct);

        if (customer == null)
        {
            return null;
        }

        return customer.MapToGetCustomerByIdResponse();
    }

    public virtual async Task<AddCustomerResponse> AddAsync(AddCustomerCommand command, Guid companyId, CancellationToken ct)
    {
        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            Document = command.Document,
            PhoneNumber = command.PhoneNumber
        };

        if (command.Address != null)
        {
            var addressCommand = new AddressCommand(command.Address.Street, command.Address.Number, command.Address.Complement, command.Address.Neighborhood, command.Address.ZipCode, command.Address.StateId, command.Address.City, command.Address.Country);
            var createdAddress = await _addressService.AddAsync(addressCommand, ct);

            var personAddress = new PersonAddressModel
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                AddressId = createdAddress.Id
            };
            await _personAddressService.InsertAsync(personAddress, companyId, ct);
        }

        var customer = new CustomerModel
        {
            Person = person,
            PersonId = person.Id
        };

        await _personService.InsertAsync(person, companyId, ct);
        var created = await _customerRepository.InsertAsync(customer, ct);

        return new AddCustomerResponse(created.Id, person.Id);
    }

    public virtual async Task<UpdateCustomerResponse?> UpdateAsync(UpdateCustomerCommand command, Guid companyId, CancellationToken ct)
    {
        var customer = await _customerRepository.GetByIdWithDetailsAsync(command.Id, ct);

        if (customer is null)
        {
            return null;
        }

        customer.Person.Name = command.Name;
        customer.Person.Email = command.Email;
        customer.Person.Document = command.Document;
        customer.Person.PhoneNumber = command.PhoneNumber;

        if (command.Address != null)
        {
            var existingPersonAddress = customer.Person.PersonAddresses.FirstOrDefault();

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
                    PersonId = customer.PersonId,
                    AddressId = createdAddress.Id
                };
                await _personAddressService.InsertAsync(newPersonAddress, companyId, ct);
            }
        }

        await _personService.UpdateAsync(customer.Person.Id, customer.Person, companyId, ct);
        var updated = await _customerRepository.UpdateAsync(command.Id, customer, ct) ?? throw new ItemNotExistsException();
        return new UpdateCustomerResponse(updated.Id, customer.PersonId);
    }

    public virtual async Task DeleteAsync(DeleteCustomerCommand command, Guid companyId, CancellationToken ct)
    {
        await _customerRepository.DeleteAsync(command.Id, ct);
    }

    public virtual async Task<CustomerInsightsResponse> GetInsightsAsync(GetCustomerInsightsQuery query, CancellationToken ct)
    {
        var summary = await GetSummaryAsync(ct);
        var topCustomers = await GetTopCustomersAsync(query.TopLimit, ct);
        var recentOrders = await GetRecentOrdersAsync(query.TopLimit, ct);
        var atRiskCustomers = await GetAtRiskCustomersAsync(query, ct);

        return new CustomerInsightsResponse
        {
            Summary = summary,
            TopCustomers = topCustomers,
            RecentOrders = recentOrders,
            AtRiskCustomers = atRiskCustomers
        };
    }

    public virtual async Task<int> GetCountAsync(CancellationToken ct)
    {
        return await _customerRepository.CountAsync(ct);
    }

    private async Task<List<CustomerRiskAlertResponse>> GetAtRiskCustomersAsync(GetCustomerInsightsQuery query, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var orders = await _orderService.GetAtRiskOrdersAsync(ct);

        var response = orders.GroupBy(o => o.CustomerId).Select(g =>
        {
            var lastOrder = g.Max(o => o.SaleDate);
            var daysSince = (now - lastOrder).Days;
            var riskLevel = daysSince >= query.RiskThresholdDays * 2 ? "High" : daysSince >= query.RiskThresholdDays ? "Medium" : "Low";

            return new CustomerRiskAlertResponse(g.Key, g.First().Customer.Person.Name, g.Count(), lastOrder, daysSince, g.Sum(o => o.TotalAmount), riskLevel);
        }).Where(c => c.DaysSinceLastOrder >= query.RiskThresholdDays).OrderByDescending(c => c.DaysSinceLastOrder).ToList();

        return response;
    }

    private async Task<List<CustomerRecentOrdersResponse>> GetRecentOrdersAsync(int topLimit, CancellationToken ct)
    {
        var orders = await _orderService.GetRecentOrdersAsync(topLimit, ct);

        var response = orders.Take(topLimit).Select(o => new CustomerRecentOrdersResponse(o.Id, o.CustomerId, o.Customer.Person.Name, o.TotalAmount, o.SaleDate, o.Status.ToString(), o.Details.Sum(d => (int)d.Quantity))).ToList();

        return response;
    }

    private async Task<List<CustomerOrderHistoryResponse>> GetTopCustomersAsync(int topLimit, CancellationToken ct)
    {
        var orders = await _orderService.GetTopCustomerOrdersAsync(ct);

        var response = orders.GroupBy(o => new { o.CustomerId, CustomerName = o.Customer.Person.Name }).Select(g => new CustomerOrderHistoryResponse(g.Key.CustomerId, g.Key.CustomerName, g.Count(), g.Sum(o => o.TotalAmount), g.Sum(o => o.Details.Sum(d => (int)d.Quantity)), g.Min(o => o.SaleDate), g.Max(o => o.SaleDate), g.Any() ? g.Sum(o => o.TotalAmount) / g.Count() : 0)).OrderByDescending(e => e.TotalSpent).Take(topLimit).ToList();

        return response;
    }

    private async Task<CustomerSummaryResponse> GetSummaryAsync(CancellationToken ct)
    {
        var totalCustomers = await _customerRepository.CountAsync(ct);
        var totalOrders = await _orderService.GetTotalOrdersCountAsync(ct);
        var totalRevenue = await _orderService.GetTotalRevenueAsync(ct);
        var totalProducts = await _productService.GetTotalProductsAsync(ct);
        var totalCost = await _orderService.GetTotalCostAsync(ct);
        var grossProfit = totalRevenue - totalCost;
        var profitMargin = totalRevenue > 0 ? grossProfit / totalRevenue * 100 : 0;
        var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;
        var totalStockValue = totalProducts * 0;

        var summary = new CustomerSummaryResponse
        {
            TotalCustomers = totalCustomers,
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            AverageOrderValue = averageOrderValue
        };
        return summary;
    }
}
