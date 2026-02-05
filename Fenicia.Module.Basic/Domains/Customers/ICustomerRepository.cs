using Fenicia.Common.Database.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Customers;

public interface ICustomerRepository
{
    Task<CustomerModel?> GetByIdAsync(Guid id);

    Task<List<CustomerModel>> GetAllAsync();

    void Add(CustomerModel customer);

    void Update(CustomerModel customer);

    void Delete(Guid id);

    Task<int> SaveChangesAsync();
}
