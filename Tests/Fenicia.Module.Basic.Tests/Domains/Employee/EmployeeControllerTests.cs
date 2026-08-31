using System.Security.Claims;

using AwesomeAssertions;
using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Employee.DTOs;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

public class EmployeeControllerTests : IDisposable
{
    private readonly EmployeeController _controller;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<EmployeeService> _mockService;

    public EmployeeControllerTests()
    {
        _mockService = new Mock<EmployeeService>();
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new EmployeeController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
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
        var command = new AddEmployeeCommand(Guid.NewGuid(), Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), null);
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenEmployeeExists_ReturnsOk()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var command = new UpdateEmployeeCommand(employeeId, Guid.NewGuid(), "Updated Name", _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), null);
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PatchAsync(command, employeeId, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenEmployeeDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var command = new UpdateEmployeeCommand(Guid.NewGuid(), Guid.NewGuid(), "Updated Name", _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), null);
        var wide = new WideEventContext();

        _mockService.Setup(s => s.UpdateAsync(It.Is<UpdateEmployeeCommand>(c => c.Id != It.IsAny<Guid>()), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateEmployeeResponse?)null);

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenEmployeeExists_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.DeleteAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetAsync_WhenEmployeesExist_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsOk()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(employeeId, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();

        _mockService.Setup(s => s.GetByIdAsync(It.Is<GetEmployeeByIdQuery>(q => q.Id != It.IsAny<Guid>()), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetEmployeeByIdResponse?)null);

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    private void SetupServiceMocks()
    {
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllEmployeeQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<List<GetAllEmployeeResponse>>(new List<GetAllEmployeeResponse>(), 0, 1, 10));

        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetEmployeeByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetEmployeeByIdQuery q, CancellationToken cancellationToken) => new GetEmployeeByIdResponse(q.Id, Guid.NewGuid(), Guid.NewGuid(), "Test", "test@test.com", "123", "123", null));

        _mockService.Setup(s => s.AddAsync(It.IsAny<AddEmployeeCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddEmployeeCommand cmd, Guid companyId, CancellationToken cancellationToken) => new AddEmployeeResponse(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateEmployeeCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateEmployeeCommand cmd, Guid companyId, CancellationToken cancellationToken) => new UpdateEmployeeResponse(cmd.Id, Guid.NewGuid(), Guid.NewGuid()));

        _mockService.Setup(s => s.DeleteAsync(It.IsAny<DeleteEmployeeCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
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
