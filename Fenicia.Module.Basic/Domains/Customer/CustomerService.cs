using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Address;
using Fenicia.Common.DTOs.Basic.Customer;
using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Common.DTOs.Basic.Person;
using Fenicia.Common.DTOs.Basic.PersonAddress;
using Fenicia.Common.Exceptions;
using Fenicia.Module.Basic.Domains.Address.Interfaces;
using Fenicia.Module.Basic.Domains.Customer.Interfaces;
using Fenicia.Module.Basic.Domains.Order.Interfaces;
using Fenicia.Module.Basic.Domains.Person.Interfaces;
using Fenicia.Module.Basic.Domains.PersonAddress.Interfaces;

namespace Fenicia.Module.Basic.Domains.Customer;

public sealed class CustomerService(
    ICustomerRepository repository,
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
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        var customers = await repository.GetAllWithDetailsAsync(page, perPage, cancellationToken);
        var total = await repository.CountAsync(cancellationToken);

        var response = customers.Select(c =>
        {
            var address = c.Person.PersonAddresses.FirstOrDefault()?.Address;
            return new GetAllCustomerResponse(
                c.Id,
                c.PersonId,
                c.Person.Name,
                c.Person.Email,
                c.Person.PhoneNumber,
                c.Person.Document,
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

        return new Pagination<List<GetAllCustomerResponse>>(response, total, page, perPage);
    }

    public async Task<List<GetAllCustomerForDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default)
    {
        var customers = await repository.GetAllWithDetailsAsync(cancellationToken: cancellationToken);

        return [.. customers.Select(c => new GetAllCustomerForDataSourceResponse(c.Id, c.Person.Name))];
    }

    public async Task<GetCustomerByIdResponse?> GetByIdAsync(
        GetCustomerByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdWithDetailsAsync(query.Id, cancellationToken);

        if (customer is null)
        {
            return null;
        }

        var address = customer.Person.PersonAddresses.FirstOrDefault()?.Address;
        return new GetCustomerByIdResponse(
            customer.Id,
            customer.PersonId,
            customer.Person.Name,
            customer.Person.Email,
            customer.Person.PhoneNumber,
            customer.Person.Document,
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

    public async Task<AddCustomerResponse> AddAsync(
        AddCustomerRequest command,
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

        var customer = new CustomerModel
        {
            PersonId = personResponse.Id
        };

        await repository.InsertAsync(customer, cancellationToken);

        if (!addressId.HasValue)
        {
            return new AddCustomerResponse(customer.Id, personResponse.Id);
        }

        var personAddressCommand = new AddPersonAddressRequest(personResponse.Id, addressId.Value);
        await personAddressService.InsertAsync(personAddressCommand, companyId, cancellationToken);

        return new AddCustomerResponse(customer.Id, personResponse.Id);
    }

    public async Task<UpdateCustomerResponse?> UpdateAsync(
        UpdateCustomerRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdWithDetailsAsync(command.Id, cancellationToken);

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
                    PersonId = customer.PersonId,
                    AddressId = createdAddress.Id
                };
                var personAddressCommand = new AddPersonAddressRequest(newPersonAddress.PersonId, newPersonAddress.AddressId);
                await personAddressService.InsertAsync(personAddressCommand, companyId, cancellationToken);
            }
        }

        var personCommand = new UpsertPersonRequest(
            customer.Person.Name,
            customer.Person.Document,
            customer.Person.Email,
            customer.Person.PhoneNumber,
            null,
            null,
            null);

        await personService.UpdateAsync(customer.Person.Id, personCommand, companyId, cancellationToken);
        var updated = await repository.UpdateAsync(command.Id, customer, cancellationToken) ??
                      throw new ForbiddenException();
        return new UpdateCustomerResponse(updated.Id, customer.PersonId);
    }

    public async Task DeleteAsync(
        DeleteCustomerRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
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
        return repository.CountAsync(cancellationToken);
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
        var totalCustomers = await repository.CountAsync(cancellationToken);
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
