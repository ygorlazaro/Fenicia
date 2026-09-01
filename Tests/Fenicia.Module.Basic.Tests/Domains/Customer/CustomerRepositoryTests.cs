using AwesomeAssertions;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class CustomerRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new CustomerRepository(_db);
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCustomers()
    {
        // Arrange
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetAllAsync(cancellationToken: CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerExists_ReturnsCustomer()
    {
        // Arrange
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByIdAsync(customer.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(customer.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task InsertAsync_WhenCustomerIsValid_InsertsCustomer()
    {
        // Arrange
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };

        // Act
        var result = await _repository.InsertAsync(customer, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Created.Should().NotBe(default(DateTime));
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerExists_UpdatesCustomer()
    {
        // Arrange
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.UpdateAsync(customer.Id, customer, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(customer.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        // Arrange
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };

        // Act
        var result = await _repository.UpdateAsync(customer.Id, customer, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerExists_SoftDeletesCustomer()
    {
        // Arrange
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.DeleteAsync(customer.Id, CancellationToken.None);

        // Assert
        result.Should().Be(1);
        var deletedCustomer = await _db.BasicCustomers.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == customer.Id);
        deletedCustomer.Should().NotBeNull();
        deletedCustomer.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        // Arrange
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.CountAsync(CancellationToken.None);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingCustomers()
    {
        // Arrange
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.FindAsync(c => c.Id == customer.Id, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task AnyAsync_WhenCustomerExists_ReturnsTrue()
    {
        // Arrange
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.AnyAsync(c => c.Id == customer.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        // Arrange
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.Query().Where(c => c.Id == customer.Id).ToListAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }
}
