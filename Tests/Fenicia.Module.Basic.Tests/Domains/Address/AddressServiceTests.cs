using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Address;

public class AddressServiceTests : IDisposable
{
    private readonly Faker _faker;
    private readonly Mock<IAddressRepository> _mockRepository;
    private readonly AddressService _service;

    public AddressServiceTests()
    {
        _mockRepository = new Mock<IAddressRepository>();
        _service = new AddressService(_mockRepository.Object);
        _faker = new Faker();
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
        var address = new AddressModel
        {
            Id = Guid.NewGuid(),
            Street = command.Street,
            Number = command.Number,
            Complement = command.Complement,
            Neighborhood = command.Neighborhood,
            ZipCode = command.ZipCode,
            StateId = command.StateId,
            State = state,
            City = command.City,
            Country = command.Country
        };

        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<AddressModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(address);

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
            State = state,
            City = "Old City",
            Country = "Old Country"
        };

        _mockRepository.Setup(r => r.UpdateAsync(address.Id, It.IsAny<AddressModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid _, AddressModel a, CancellationToken _) =>
            {
                a.State = new StateModel { Id = a.StateId, Name = "State", Uf = "ST" };
                return a;
            });

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
        var result = await _service.UpdateAsync(address.Id, command, CancellationToken.None);

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
        _mockRepository.Setup(r => r.UpdateAsync(
                It.IsAny<Guid>(),
                It.IsAny<AddressModel>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressModel?)null);

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
        var result = await _service.UpdateAsync(Guid.NewGuid(), command, CancellationToken.None);

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
            State = state,
            City = _faker.Address.City(),
            Country = _faker.Address.Country()
        };

        _mockRepository.Setup(r => r.GetByIdAsync(address.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(address);

        // Act
        var result = await _service.GetByIdAsync(address.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(address.Id);
        result.Street.Should().Be(address.Street);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAddressDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressModel?)null);

        // Act
        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}