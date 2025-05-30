using Fenicia.Auth.Contexts;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Repositories;

public class CustomerRepository(AuthContext authContext) : ICustomerRepository
{
    public async Task<CustomerModel?> GetByUserIdAsync(Guid userId, Guid companyId)
    {
        var query = from customer in authContext.Customers
                    where customer.UserId == userId && companyId == customer.CompanyId
                    select customer;

        return await query.FirstOrDefaultAsync();
    }

    public async Task<Guid?> CreateNewCustomerAsync(Guid userId, Guid companyId)
    {
        var customer = new CustomerModel
        {
            UserId = userId,
            CompanyId = companyId
        };

        authContext.Customers.Add(customer);

        await authContext.SaveChangesAsync();

        return customer.Id;
    }
}