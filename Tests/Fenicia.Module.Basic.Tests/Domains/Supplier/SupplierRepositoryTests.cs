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
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllAsync(ct: CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSupplierExists_ReturnsSupplier()
    {
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(supplier.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(supplier.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSupplierDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task InsertAsync_WhenSupplierIsValid_InsertsSupplier()
    {
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };

        var result = await _repository.InsertAsync(supplier, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.Created);
    }

    [Fact]
    public async Task UpdateAsync_WhenSupplierExists_UpdatesSupplier()
    {
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.UpdateAsync(supplier.Id, supplier, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(supplier.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenSupplierDoesNotExist_ReturnsNull()
    {
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };

        var result = await _repository.UpdateAsync(supplier.Id, supplier, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenSupplierExists_SoftDeletesSupplier()
    {
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.DeleteAsync(supplier.Id, CancellationToken.None);

        Assert.Equal(1, result);
        var deletedSupplier = await _db.BasicSuppliers.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == supplier.Id);
        Assert.NotNull(deletedSupplier);
        Assert.NotNull(deletedSupplier.Deleted);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountAsync(CancellationToken.None);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingSuppliers()
    {
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.FindAsync(s => s.Id == supplier.Id, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task AnyAsync_WhenSupplierExists_ReturnsTrue()
    {
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyAsync(s => s.Id == supplier.Id, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.Query().Where(s => s.Id == supplier.Id).ToListAsync(CancellationToken.None);

        Assert.Single(result);
    }
}
