using AwesomeAssertions;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.PersonAddress;

public class PersonAddressRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly PersonAddressRepository _repository;

    public PersonAddressRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new PersonAddressRepository(_db);
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPersonAddresses()
    {
        // Arrange
        var personAddress = new PersonAddressModel
            { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };
        _db.BasicPersonAddresses.Add(personAddress);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetAllAsync(cancellationToken: CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPersonAddressExists_ReturnsPersonAddress()
    {
        // Arrange
        var personAddress = new PersonAddressModel
            { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };
        _db.BasicPersonAddresses.Add(personAddress);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByIdAsync(personAddress.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(personAddress.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPersonAddressDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task InsertAsync_WhenPersonAddressIsValid_InsertsPersonAddress()
    {
        // Arrange
        var personAddress = new PersonAddressModel
            { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };

        // Act
        var result = await _repository.InsertAsync(personAddress, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Created.Should().NotBe(default);
    }

    [Fact]
    public async Task UpdateAsync_WhenPersonAddressExists_UpdatesPersonAddress()
    {
        // Arrange
        var personAddress = new PersonAddressModel
            { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };
        _db.BasicPersonAddresses.Add(personAddress);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.UpdateAsync(personAddress.Id, personAddress, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(personAddress.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenPersonAddressDoesNotExist_ReturnsNull()
    {
        // Arrange
        var personAddress = new PersonAddressModel
            { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };

        // Act
        var result = await _repository.UpdateAsync(personAddress.Id, personAddress, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenPersonAddressExists_SoftDeletesPersonAddress()
    {
        // Arrange
        var personAddress = new PersonAddressModel
            { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };
        _db.BasicPersonAddresses.Add(personAddress);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.DeleteAsync(personAddress.Id, CancellationToken.None);

        // Assert
        result.Should().Be(1);
        var deletedPersonAddress = await _db.BasicPersonAddresses.IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == personAddress.Id);
        deletedPersonAddress.Should().NotBeNull();
        deletedPersonAddress.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        // Arrange
        var personAddress = new PersonAddressModel
            { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };
        _db.BasicPersonAddresses.Add(personAddress);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.CountAsync(CancellationToken.None);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingPersonAddresses()
    {
        // Arrange
        var personAddress = new PersonAddressModel
            { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };
        _db.BasicPersonAddresses.Add(personAddress);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.FindAsync(p => p.Id == personAddress.Id, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task AnyAsync_WhenPersonAddressExists_ReturnsTrue()
    {
        // Arrange
        var personAddress = new PersonAddressModel
            { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };
        _db.BasicPersonAddresses.Add(personAddress);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.AnyAsync(p => p.Id == personAddress.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        // Arrange
        var personAddress = new PersonAddressModel
            { Id = Guid.NewGuid(), PersonId = Guid.NewGuid(), AddressId = Guid.NewGuid() };
        _db.BasicPersonAddresses.Add(personAddress);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.Query().Where(p => p.Id == personAddress.Id).ToListAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }
}