using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.PersonAddress;

public class PersonAddressRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly PersonAddressRepository _repository;

    public PersonAddressRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new PersonAddressRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPersonAddresses()
    {
        var personAddress = new PersonAddressModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };
        _db.BasicPersonAddresses.Add(personAddress);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllAsync(ct: CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPersonAddressExists_ReturnsPersonAddress()
    {
        var personAddress = new PersonAddressModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };
        _db.BasicPersonAddresses.Add(personAddress);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(personAddress.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(personAddress.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPersonAddressDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task InsertAsync_WhenPersonAddressIsValid_InsertsPersonAddress()
    {
        var personAddress = new PersonAddressModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };

        var result = await _repository.InsertAsync(personAddress, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.Created);
    }

    [Fact]
    public async Task UpdateAsync_WhenPersonAddressExists_UpdatesPersonAddress()
    {
        var personAddress = new PersonAddressModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };
        _db.BasicPersonAddresses.Add(personAddress);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.UpdateAsync(personAddress.Id, personAddress, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(personAddress.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenPersonAddressDoesNotExist_ReturnsNull()
    {
        var personAddress = new PersonAddressModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };

        var result = await _repository.UpdateAsync(personAddress.Id, personAddress, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenPersonAddressExists_SoftDeletesPersonAddress()
    {
        var personAddress = new PersonAddressModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };
        _db.BasicPersonAddresses.Add(personAddress);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.DeleteAsync(personAddress.Id, CancellationToken.None);

        Assert.Equal(1, result);
        var deletedPersonAddress = await _db.BasicPersonAddresses.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == personAddress.Id);
        Assert.NotNull(deletedPersonAddress);
        Assert.NotNull(deletedPersonAddress.Deleted);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        var personAddress = new PersonAddressModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };
        _db.BasicPersonAddresses.Add(personAddress);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountAsync(CancellationToken.None);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingPersonAddresses()
    {
        var personAddress = new PersonAddressModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };
        _db.BasicPersonAddresses.Add(personAddress);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.FindAsync(p => p.Id == personAddress.Id, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task AnyAsync_WhenPersonAddressExists_ReturnsTrue()
    {
        var personAddress = new PersonAddressModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };
        _db.BasicPersonAddresses.Add(personAddress);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyAsync(p => p.Id == personAddress.Id, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        var personAddress = new PersonAddressModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };
        _db.BasicPersonAddresses.Add(personAddress);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.Query().Where(p => p.Id == personAddress.Id).ToListAsync(CancellationToken.None);

        Assert.Single(result);
    }
}
