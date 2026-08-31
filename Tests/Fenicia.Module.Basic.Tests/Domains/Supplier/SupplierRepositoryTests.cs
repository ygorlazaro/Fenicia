using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Supplier;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class SupplierRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly SupplierRepository _repository;

    public SupplierRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new SupplierRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllSuppliers()
    {
        // Arrange
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetAllAsync(cancellationToken: CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSupplierExists_ReturnsSupplier()
    {
        // Arrange
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByIdAsync(supplier.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(supplier.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSupplierDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task InsertAsync_WhenSupplierIsValid_InsertsSupplier()
    {
        // Arrange
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };

        // Act
        var result = await _repository.InsertAsync(supplier, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Created.Should().NotBe(default(DateTime));
    }

    [Fact]
    public async Task UpdateAsync_WhenSupplierExists_UpdatesSupplier()
    {
        // Arrange
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.UpdateAsync(supplier.Id, supplier, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(supplier.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenSupplierDoesNotExist_ReturnsNull()
    {
        // Arrange
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };

        // Act
        var result = await _repository.UpdateAsync(supplier.Id, supplier, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenSupplierExists_SoftDeletesSupplier()
    {
        // Arrange
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.DeleteAsync(supplier.Id, CancellationToken.None);

        // Assert
        result.Should().Be(1);
        var deletedSupplier = await _db.BasicSuppliers.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == supplier.Id);
        deletedSupplier.Should().NotBeNull();
        deletedSupplier!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        // Arrange
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.CountAsync(CancellationToken.None);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingSuppliers()
    {
        // Arrange
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.FindAsync(s => s.Id == supplier.Id, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task AnyAsync_WhenSupplierExists_ReturnsTrue()
    {
        // Arrange
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.AnyAsync(s => s.Id == supplier.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        // Arrange
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.Query().Where(s => s.Id == supplier.Id).ToListAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }
}
