using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Customer.DTOs;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer;

public class CustomerService(DefaultContext db)
{
    public async Task<Pagination<List<GetAllCustomerResponse>>> GetAllAsync(GetAllCustomerQuery query, CancellationToken ct)
    {
        var total = await db.BasicCustomers.CountAsync(ct);

        var customers = await db.BasicCustomers
            .Include(c => c.Person)
            .Include(c => c.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

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
        var customer = await db.BasicCustomers
            .Include(c => c.Person)
            .Include(c => c.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .FirstOrDefaultAsync(c => c.Id == query.Id, ct);

        if (customer == null)
            return null;

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

    public async Task<AddCustomerResponse> AddAsync(AddCustomerCommand command, CancellationToken ct)
    {
        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            Document = command.Document,
            PhoneNumber = command.PhoneNumber
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
            db.AuthAddresses.Add(address);
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
            db.BasicPersonAddresses.Add(personAddress);
        }

        db.BasicCustomers.Add(customer);

        await db.SaveChangesAsync(ct);

        return new AddCustomerResponse(customer.Id, person.Id);
    }

    public async Task<UpdateCustomerResponse?> UpdateAsync(UpdateCustomerCommand command, CancellationToken ct)
    {
        var customer = await db.BasicCustomers
            .Include(c => c.Person)
            .Include(c => c.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
            .FirstOrDefaultAsync(c => c.Id == command.Id, ct);

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
                db.AuthAddresses.Add(newAddress);

                var newPersonAddress = new PersonAddressModel
                {
                    Id = Guid.NewGuid(),
                    PersonId = customer.PersonId,
                    AddressId = newAddress.Id
                };
                db.BasicPersonAddresses.Add(newPersonAddress);
            }
        }

        db.BasicCustomers.Update(customer);

        await db.SaveChangesAsync(ct);

        return new UpdateCustomerResponse(customer.Id, customer.PersonId);
    }

    public async Task DeleteAsync(DeleteCustomerCommand command, CancellationToken ct)
    {
        var customer = await db.BasicCustomers.FirstOrDefaultAsync(c => c.Id == command.Id, ct);

        if (customer is null)
        {
            return;
        }

        customer.Deleted = DateTime.Now;

        db.BasicCustomers.Update(customer);

        await db.SaveChangesAsync(ct);
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

        var orders = await db.BasicOrders.Include(o => o.Customer).ThenInclude(c => c.Person).ToListAsync(ct);

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
        var orders = await db.BasicOrders.Include(o => o.Customer).ThenInclude(c => c.Person).Include(o => o.Details).OrderByDescending(o => o.SaleDate).Take(topLimit * 2).ToListAsync(ct);

        var response = orders.Take(topLimit).Select(o => new CustomerRecentOrdersResponse(o.Id, o.CustomerId, o.Customer.Person.Name, o.TotalAmount, o.SaleDate, o.Status.ToString(), o.Details.Sum(d => (int)d.Quantity))).ToList();

        return response;
    }

    private async Task<List<CustomerOrderHistoryResponse>> GetTopCustomersAsync(int topLimit, CancellationToken ct)
    {
        var orders = await db.BasicOrders.Include(o => o.Customer).ThenInclude(c => c.Person).Include(o => o.Details).ToListAsync(ct);

        var response = orders.GroupBy(o => new { o.CustomerId, CustomerName = o.Customer.Person.Name }).Select(g => new CustomerOrderHistoryResponse(g.Key.CustomerId, g.Key.CustomerName, g.Count(), g.Sum(o => o.TotalAmount), g.Sum(o => o.Details.Sum(d => (int)d.Quantity)), g.Min(o => o.SaleDate), g.Max(o => o.SaleDate), g.Any() ? g.Sum(o => o.TotalAmount) / g.Count() : 0)).OrderByDescending(e => e.TotalSpent).Take(topLimit).ToList();

        return response;
    }

    private async Task<CustomerSummaryResponse> GetSummaryAsync(CancellationToken ct)
    {
        var totalCustomers = await db.BasicCustomers.CountAsync(ct);
        var totalOrders = await db.BasicOrders.CountAsync(ct);
        var totalRevenue = await db.BasicOrders.SumAsync(o => o.TotalAmount, ct);
        var averageOrderValue = totalRevenue / (totalOrders > 0 ? totalOrders : 1);

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
