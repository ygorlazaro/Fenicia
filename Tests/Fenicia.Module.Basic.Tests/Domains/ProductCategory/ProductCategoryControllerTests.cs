using System.Security.Claims;

using AwesomeAssertions;
using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class ProductCategoryControllerTests : IDisposable
{
    private readonly ProductCategoryController _controller;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<ProductCategoryService> _mockService;

    public ProductCategoryControllerTests()
    {
        _mockService = new Mock<ProductCategoryService>();
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new ProductCategoryController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _faker = new Faker();
        SetupUserClaims(Guid.NewGuid());
        SetupServiceMocks();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private void SetupServiceMocks()
    {
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllProductCategoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<List<GetAllProductCategoryResponse>>(new List<GetAllProductCategoryResponse>(), 0, 1, 10));

        _mockService.Setup(s => s.GetByIdAsync(It.Is<GetProductCategoryByIdQuery>(q => q.Id == It.IsAny<Guid>()), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProductCategoryByIdQuery q, CancellationToken ct) => new GetProductCategoryByIdResponse(q.Id, "Test Category"));

        _mockService.Setup(s => s.AddAsync(It.IsAny<AddProductCategoryCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddProductCategoryCommand cmd, Guid companyId, CancellationToken ct) => new AddProductCategoryResponse(cmd.Id, cmd.Name));

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateProductCategoryCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateProductCategoryCommand cmd, Guid companyId, CancellationToken ct) => new UpdateProductCategoryResponse(cmd.Id, cmd.Name));

        _mockService.Setup(s => s.DeleteAsync(It.IsAny<DeleteProductCategoryCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        // Arrange
        var command = new AddProductCategoryCommand(Guid.NewGuid(), _faker.Commerce.Categories(1).First());
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenCategoryExists_ReturnsOk()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new UpdateProductCategoryCommand(categoryId, "Updated Name");
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PatchAsync(command, categoryId, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenCategoryDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), "Updated Name");
        var wide = new WideEventContext();

        _mockService.Setup(s => s.UpdateAsync(It.Is<UpdateProductCategoryCommand>(c => c.Id != It.IsAny<Guid>()), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateProductCategoryResponse?)null);

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryExists_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.DeleteAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetAsync_WhenCategoriesExist_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ReturnsOk()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(categoryId, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();

        _mockService.Setup(s => s.GetByIdAsync(It.Is<GetProductCategoryByIdQuery>(q => q.Id != It.IsAny<Guid>()), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProductCategoryByIdResponse?)null);

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}
