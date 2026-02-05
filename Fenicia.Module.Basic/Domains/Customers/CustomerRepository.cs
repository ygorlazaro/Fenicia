using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customers;

public class CustomerRepository(BasicContext context) : ICustomerRepository
{
    public async Task<CustomerModel?> GetByIdAsync(Guid id)
    {
        return await context.Customers.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<CustomerModel>> GetAllAsync()
    {
        return await context.Customers.ToListAsync();
    }

    public void Add(CustomerModel customer)
    {
        context.Customers.Add(customer);
    }

    public void Update(CustomerModel customer)
    {
        context.Customers.Update(customer);
    }

    public void Delete(Guid id)
    {
        context.Customers.Remove(new CustomerModel { Id = id });
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}
