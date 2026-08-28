using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Fenicia.Module.Basic.Domains.Dashboard;

namespace Fenicia.Module.Basic.Domains.Customer;

public class CustomerService(
    CustomerRepository customerRepository,
    PersonRepository personRepository,
    AddressRepository addressRepository,
    PersonAddressRepository personAddressRepository,
    DashboardRepository dashboardRepository)
{
    public async Task<Pagination<List<GetAllCustomerResponse>>> GetAllAsync(GetAllCustomerQuery query, CancellationToken ct)
    {
        var total = await customerRepository.CountAsync(ct);

        var customers = await customerRepository.GetAllWithDetailsAsync(query.Page, query.PerPage, ct);

        var response = customers.Select(c =>
    {
        var personAddress = c.Person.PersonAddresses.FirstOrDefault();
        var address = personAddress?.Address;

            return new GetAllCustomerResponse(
                c.Id,
                c.PersonId,
                c.Person.Name,
                c.Person.Email,
                c.Person.PhoneNumber,
                c.Person.Document,
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

        return new Pagination<List<GetAllCustomerResponse>>(response, total, query.Page, query.PerPage);
    }

    public async Task<GetCustomerByIdResponse?> GetByIdAsync(GetCustomerByIdQuery query, CancellationToken ct)
    {
        var customer = await customerRepository.GetByIdWithDetailsAsync(query.Id, ct);

        if (customer == null)
{
    return null;
}

        var personAddress = customer.Person.PersonAddresses.FirstOrDefault();
        var address = personAddress?.Address;

        return new GetCustomerByIdResponse(
            customer.Id,
            customer.PersonId,
            customer.Person.Name,
            customer.Person.Email,
            customer.Person.PhoneNumber,
            customer.Person.Document,
            address != null ? new AddressResponse(
                address.Id,
                address.Street,
                address.Number,
                address.Complement,
                address.Neighborhood,
                address.ZipCode,
                address.StateId,
                address.State.Name,
                address.City,
                address.Country
            ) : null
        );
    }

    public async Task<AddCustomerResponse> AddAsync(AddCustomerCommand command, Guid companyId, CancellationToken ct)
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

        var customer = new CustomerModel
        {
            Person = person,
            PersonId = person.Id
        };

        if (address != null)
        {
            var personAddress = new PersonAddressModel
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                AddressId = address.Id
            };
            await personAddressRepository.InsertAsync(personAddress, ct);
        }

        await personRepository.InsertAsync(person, ct);
        var created = await customerRepository.InsertAsync(customer, ct);

        return new AddCustomerResponse(created.Id, person.Id);
    }

    public async Task<UpdateCustomerResponse?> UpdateAsync(UpdateCustomerCommand command, Guid companyId, CancellationToken ct)
    {
        var customer = await customerRepository.GetByIdWithDetailsAsync(command.Id, ct);

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
                    PersonId = customer.PersonId,
                    AddressId = newAddress.Id
                };
                await personAddressRepository.InsertAsync(newPersonAddress, ct);
            }
        }

        await personRepository.UpdateAsync(customer.Person.Id, customer.Person, ct);
        var updated = await customerRepository.UpdateAsync(command.Id, customer, ct);

        return new UpdateCustomerResponse(updated.Id, customer.PersonId);
    }

    public async Task DeleteAsync(DeleteCustomerCommand command, CancellationToken ct)
    {
        var customer = await customerRepository.GetByIdAsync(command.Id, ct);

        if (customer is null)
        {
            return;
        }

        customer.Deleted = DateTime.Now;

        await customerRepository.UpdateAsync(command.Id, customer, ct);
    }

    public async Task<CustomerInsightsResponse> GetInsightsAsync(GetCustomerInsightsQuery query, CancellationToken ct)
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

    private async Task<List<CustomerRiskAlertResponse>> GetAtRiskCustomersAsync(GetCustomerInsightsQuery query, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var orders = await dashboardRepository.GetAtRiskOrdersAsync(ct);

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
        var orders = await dashboardRepository.GetRecentOrdersAsync(topLimit, ct);

        var response = orders.Take(topLimit).Select(o => new CustomerRecentOrdersResponse(o.Id, o.CustomerId, o.Customer.Person.Name, o.TotalAmount, o.SaleDate, o.Status.ToString(), o.Details.Sum(d => (int)d.Quantity))).ToList();

        return response;
    }

    private async Task<List<CustomerOrderHistoryResponse>> GetTopCustomersAsync(int topLimit, CancellationToken ct)
    {
        var orders = await dashboardRepository.GetTopCustomerOrdersAsync(ct);

        var response = orders.GroupBy(o => new { o.CustomerId, CustomerName = o.Customer.Person.Name }).Select(g => new CustomerOrderHistoryResponse(g.Key.CustomerId, g.Key.CustomerName, g.Count(), g.Sum(o => o.TotalAmount), g.Sum(o => o.Details.Sum(d => (int)d.Quantity)), g.Min(o => o.SaleDate), g.Max(o => o.SaleDate), g.Any() ? g.Sum(o => o.TotalAmount) / g.Count() : 0)).OrderByDescending(e => e.TotalSpent).Take(topLimit).ToList();

        return response;
    }

    private async Task<CustomerSummaryResponse> GetSummaryAsync(CancellationToken ct)
    {
        var totalCustomers = await customerRepository.CountAsync(ct);
        var totalOrders = await dashboardRepository.GetTotalOrdersAsync(ct);
        var totalRevenue = await dashboardRepository.GetTotalRevenueAsync(ct);
        var totalProducts = await dashboardRepository.GetTotalProductsAsync(ct);
        var totalCost = await dashboardRepository.GetTotalCostAsync(ct);
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
