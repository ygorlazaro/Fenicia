using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Address;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Address;

public class AddressRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly AddressRepository _repository;

    public AddressRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new AddressRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllAddresses()
    {
        var address = new AddressModel { Id = Guid.NewGuid(), Street = _faker.Address.StreetName(), Number = _faker.Address.BuildingNumber(), ZipCode = "12345678", StateId = Guid.NewGuid(), City = _faker.Address.City() };
        _db.AuthAddresses.Add(address);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllAsync(ct: CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAddressExists_ReturnsAddress()
    {
        var address = new AddressModel { Id = Guid.NewGuid(), Street = _faker.Address.StreetName(), Number = _faker.Address.BuildingNumber(), ZipCode = "12345678", StateId = Guid.NewGuid(), City = _faker.Address.City() };
        _db.AuthAddresses.Add(address);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(address.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(address.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAddressDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task InsertAsync_WhenAddressIsValid_InsertsAddress()
    {
        var address = new AddressModel { Id = Guid.NewGuid(), Street = _faker.Address.StreetName(), Number = _faker.Address.BuildingNumber(), ZipCode = "12345678", StateId = Guid.NewGuid(), City = _faker.Address.City() };

        var result = await _repository.InsertAsync(address, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.Created);
    }

    [Fact]
    public async Task UpdateAsync_WhenAddressExists_UpdatesAddress()
    {
        var address = new AddressModel { Id = Guid.NewGuid(), Street = _faker.Address.StreetName(), Number = _faker.Address.BuildingNumber(), ZipCode = "12345678", StateId = Guid.NewGuid(), City = _faker.Address.City() };
        _db.AuthAddresses.Add(address);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.UpdateAsync(address.Id, address, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(address.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenAddressDoesNotExist_ReturnsNull()
    {
        var address = new AddressModel { Id = Guid.NewGuid(), Street = _faker.Address.StreetName(), Number = _faker.Address.BuildingNumber(), ZipCode = "12345678", StateId = Guid.NewGuid(), City = _faker.Address.City() };

        var result = await _repository.UpdateAsync(address.Id, address, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenAddressExists_SoftDeletesAddress()
    {
        var address = new AddressModel { Id = Guid.NewGuid(), Street = _faker.Address.StreetName(), Number = _faker.Address.BuildingNumber(), ZipCode = "12345678", StateId = Guid.NewGuid(), City = _faker.Address.City() };
        _db.AuthAddresses.Add(address);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.DeleteAsync(address.Id, CancellationToken.None);

        Assert.Equal(1, result);
        var deletedAddress = await _db.AuthAddresses.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Id == address.Id);
        Assert.NotNull(deletedAddress);
        Assert.NotNull(deletedAddress.Deleted);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        var address = new AddressModel { Id = Guid.NewGuid(), Street = _faker.Address.StreetName(), Number = _faker.Address.BuildingNumber(), ZipCode = "12345678", StateId = Guid.NewGuid(), City = _faker.Address.City() };
        _db.AuthAddresses.Add(address);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountAsync(CancellationToken.None);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingAddresses()
    {
        var address = new AddressModel { Id = Guid.NewGuid(), Street = _faker.Address.StreetName(), Number = _faker.Address.BuildingNumber(), ZipCode = "12345678", StateId = Guid.NewGuid(), City = _faker.Address.City() };
        _db.AuthAddresses.Add(address);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.FindAsync(a => a.Id == address.Id, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task AnyAsync_WhenAddressExists_ReturnsTrue()
    {
        var address = new AddressModel { Id = Guid.NewGuid(), Street = _faker.Address.StreetName(), Number = _faker.Address.BuildingNumber(), ZipCode = "12345678", StateId = Guid.NewGuid(), City = _faker.Address.City() };
        _db.AuthAddresses.Add(address);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyAsync(a => a.Id == address.Id, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        var address = new AddressModel { Id = Guid.NewGuid(), Street = _faker.Address.StreetName(), Number = _faker.Address.BuildingNumber(), ZipCode = "12345678", StateId = Guid.NewGuid(), City = _faker.Address.City() };
        _db.AuthAddresses.Add(address);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.Query().Where(a => a.Id == address.Id).ToListAsync(CancellationToken.None);

        Assert.Single(result);
    }
}
