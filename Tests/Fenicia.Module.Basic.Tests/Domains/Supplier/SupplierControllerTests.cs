using System.Security.Claims;

using AwesomeAssertions;
using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class SupplierControllerTests : IDisposable
{
    private readonly SupplierController _controller;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<SupplierService> _mockService;

    public SupplierControllerTests()
    {
        _mockService = new Mock<SupplierService>();
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new SupplierController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _faker = new Faker();
        SetupUserClaims(Guid.NewGuid());
        SetupServiceMocks();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        // Arrange
        var command = new AddSupplierCommand(Guid.NewGuid(), _faker.Person.FullName, _faker.Person.Email, _faker.Person.Random.AlphaNumeric(11), _faker.Person.Random.AlphaNumeric(11), _faker.Person.Random.AlphaNumeric(11), new AddressDTO(_faker.Address.StreetAddress(), _faker.Address.BuildingNumber(), null, _faker.Address.City(), _faker.Address.ZipCode(), Guid.NewGuid(), _faker.Address.City(), null));
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenSupplierExists_ReturnsOk()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var command = new UpdateSupplierCommand(supplierId, _faker.Person.FullName, _faker.Person.Email, _faker.Person.Random.AlphaNumeric(11), "12345678900", "12345678900", new AddressDTO(_faker.Address.StreetAddress(), _faker.Address.BuildingNumber(), null, _faker.Address.City(), _faker.Address.ZipCode(), Guid.NewGuid(), _faker.Address.City(), null));
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PatchAsync(command, supplierId, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenSupplierDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var command = new UpdateSupplierCommand(Guid.NewGuid(), _faker.Person.FullName, _faker.Person.Email, _faker.Person.Random.AlphaNumeric(11), "12345678900", "12345678900", new AddressDTO(_faker.Address.StreetAddress(), _faker.Address.BuildingNumber(), null, _faker.Address.City(), _faker.Address.ZipCode(), Guid.NewGuid(), _faker.Address.City(), null));
        var wide = new WideEventContext();

        _mockService.Setup(s => s.UpdateAsync(It.Is<UpdateSupplierCommand>(c => c.Id != It.IsAny<Guid>()), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateSupplierResponse?)null);

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenSupplierExists_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.DeleteAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetAsync_WhenSuppliersExist_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenSupplierExists_ReturnsOk()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(supplierId, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenSupplierDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();

        _mockService.Setup(s => s.GetByIdAsync(It.Is<GetSupplierByIdQuery>(q => q.Id != It.IsAny<Guid>()), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetSupplierByIdResponse?)null);

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    private void SetupServiceMocks()
    {
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllSupplierQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<List<GetAllSupplierResponse>>(new List<GetAllSupplierResponse>(), 0, 1, 10));

        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetSupplierByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetSupplierByIdQuery q, CancellationToken cancellationToken) => new GetSupplierByIdResponse(q.Id, Guid.NewGuid(), "Test", "test@test.com", "123", "123", null));

        _mockService.Setup(s => s.AddAsync(It.IsAny<AddSupplierCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddSupplierCommand cmd, Guid companyId, CancellationToken cancellationToken) => new AddSupplierResponse(cmd.Id, cmd.Cnpj));

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateSupplierCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateSupplierCommand cmd, Guid companyId, CancellationToken cancellationToken) => new UpdateSupplierResponse(cmd.Id, cmd.Cnpj));

        _mockService.Setup(s => s.DeleteAsync(It.IsAny<DeleteSupplierCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
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
