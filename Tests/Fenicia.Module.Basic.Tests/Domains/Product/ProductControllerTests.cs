using System.Security.Claims;

using AwesomeAssertions;
using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.DTOs;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class ProductControllerTests : IDisposable
{
    private readonly ProductController _controller;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<ProductService> _mockService;

    public ProductControllerTests()
    {
        _mockService = new Mock<ProductService>();
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new ProductController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
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
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllProductQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<List<GetAllProductResponse>>(new List<GetAllProductResponse>(), 0, 1, 10));

        _mockService.Setup(s => s.GetByIdAsync(It.Is<GetProductByIdQuery>(q => q.Id == It.IsAny<Guid>()), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProductByIdQuery q, CancellationToken ct) => new GetProductByIdResponse(q.Id, "Test Product", null, null, null, null, 100, 10, null, null, null, null, null, null, Guid.NewGuid(), "Category", null, null, true));

        _mockService.Setup(s => s.AddAsync(It.IsAny<AddProductCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddProductCommand cmd, Guid companyId, CancellationToken ct) => new AddProductResponse(cmd.Id, cmd.Name, null, null, null, null, 100, 10, null, null, null, null, null, null, Guid.NewGuid(), "Category", null, null, true));

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateProductCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateProductCommand cmd, Guid companyId, CancellationToken ct) => new UpdateProductResponse(cmd.Id, cmd.Name, null, null, null, null, 100, 10, null, null, null, null, null, null, Guid.NewGuid(), "Category", null, null, true));

        _mockService.Setup(s => s.DeleteAsync(It.IsAny<DeleteProductCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        // Arrange
        var command = new AddProductCommand(Id: Guid.NewGuid(), Name: _faker.Commerce.ProductName(), SalesPrice: _faker.Random.Decimal());
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenProductExists_ReturnsOk()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new UpdateProductCommand(productId, Name: "Updated Name", SalesPrice: _faker.Random.Decimal());
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PatchAsync(command, productId, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var command = new UpdateProductCommand(Id: Guid.NewGuid(), Name: "Updated Name", SalesPrice: _faker.Random.Decimal());
        var wide = new WideEventContext();

        _mockService.Setup(s => s.UpdateAsync(It.Is<UpdateProductCommand>(c => c.Id != It.IsAny<Guid>()), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateProductResponse?)null);

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.DeleteAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetAsync_WhenProductsExist_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsOk()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(productId, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();

        _mockService.Setup(s => s.GetByIdAsync(It.Is<GetProductByIdQuery>(q => q.Id != It.IsAny<Guid>()), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProductByIdResponse?)null);

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
