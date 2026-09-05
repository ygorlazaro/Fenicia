using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Exceptions;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Fenicia.Module.Basic.Domains.Address.Interfaces;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Fenicia.Module.Basic.Domains.Customer.Interfaces;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.Order.Interfaces;
using Fenicia.Module.Basic.Domains.Person.Interfaces;
using Fenicia.Module.Basic.Domains.PersonAddress.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer;

public sealed class CustomerService(
    ICustomerRepository customerRepository,
    IPersonService personService,
    IAddressService addressService,
    IPersonAddressService personAddressService,
    IOrderService orderService) : ICustomerService
{
    public CustomerService()
        : this(null!, null!, null!, null!, null!)
    {
    }

    public async Task<Pagination<List<GetAllCustomerResponse>>> GetAllAsync(
        GetAllCustomerQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = customerRepository.Query()
            .Include(c => c.Person)
            .Include(c => c.Person.PersonAddresses)
            .ThenInclude(pa => pa.Address)
            .ThenInclude(a => a.State);

        var filteredQuery = baseQuery;

        var total = await filteredQuery.CountAsync(cancellationToken);

        var customers = await filteredQuery
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);

        var response = customers.Select(c => c.MapToGetAllCustomerResponse()).ToList();

        return new Pagination<List<GetAllCustomerResponse>>(response, total, query.Page, query.PerPage);
    }

    public async Task<List<GetAllCustomerForDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default)
    {
        var customers = await customerRepository.GetAllWithDetailsAsync(cancellationToken: cancellationToken);

        return [.. customers.Select(c => new GetAllCustomerForDataSourceResponse(c.Id, c.Person.Name))];
    }

    public async Task<GetCustomerByIdResponse?> GetByIdAsync(
        GetCustomerByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var customer = await customerRepository.GetByIdWithDetailsAsync(query.Id, cancellationToken);

        return customer?.MapToGetCustomerByIdResponse();
    }

    public async Task<AddCustomerResponse> AddAsync(
        AddCustomerCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            Document = command.Document,
            PhoneNumber = command.PhoneNumber
        };

        Guid? addressId = null;

        if (command.Address != null)
        {
            var addressCommand = new AddressCommand(
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

        await personService.InsertAsync(person, companyId, cancellationToken);

        var customer = new CustomerModel
        {
            Person = person,
            PersonId = person.Id
        };

        await customerRepository.InsertAsync(customer, cancellationToken);

        if (addressId.HasValue)
        {
            var personAddress = new PersonAddressModel
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                AddressId = addressId.Value
            };
            await personAddressService.InsertAsync(personAddress, companyId, cancellationToken);
        }

        return new AddCustomerResponse(customer.Id, person.Id);
    }

    public async Task<UpdateCustomerResponse?> UpdateAsync(
        UpdateCustomerCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var customer = await customerRepository.GetByIdWithDetailsAsync(command.Id, cancellationToken);

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
                var addressCommand = new AddressCommand(
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
                var addressCommand = new AddressCommand(
                    command.Address.Street,
                    command.Address.Number,
                    command.Address.Complement,
                    command.Address.Neighborhood,
                    command.Address.ZipCode,
                    command.Address.StateId,
                    command.Address.City,
                    command.Address.Country);
                var createdAddress = await addressService.AddAsync(addressCommand, cancellationToken);

                var newPersonAddress = new PersonAddressModel
                {
                    Id = Guid.NewGuid(),
                    PersonId = customer.PersonId,
                    AddressId = createdAddress.Id
                };
                await personAddressService.InsertAsync(newPersonAddress, companyId, cancellationToken);
            }
        }

        await personService.UpdateAsync(customer.Person.Id, customer.Person, companyId, cancellationToken);
        var updated = await customerRepository.UpdateAsync(command.Id, customer, cancellationToken) ??
                      throw new ItemNotExistsException();
        return new UpdateCustomerResponse(updated.Id, customer.PersonId);
    }

    public async Task DeleteAsync(
        DeleteCustomerCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        await customerRepository.DeleteAsync(command.Id, cancellationToken);
    }

    public async Task<CustomerInsightsResponse> GetInsightsAsync(
        GetCustomerInsightsQuery query,
        CancellationToken cancellationToken = default)
    {
        var summary = await GetSummaryAsync(cancellationToken);
        var topCustomers = await GetTopCustomersAsync(query.TopLimit, cancellationToken);
        var recentOrders = await GetRecentOrdersAsync(query.TopLimit, cancellationToken);
        var atRiskCustomers = await GetAtRiskCustomersAsync(query, cancellationToken);

        return new CustomerInsightsResponse
        {
            Summary = summary,
            TopCustomers = topCustomers,
            RecentOrders = recentOrders,
            AtRiskCustomers = atRiskCustomers
        };
    }

    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return customerRepository.CountAsync(cancellationToken);
    }

    private async Task<List<CustomerRiskAlertResponse>> GetAtRiskCustomersAsync(
        GetCustomerInsightsQuery query,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var orders = await orderService.GetAtRiskOrdersAsync(cancellationToken);

        var response = orders.GroupBy(o => o.CustomerId).Select(g =>
            {
                var lastOrder = g.Max(o => o.SaleDate);
                var daysSince = (now - lastOrder).Days;
                var riskLevel = daysSince >= query.RiskThresholdDays * 2 ? "High" :
                    daysSince >= query.RiskThresholdDays ? "Medium" : "Low";

                return new CustomerRiskAlertResponse(
                    g.Key,
                    g.First().Customer.Person.Name,
                    g.Count(),
                    lastOrder,
                    daysSince,
                    g.Sum(o => o.TotalAmount),
                    riskLevel);
            }).Where(c => c.DaysSinceLastOrder >= query.RiskThresholdDays).OrderByDescending(c => c.DaysSinceLastOrder)
            .ToList();

        return response;
    }

    private async Task<List<CustomerRecentOrdersResponse>> GetRecentOrdersAsync(
        int topLimit,
        CancellationToken cancellationToken = default)
    {
        var orders = await orderService.GetRecentOrdersAsync(topLimit, cancellationToken);

        var response = orders.Take(topLimit).Select(o => new CustomerRecentOrdersResponse(
            o.Id,
            o.CustomerId,
            o.Customer.Person.Name,
            o.TotalAmount,
            o.SaleDate,
            o.Status.ToString(),
            o.Details.Sum(d => (int)d.Quantity))).ToList();

        return response;
    }

    private async Task<List<CustomerOrderHistoryResponse>> GetTopCustomersAsync(
        int topLimit,
        CancellationToken cancellationToken = default)
    {
        var orders = await orderService.GetTopCustomerOrdersAsync(cancellationToken);

        var response = orders.GroupBy(o => new { o.CustomerId, CustomerName = o.Customer.Person.Name }).Select(g =>
                new CustomerOrderHistoryResponse(
                    g.Key.CustomerId,
                    g.Key.CustomerName,
                    g.Count(),
                    g.Sum(o => o.TotalAmount),
                    g.Sum(o => o.Details.Sum(d => (int)d.Quantity)),
                    g.Min(o => o.SaleDate),
                    g.Max(o => o.SaleDate),
                    g.Any() ? g.Sum(o => o.TotalAmount) / g.Count() : 0)).OrderByDescending(e => e.TotalSpent)
            .Take(topLimit).ToList();

        return response;
    }

    private async Task<CustomerSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var totalCustomers = await customerRepository.CountAsync(cancellationToken);
        var totalOrders = await orderService.GetTotalOrdersCountAsync(cancellationToken);
        var totalRevenue = await orderService.GetTotalRevenueAsync(cancellationToken);
        var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

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