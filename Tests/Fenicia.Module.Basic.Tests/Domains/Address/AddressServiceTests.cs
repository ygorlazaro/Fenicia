using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Fenicia.Module.Basic.Tests.Domains.Address;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Address;

public class AddressServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly AddressService _service;

    public AddressServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new DefaultContext(options, new Fenicia.Common.Tests.TestCompanyContext());
        _service = new AddressService(new AddressRepository(_db));
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesAddressAndReturnsResponse()
    {
        // Arrange
        var command = new AddressCommand(_faker.Address.StreetAddress(), _faker.Address.BuildingNumber(), _faker.Address.SecondaryAddress(), _faker.Address.City(), "84140955", Guid.NewGuid(), _faker.Address.City(), _faker.Address.Country());

        // Act
        var result = await _service.AddAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Street.Should().Be(command.Street);
        result.Number.Should().Be(command.Number);
        result.Complement.Should().Be(command.Complement);
        result.Neighborhood.Should().Be(command.Neighborhood);
        result.ZipCode.Should().Be(command.ZipCode);
        result.StateId.Should().Be(command.StateId);
        result.City.Should().Be(command.City);
        result.Country.Should().Be(command.Country);
    }

    [Fact]
    public async Task UpdateAsync_WhenAddressExists_UpdatesAddressAndReturnsResponse()
    {
        // Arrange
        var address = new AddressModel
        {
            Street = _faker.Address.StreetAddress(),
            Number = _faker.Address.BuildingNumber(),
            Complement = _faker.Address.SecondaryAddress(),
            Neighborhood = _faker.Address.City(),
            ZipCode = _faker.Address.ZipCode(),
            StateId = Guid.NewGuid(),
            City = _faker.Address.City(),
            Country = _faker.Address.Country()
        };
        _db.AuthAddresses.Add(address);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new AddressCommand("Updated Street", "Updated Number", "Updated Complement", "Updated Neighborhood", "12345678", Guid.NewGuid(), "Updated City", "Updated Country");

        // Act
        var result = await _service.UpdateAsync(address.Id, command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(address.Id);
        result.Street.Should().Be("Updated Street");
        result.Number.Should().Be("Updated Number");
    }

    [Fact]
    public async Task UpdateAsync_WhenAddressDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new AddressCommand("Updated Street", "Updated Number", "Updated Complement", "Updated Neighborhood", "12345678", Guid.NewGuid(), "Updated City", "Updated Country");

        // Act
        var result = await _service.UpdateAsync(Guid.NewGuid(), command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenAddressExists_ReturnsResponse()
    {
        // Arrange
        var address = new AddressModel
        {
            Street = _faker.Address.StreetAddress(),
            Number = _faker.Address.BuildingNumber(),
            Complement = _faker.Address.SecondaryAddress(),
            Neighborhood = _faker.Address.City(),
            ZipCode = _faker.Address.ZipCode(),
            StateId = Guid.NewGuid(),
            City = _faker.Address.City(),
            Country = _faker.Address.Country()
        };
        _db.AuthAddresses.Add(address);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(address.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(address.Id);
        result.Street.Should().Be(address.Street);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAddressDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
