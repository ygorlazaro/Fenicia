using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Fenicia.Module.Basic.Domains.Address.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Address;

public class AddressServiceTests : IDisposable
{
    private readonly DbContextOptions<DefaultContext> _dbOptions;
    private readonly Faker _faker;
    private readonly Mock<ICompanyContext> _mockCompanyContext;

    public AddressServiceTests()
    {
        _dbOptions = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _faker = new Faker();
        _mockCompanyContext = new Mock<ICompanyContext>();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesAddressAndReturnsResponse()
    {
        // Arrange
        var command = new AddressCommand(
            _faker.Address.StreetAddress(),
            _faker.Address.BuildingNumber(),
            _faker.Address.SecondaryAddress(),
            _faker.Address.City(),
            "84140955",
            Guid.NewGuid(),
            _faker.Address.City(),
            _faker.Address.Country());
        var state = new StateModel { Id = command.StateId, Name = "State", Uf = "ST" };

        var db = NewDb();
        db.AuthStates.Add(state);
        await db.SaveChangesAsync(CancellationToken.None);

        var service = CreateService(db);

        // Act
        var result = await service.AddAsync(command, CancellationToken.None);

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
        var stateId = Guid.NewGuid();
        var state = new StateModel { Id = stateId, Name = "State", Uf = "ST" };
        var address = new AddressModel
        {
            Id = Guid.NewGuid(),
            Street = "Old Street",
            Number = "Old Number",
            Complement = "Old Complement",
            Neighborhood = "Old Neighborhood",
            ZipCode = "00000000",
            StateId = stateId,
            City = "Old City",
            Country = "Old Country"
        };

        var db = NewDb();
        db.AuthStates.Add(state);
        db.AuthAddresses.Add(address);
        await db.SaveChangesAsync(CancellationToken.None);

        var mockRepo = new Mock<IAddressRepository>();
        mockRepo.Setup(r => r.Query()).Returns(() => db.AuthAddresses);
        mockRepo.Setup(r => r.UpdateAsync(address.Id, It.IsAny<AddressModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, AddressModel a, CancellationToken _) =>
            {
                var dbAddress = db.AuthAddresses.First(x => x.Id == id);
                db.Entry(dbAddress).CurrentValues.SetValues(a);
                dbAddress.State = state;
                db.SaveChanges();
                return dbAddress;
            });

        var service = new AddressService(mockRepo.Object);

        var command = new AddressCommand(
            "Updated Street",
            "Updated Number",
            "Updated Complement",
            "Updated Neighborhood",
            "12345678",
            Guid.NewGuid(),
            "Updated City",
            "Updated Country");

        // Act
        var result = await service.UpdateAsync(address.Id, command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(address.Id);
        result.Street.Should().Be("Updated Street");
        result.Number.Should().Be("Updated Number");
    }

    [Fact]
    public async Task UpdateAsync_WhenAddressDoesNotExist_ReturnsNull()
    {
        // Arrange
        var db = NewDb();
        var service = CreateService(db);

        var command = new AddressCommand(
            "Updated Street",
            "Updated Number",
            "Updated Complement",
            "Updated Neighborhood",
            "12345678",
            Guid.NewGuid(),
            "Updated City",
            "Updated Country");

        // Act
        var result = await service.UpdateAsync(Guid.NewGuid(), command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenAddressExists_ReturnsResponse()
    {
        // Arrange
        var stateId = Guid.NewGuid();
        var state = new StateModel { Id = stateId, Name = "State", Uf = "ST" };
        var address = new AddressModel
        {
            Id = Guid.NewGuid(),
            Street = _faker.Address.StreetAddress(),
            Number = _faker.Address.BuildingNumber(),
            Complement = _faker.Address.SecondaryAddress(),
            Neighborhood = _faker.Address.City(),
            ZipCode = _faker.Address.ZipCode(),
            StateId = stateId,
            City = _faker.Address.City(),
            Country = _faker.Address.Country()
        };

        var db = NewDb();
        db.AuthStates.Add(state);
        db.AuthAddresses.Add(address);
        await db.SaveChangesAsync(CancellationToken.None);

        var service = CreateService(db);

        // Act
        var result = await service.GetByIdAsync(address.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(address.Id);
        result.Street.Should().Be(address.Street);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAddressDoesNotExist_ReturnsNull()
    {
        // Arrange
        var db = NewDb();
        var service = CreateService(db);

        // Act
        var result = await service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    private DefaultContext NewDb()
    {
        return new DefaultContext(_dbOptions, _mockCompanyContext.Object);
    }

    private AddressService CreateService(DefaultContext db)
    {
        return new AddressService(new AddressRepository(db));
    }
}
