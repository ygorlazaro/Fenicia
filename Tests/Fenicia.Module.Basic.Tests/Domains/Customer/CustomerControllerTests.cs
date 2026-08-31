using System.Security.Claims;

using AwesomeAssertions;
using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Customer.DTOs;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class CustomerControllerTests : IDisposable
{
    private readonly CustomerController _controller;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<CustomerService> _mockService;

    public CustomerControllerTests()
    {
        _mockService = new Mock<CustomerService>();
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new CustomerController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
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
        var command = new AddCustomerCommand(_faker.Person.FullName, _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), null);
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenCustomerExists_ReturnsOk()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = new UpdateCustomerCommand(customerId, "Updated Name", _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), null);
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PatchAsync(command, customerId, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenCustomerDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var command = new UpdateCustomerCommand(Guid.NewGuid(), "Updated Name", _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), null);
        var wide = new WideEventContext();

        _mockService.Setup(s => s.UpdateAsync(It.Is<UpdateCustomerCommand>(c => c.Id != It.IsAny<Guid>()), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateCustomerResponse?)null);

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerExists_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.DeleteAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetAsync_WhenCustomersExist_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerExists_ReturnsOk()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(customerId, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();

        _mockService.Setup(s => s.GetByIdAsync(It.Is<GetCustomerByIdQuery>(q => q.Id != It.IsAny<Guid>()), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetCustomerByIdResponse?)null);

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    private void SetupServiceMocks()
    {
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllCustomerQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<List<GetAllCustomerResponse>>(new List<GetAllCustomerResponse>(), 0, 1, 10));

        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetCustomerByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetCustomerByIdQuery q, CancellationToken cancellationToken) => new GetCustomerByIdResponse(q.Id, Guid.NewGuid(), "Test", "test@test.com", "123", "123", null));

        _mockService.Setup(s => s.AddAsync(It.IsAny<AddCustomerCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddCustomerCommand cmd, Guid companyId, CancellationToken cancellationToken) => new AddCustomerResponse(Guid.NewGuid(), Guid.NewGuid()));

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateCustomerCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateCustomerCommand cmd, Guid companyId, CancellationToken cancellationToken) => new UpdateCustomerResponse(cmd.Id, Guid.NewGuid()));

        _mockService.Setup(s => s.DeleteAsync(It.IsAny<DeleteCustomerCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
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
