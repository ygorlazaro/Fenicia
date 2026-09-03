using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.DTOs;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;
using Fenicia.Module.Basic.Domains.StockMovement.Interfaces;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class ProductServiceTests : IDisposable
{
    private readonly Faker _faker;
    private readonly Mock<IProductCategoryService> _mockCategoryService;
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _mockCategoryService = new Mock<IProductCategoryService>();
        var mockOrderDetailService = new Mock<IOrderDetailService>();
        var mockStockMovementService = new Mock<IStockMovementService>();
        _service = new ProductService(
            _mockRepository.Object,
            _mockCategoryService.Object,
            mockOrderDetailService.Object,
            mockStockMovementService.Object);
        _faker = new Faker();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Cat" };
        var supplier = new SupplierModel
            { Id = Guid.NewGuid(), Person = new PersonModel { Id = Guid.NewGuid(), Name = "Sup" } };
        var product = new ProductModel
        {
            Id = Guid.NewGuid(), Name = "Test", SalesPrice = 100m, CategoryId = category.Id, Category = category,
            SupplierId = supplier.Id, Supplier = supplier
        };
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetByIdAsync(new GetProductByIdQuery(product.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductModel?)null);

        // Act
        var result = await _service.GetByIdAsync(new GetProductByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesProduct()
    {
        // Arrange
        var command = new AddProductCommand(
            Guid.NewGuid(),
            _faker.Commerce.ProductName(),
            SalesPrice: _faker.Random.Decimal());
        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductModel p, CancellationToken _) => p);
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid _, CancellationToken _) => null);
        _mockCategoryService.Setup(c => c.GetByIdAsync(
                It.IsAny<GetProductCategoryByIdQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProductCategoryByIdResponse?)null);

        // Act
        var result = await _service.AddAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductExists_UpdatesProduct()
    {
        // Arrange
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Cat" };
        var product = new ProductModel
            { Id = Guid.NewGuid(), Name = "Old", SalesPrice = 100m, CategoryId = category.Id, Category = category };
        _mockRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockRepository.Setup(r => r.UpdateAsync(product.Id, It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid _, ProductModel p, CancellationToken _) => p);
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockCategoryService.Setup(c => c.GetByIdAsync(
                It.IsAny<GetProductCategoryByIdQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetProductCategoryByIdResponse(category.Id, category.Name));

        var command = new UpdateProductCommand(product.Id, "Updated", SalesPrice: 200m);

        // Act
        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductModel?)null);

        var command = new UpdateProductCommand(Guid.NewGuid(), "Updated", SalesPrice: 200m);

        // Act
        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_DeletesProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(new DeleteProductCommand(productId), Guid.NewGuid(), CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCountAsync_ReturnsCount()
    {
        // Arrange
        _mockRepository.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(42);

        // Act
        var result = await _service.GetCountAsync(CancellationToken.None);

        // Assert
        result.Should().Be(42);
    }
}