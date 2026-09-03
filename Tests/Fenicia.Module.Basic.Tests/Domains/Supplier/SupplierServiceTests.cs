using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Address.Interfaces;
using Fenicia.Module.Basic.Domains.PersonAddress.Interfaces;
using Fenicia.Module.Basic.Domains.Product.Interfaces;
using Fenicia.Module.Basic.Domains.StockMovement.Interfaces;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class SupplierServiceTests : IDisposable
{
    private readonly Faker _faker;
    private readonly Mock<ISupplierRepository> _mockRepository;
    private readonly SupplierService _service;

    public SupplierServiceTests()
    {
        _mockRepository = new Mock<ISupplierRepository>();
        var mockProductService = new Mock<IProductService>();
        var mockStockMovementService = new Mock<IStockMovementService>();
        var mockAddressService = new Mock<IAddressService>();
        var mockPersonAddressService = new Mock<IPersonAddressService>();
        _service = new SupplierService(
            _mockRepository.Object,
            mockProductService.Object,
            mockStockMovementService.Object,
            mockAddressService.Object,
            mockPersonAddressService.Object);
        _faker = new Faker();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSupplierExists_ReturnsSupplier()
    {
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = "Supp" };
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = person.Id, Person = person };
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(supplier.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);

        // Act
        var result = await _service.GetByIdAsync(new GetSupplierByIdQuery(supplier.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(supplier.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSupplierDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SupplierModel?)null);

        // Act
        var result = await _service.GetByIdAsync(new GetSupplierByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesSupplier()
    {
        // Arrange
        var command = new AddSupplierCommand(
            Guid.NewGuid(),
            "Test Supplier",
            _faker.Internet.Email(),
            _faker.Person.Random.AlphaNumeric(11),
            _faker.Phone.PhoneNumber(),
            "12345678901",
            null);
        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<SupplierModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SupplierModel s, CancellationToken _) => s);

        // Act
        var result = await _service.AddAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_WhenSupplierExists_UpdatesSupplier()
    {
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = "Old" };
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = person.Id, Person = person, Cnpj = "111" };
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(supplier.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);
        _mockRepository.Setup(r => r.UpdateAsync(supplier.Id, It.IsAny<SupplierModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid _, SupplierModel s, CancellationToken _) => s);

        var command = new UpdateSupplierCommand(
            supplier.Id,
            "Updated",
            _faker.Internet.Email(),
            _faker.Person.Random.AlphaNumeric(11),
            _faker.Phone.PhoneNumber(),
            "99999999999",
            null);

        // Act
        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(supplier.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenSupplierDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SupplierModel?)null);

        var command = new UpdateSupplierCommand(
            Guid.NewGuid(),
            "Updated",
            _faker.Internet.Email(),
            _faker.Person.Random.AlphaNumeric(11),
            _faker.Phone.PhoneNumber(),
            "99999999999",
            null);

        // Act
        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenSupplierExists_DeletesSupplier()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(new DeleteSupplierCommand(supplierId), Guid.NewGuid(), CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(supplierId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCountAsync_ReturnsCount()
    {
        // Arrange
        _mockRepository.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(3);

        // Act
        var result = await _service.GetCountAsync(CancellationToken.None);

        // Assert
        result.Should().Be(3);
    }
}