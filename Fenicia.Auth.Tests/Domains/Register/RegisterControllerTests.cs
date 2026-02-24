using Fenicia.Auth.Domains.Company.CheckCompanyExists;
using Fenicia.Auth.Domains.Register;
using Fenicia.Auth.Domains.Role.GetAdminRole;
using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.CreateNewUser;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Migrations.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Register;

[TestFixture]
public class RegisterControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.mockCheckUserExistsHandler = new Mock<CheckUserExistsHandler>(this.context);
        this.mockCheckCompanyExistsHandler = new Mock<CheckCompanyExistsHandler>(this.context);
        this.mockHashPasswordHandler = new Mock<HashPasswordHandler>();
        this.mockGetAdminRoleHandler = new Mock<GetAdminRoleHandler>(this.context);
        this.mockMigrationService = new Mock<IMigrationService>();
        this.createNewUserHandler = new CreateNewUserHandler(
            this.context,
            this.mockCheckUserExistsHandler.Object,
            this.mockCheckCompanyExistsHandler.Object,
            this.mockHashPasswordHandler.Object,
            this.mockGetAdminRoleHandler.Object,
            this.mockMigrationService.Object);

        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new RegisterController()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private RegisterController controller = null!;
    private AuthContext context = null!;
    private CreateNewUserHandler createNewUserHandler = null!;
    private Mock<CheckUserExistsHandler> mockCheckUserExistsHandler = null!;
    private Mock<CheckCompanyExistsHandler> mockCheckCompanyExistsHandler = null!;
    private Mock<HashPasswordHandler> mockHashPasswordHandler = null!;
    private Mock<GetAdminRoleHandler> mockGetAdminRoleHandler = null!;
    private Mock<IMigrationService> mockMigrationService = null!;
    private Mock<HttpContext> mockHttpContext = null!;

    [Test]
    public void CreateNewUserAsync_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var companyQuery = new CreateNewUserCompanyQuery("Company Name", "12.345.678/0001-90", "UTC");
        var query = new CreateNewUserQuery("existing@example.com", "password123", "Test User", companyQuery);

        this.mockCheckUserExistsHandler
            .Setup(h => h.Handle(query.Email, cancellationToken))
            .ReturnsAsync(true);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.controller.CreateNewUserAsync(
                query,
                this.createNewUserHandler,
                wide,
                cancellationToken));
    }

    [Test]
    public void CreateNewUserAsync_WhenCompanyAlreadyExists_ThrowsArgumentException()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var companyQuery = new CreateNewUserCompanyQuery("Existing Company", "12.345.678/0001-90", "UTC");
        var query = new CreateNewUserQuery("test@example.com", "password123", "Test User", companyQuery);

        this.mockCheckUserExistsHandler
            .Setup(h => h.Handle(query.Email, cancellationToken))
            .ReturnsAsync(false);

        var checkCompanyExistsQuery = new CheckCompanyExistsQuery(companyQuery.Cnpj, true);
        this.mockCheckCompanyExistsHandler
            .Setup(h => h.Handle(checkCompanyExistsQuery, cancellationToken))
            .ReturnsAsync(true);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.controller.CreateNewUserAsync(
                query,
                this.createNewUserHandler,
                wide,
                cancellationToken));
    }

    [Test]
    public void CreateNewUserAsync_WhenAdminRoleDoesNotExist_ThrowsArgumentException()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var companyQuery = new CreateNewUserCompanyQuery("Company Name", "12.345.678/0001-90", "UTC");
        var query = new CreateNewUserQuery("test@example.com", "password123", "Test User", companyQuery);

        this.mockCheckUserExistsHandler
            .Setup(h => h.Handle(query.Email, cancellationToken))
            .ReturnsAsync(false);

        var checkCompanyExistsQuery = new CheckCompanyExistsQuery(companyQuery.Cnpj, true);
        this.mockCheckCompanyExistsHandler
            .Setup(h => h.Handle(checkCompanyExistsQuery, cancellationToken))
            .ReturnsAsync(false);

        this.mockHashPasswordHandler
            .Setup(h => h.Handle(query.Password))
            .Returns("hashedPassword");

        this.mockGetAdminRoleHandler
            .Setup(h => h.Handle(cancellationToken))
            .ReturnsAsync((GetAdminRoleResponse?)null);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.controller.CreateNewUserAsync(
                query,
                this.createNewUserHandler,
                wide,
                cancellationToken));
    }

    [Test]
    public async Task CreateNewUserAsync_WhenValidRequest_ReturnsOkWithUser()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var adminRoleId = Guid.NewGuid();
        var adminRole = new GetAdminRoleResponse(adminRoleId, "Admin");

        var companyQuery = new CreateNewUserCompanyQuery("Company Name", "12.345.678/0001-90", "UTC");
        var query = new CreateNewUserQuery("test@example.com", "password123", "Test User", companyQuery);

        this.mockCheckUserExistsHandler
            .Setup(h => h.Handle(query.Email, cancellationToken))
            .ReturnsAsync(false);

        var checkCompanyExistsQuery = new CheckCompanyExistsQuery(companyQuery.Cnpj, true);
        this.mockCheckCompanyExistsHandler
            .Setup(h => h.Handle(checkCompanyExistsQuery, cancellationToken))
            .ReturnsAsync(false);

        this.mockHashPasswordHandler
            .Setup(h => h.Handle(query.Password))
            .Returns("hashedPassword");

        this.mockGetAdminRoleHandler
            .Setup(h => h.Handle(cancellationToken))
            .ReturnsAsync(adminRole);

        this.mockMigrationService
            .Setup(x => x.RunMigrationsAsync(It.IsAny<Guid>(), It.IsAny<List<ModuleType>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await this.controller.CreateNewUserAsync(
            query,
            this.createNewUserHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        var returnedResponse = okResult.Value as CreateNewUserResponse;
        Assert.That(returnedResponse, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedResponse!.Email, Is.EqualTo(query.Email));
            Assert.That(returnedResponse.Name, Is.EqualTo(query.Name));
            Assert.That(returnedResponse.Company.Name, Is.EqualTo(companyQuery.Name));
            Assert.That(wide.UserId, Is.EqualTo(query.Email));
        }

        // Verify user was created
        var createdUser = await this.context.Users.FirstOrDefaultAsync(u => u.Email == query.Email, cancellationToken: cancellationToken);
        Assert.That(createdUser, Is.Not.Null);
        Assert.That(createdUser!.Password, Is.EqualTo("hashedPassword"));

        // Verify company was created
        var createdCompany = await this.context.Companies.FirstOrDefaultAsync(c => c.Cnpj == companyQuery.Cnpj, cancellationToken: cancellationToken);
        Assert.That(createdCompany, Is.Not.Null);
        Assert.That(createdCompany!.Name, Is.EqualTo(companyQuery.Name));

        // Verify user role was created
        var userRole = await this.context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == createdUser.Id, cancellationToken: cancellationToken);
        Assert.That(userRole, Is.Not.Null);
        Assert.That(userRole!.RoleId, Is.EqualTo(adminRoleId));
    }

    [Test]
    public async Task CreateNewUserAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var adminRoleId = Guid.NewGuid();
        var adminRole = new GetAdminRoleResponse(adminRoleId, "Admin");

        var companyQuery = new CreateNewUserCompanyQuery("Company Name", "12.345.678/0001-90", "UTC");
        var query = new CreateNewUserQuery("test@example.com", "password123", "Test User", companyQuery);

        this.mockCheckUserExistsHandler
            .Setup(h => h.Handle(query.Email, cancellationToken))
            .ReturnsAsync(false);

        var checkCompanyExistsQuery = new CheckCompanyExistsQuery(companyQuery.Cnpj, true);
        this.mockCheckCompanyExistsHandler
            .Setup(h => h.Handle(checkCompanyExistsQuery, cancellationToken))
            .ReturnsAsync(false);

        this.mockHashPasswordHandler
            .Setup(h => h.Handle(query.Password))
            .Returns("hashedPassword");

        this.mockGetAdminRoleHandler
            .Setup(h => h.Handle(cancellationToken))
            .ReturnsAsync(adminRole);

        this.mockMigrationService
            .Setup(x => x.RunMigrationsAsync(It.IsAny<Guid>(), It.IsAny<List<ModuleType>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await this.controller.CreateNewUserAsync(
            query,
            this.createNewUserHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(wide.UserId, Is.EqualTo(query.Email));
    }

    [Test]
    public void RegisterController_HasAllowAnonymousAttribute()
    {
        // Arrange
        var controllerType = typeof(RegisterController);

        // Act
        var allowAnonymousAttribute =
            controllerType.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(allowAnonymousAttribute, Is.Not.Null, "RegisterController should have AllowAnonymous attribute");
    }

    [Test]
    public void RegisterController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(RegisterController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "RegisterController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void RegisterController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(RegisterController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "RegisterController should have ApiController attribute");
    }

    [Test]
    public void RegisterController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(RegisterController);

        // Act
        var producesAttribute =
            controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.That(producesAttribute, Is.Not.Null, "RegisterController should have Produces attribute");
        Assert.That(producesAttribute!.ContentTypes.FirstOrDefault(), Is.EqualTo("application/json"));
    }
}