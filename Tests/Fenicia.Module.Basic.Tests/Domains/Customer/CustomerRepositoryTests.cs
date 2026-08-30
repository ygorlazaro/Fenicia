using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class CustomerRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new CustomerRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCustomers()
    {
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllAsync(ct: CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerExists_ReturnsCustomer()
    {
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(customer.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(customer.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task InsertAsync_WhenCustomerIsValid_InsertsCustomer()
    {
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };

        var result = await _repository.InsertAsync(customer, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.Created);
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerExists_UpdatesCustomer()
    {
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.UpdateAsync(customer.Id, customer, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(customer.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };

        var result = await _repository.UpdateAsync(customer.Id, customer, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerExists_SoftDeletesCustomer()
    {
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.DeleteAsync(customer.Id, CancellationToken.None);

        Assert.Equal(1, result);
        var deletedCustomer = await _db.BasicCustomers.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == customer.Id);
        Assert.NotNull(deletedCustomer);
        Assert.NotNull(deletedCustomer.Deleted);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountAsync(CancellationToken.None);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingCustomers()
    {
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.FindAsync(c => c.Id == customer.Id, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task AnyAsync_WhenCustomerExists_ReturnsTrue()
    {
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyAsync(c => c.Id == customer.Id, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.Query().Where(c => c.Id == customer.Id).ToListAsync(CancellationToken.None);

        Assert.Single(result);
    }
}
