using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class ProductCategoryServiceTests : IDisposable
{
    private readonly DbContextOptions<DefaultContext> _dbOptions;
    private readonly Faker _faker;
    private readonly Mock<IProductCategoryRepository> _mockRepository;
    private readonly Mock<ICompanyContext> _mockCompanyContext;
    private readonly ProductCategoryService _service;

    public ProductCategoryServiceTests()
    {
        _dbOptions = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _mockRepository = new Mock<IProductCategoryRepository>();
        _mockCompanyContext = new Mock<ICompanyContext>();
        _service = new ProductCategoryService(_mockRepository.Object, _mockCompanyContext.Object);
        _faker = new Faker();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenCategoriesExist_ReturnsPaginationWithCategories()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        _mockCompanyContext.Setup(c => c.CompanyId).Returns(companyId);

        var db = NewDb();
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First(), CompanyId = companyId };
        db.BasicProductCategories.Add(category);
        await db.SaveChangesAsync(CancellationToken.None);
        _mockRepository.Setup(r => r.Query()).Returns(() => db.BasicProductCategories);

        // Act
        var result = await _service.GetAllAsync(new GetAllProductCategoryQuery());

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ReturnsCategory()
    {
        // Arrange
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        _mockRepository.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _service.GetByIdAsync(new GetProductCategoryByIdQuery(category.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(category.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCategoryModel?)null);

        // Act
        var result = await _service.GetByIdAsync(
            new GetProductCategoryByIdQuery(Guid.NewGuid()),
            CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesCategory()
    {
        // Arrange
        var command = new AddProductCategoryCommand(Guid.NewGuid(), _faker.Commerce.Categories(1).First());
        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<ProductCategoryModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCategoryModel c, CancellationToken _) => c);

        // Act
        var result = await _service.AddAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_WhenCategoryExists_UpdatesCategory()
    {
        // Arrange
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Old Name" };
        _mockRepository.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _mockRepository.Setup(r => r.UpdateAsync(
                category.Id,
                It.IsAny<ProductCategoryModel>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid _, ProductCategoryModel c, CancellationToken _) => c);

        var command = new UpdateProductCategoryCommand(category.Id, "Updated Name");

        // Act
        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateAsync_WhenCategoryDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCategoryModel?)null);

        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), "Updated Name");

        // Act
        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryExists_DeletesCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(
            new DeleteProductCategoryCommand(categoryId),
            Guid.NewGuid(),
            CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
    }

    private DefaultContext NewDb()
    {
        return new DefaultContext(_dbOptions, new TestCompanyContext());
    }
}