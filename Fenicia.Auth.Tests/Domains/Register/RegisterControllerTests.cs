using Fenicia.Auth.Domains.Company.CheckCompanyExists;
using Fenicia.Auth.Domains.Register;
using Fenicia.Auth.Domains.Role.GetAdminRole;
using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.CreateNewUser;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Register;

public class RegisterControllerTests : IDisposable
{
    public RegisterControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.mockCheckUserExistsHandler = new Mock<CheckUserExistsHandler>(this.context);
        this.mockCheckCompanyExistsHandler = new Mock<CheckCompanyExistsHandler>(this.context);
        this.mockHashPasswordHandler = new Mock<HashPasswordHandler>();
        this.mockGetAdminRoleHandler = new Mock<GetAdminRoleHandler>(this.context);
        var createNewUserHandler = new CreateNewUserHandler(
            this.context,
            this.mockCheckUserExistsHandler.Object,
            this.mockCheckCompanyExistsHandler.Object,
            this.mockHashPasswordHandler.Object,
            this.mockGetAdminRoleHandler.Object);

        var mockHttpContext = new Mock<HttpContext>();

        this.controller = new RegisterController(createNewUserHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            }
        };
    }

    public void Dispose()
    {
        this.context.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly RegisterController controller;
    private readonly DefaultContext context;
    private readonly Mock<CheckUserExistsHandler> mockCheckUserExistsHandler;
    private readonly Mock<CheckCompanyExistsHandler> mockCheckCompanyExistsHandler;
    private readonly Mock<HashPasswordHandler> mockHashPasswordHandler;
    private readonly Mock<GetAdminRoleHandler> mockGetAdminRoleHandler;

    [Fact]
    public async Task CreateNewUserAsync_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        // Arrange
        var wide = new WideEventContext();
        var xrt = CancellationToken.None;

        var companyQuery = new CreateNewUserCompanyQuery("Company Name", "12.345.678/0001-90", "UTC");
        var query = new CreateNewUserQuery("existing@example.com", "password123", "Test User", companyQuery);

        this.mockCheckUserExistsHandler
            .Setup(h => h.Handle(query.Email, xrt))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.controller.CreateNewUserAsync(
                query,
                wide,
                xrt));
    }

    [Fact]
    public async Task CreateNewUserAsync_WhenCompanyAlreadyExists_ThrowsArgumentException()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var companyQuery = new CreateNewUserCompanyQuery("Existing Company", "12.345.678/0001-90", "UTC");
        var query = new CreateNewUserQuery("test@example.com", "password123", "Test User", companyQuery);

        this.mockCheckUserExistsHandler
            .Setup(h => h.Handle(query.Email, ct))
            .ReturnsAsync(false);

        var checkCompanyExistsQuery = new CheckCompanyExistsQuery(companyQuery.Cnpj, true);
        this.mockCheckCompanyExistsHandler
            .Setup(h => h.Handle(checkCompanyExistsQuery, ct))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.controller.CreateNewUserAsync(
                query,
                wide,
                ct));
    }

    [Fact]
    public async Task CreateNewUserAsync_WhenAdminRoleDoesNotExist_ThrowsArgumentException()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var companyQuery = new CreateNewUserCompanyQuery("Company Name", "12.345.678/0001-90", "UTC");
        var query = new CreateNewUserQuery("test@example.com", "password123", "Test User", companyQuery);

        this.mockCheckUserExistsHandler
            .Setup(h => h.Handle(query.Email, ct))
            .ReturnsAsync(false);

        var checkCompanyExistsQuery = new CheckCompanyExistsQuery(companyQuery.Cnpj, true);
        this.mockCheckCompanyExistsHandler
            .Setup(h => h.Handle(checkCompanyExistsQuery, ct))
            .ReturnsAsync(false);

        this.mockHashPasswordHandler
            .Setup(h => h.Handle(query.Password))
            .Returns("hashedPassword");

        this.mockGetAdminRoleHandler
            .Setup(h => h.Handle(ct))
            .ReturnsAsync((GetAdminRoleResponse?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.controller.CreateNewUserAsync(
                query,
                wide,
                ct));
    }

    [Fact]
    public async Task CreateNewUserAsync_WhenValidRequest_ReturnsOkWithUser()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var adminRoleId = Guid.NewGuid();
        var adminRole = new GetAdminRoleResponse(adminRoleId, "Admin");

        var companyQuery = new CreateNewUserCompanyQuery("Company Name", "12.345.678/0001-90", "UTC");
        var query = new CreateNewUserQuery("test@example.com", "password123", "Test User", companyQuery);

        this.mockCheckUserExistsHandler
            .Setup(h => h.Handle(query.Email, ct))
            .ReturnsAsync(false);

        var checkCompanyExistsQuery = new CheckCompanyExistsQuery(companyQuery.Cnpj, true);
        this.mockCheckCompanyExistsHandler
            .Setup(h => h.Handle(checkCompanyExistsQuery, ct))
            .ReturnsAsync(false);

        this.mockHashPasswordHandler
            .Setup(h => h.Handle(query.Password))
            .Returns("hashedPassword");

        this.mockGetAdminRoleHandler
            .Setup(h => h.Handle(ct))
            .ReturnsAsync(adminRole);

        // Act
        var result = await this.controller.CreateNewUserAsync(
            query,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var returnedResponse = okResult.Value as CreateNewUserResponse;
        Assert.NotNull(returnedResponse);
        Assert.Equal(query.Email, returnedResponse.Email);
        Assert.Equal(query.Name, returnedResponse.Name);
        Assert.Equal(companyQuery.Name, returnedResponse.Company.Name);
        Assert.Equal(query.Email, wide.UserId);

        // Verify user was created
        var createdUser = await this.context.AuthUsers.FirstOrDefaultAsync(u => u.Email == query.Email, ct);
        Assert.NotNull(createdUser);
        Assert.Equal("hashedPassword", createdUser.Password);

        // Verify company was created
        var createdCompany = await this.context.AuthCompanies.FirstOrDefaultAsync(c => c.Cnpj == companyQuery.Cnpj, ct);
        Assert.NotNull(createdCompany);
        Assert.Equal(companyQuery.Name, createdCompany.Name);

        // Verify user role was created
        var userRole = await this.context.AuthUserRoles.FirstOrDefaultAsync(ur => ur.UserId == createdUser.Id, ct);
        Assert.NotNull(userRole);
        Assert.Equal(adminRoleId, userRole.RoleId);
    }

    [Fact]
    public async Task CreateNewUserAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var adminRoleId = Guid.NewGuid();
        var adminRole = new GetAdminRoleResponse(adminRoleId, "Admin");

        var companyQuery = new CreateNewUserCompanyQuery("Company Name", "12.345.678/0001-90", "UTC");
        var query = new CreateNewUserQuery("test@example.com", "password123", "Test User", companyQuery);

        this.mockCheckUserExistsHandler
            .Setup(h => h.Handle(query.Email, ct))
            .ReturnsAsync(false);

        var checkCompanyExistsQuery = new CheckCompanyExistsQuery(companyQuery.Cnpj, true);
        this.mockCheckCompanyExistsHandler
            .Setup(h => h.Handle(checkCompanyExistsQuery, ct))
            .ReturnsAsync(false);

        this.mockHashPasswordHandler
            .Setup(h => h.Handle(query.Password))
            .Returns("hashedPassword");

        this.mockGetAdminRoleHandler
            .Setup(h => h.Handle(ct))
            .ReturnsAsync(adminRole);

        // Act
        await this.controller.CreateNewUserAsync(
            query,
            wide,
            ct);

        // Assert
        Assert.Equal(query.Email, wide.UserId);
    }

    [Fact]
    public void RegisterController_HasAllowAnonymousAttribute()
    {
        // Arrange
        var controllerType = typeof(RegisterController);

        // Act
        var allowAnonymousAttribute =
            controllerType.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(allowAnonymousAttribute);
    }

    [Fact]
    public void RegisterController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(RegisterController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void RegisterController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(RegisterController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public void RegisterController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(RegisterController);

        // Act
        var producesAttribute =
            controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }
}
