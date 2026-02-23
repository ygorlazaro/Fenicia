using System.Security.Claims;

using Fenicia.Auth.Domains.Order.CreateNewOrder;
using Fenicia.Auth.Domains.Subscription.CreateCreditsForOrder;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Migrations.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Order;

[TestFixture]
public class OrderControllerTests
{
    private Auth.Domains.Order.OrderController controller = null!;
    private AuthContext context = null!;
    private CreateNewOrderHandler createNewOrderHandler = null!;
    private Mock<CreateCreditsForOrderHandler> mockCreateCreditsForOrderHandler = null!;
    private Mock<IMigrationService> mockMigrationService = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testUserId;
    private Guid testCompanyId;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.testUserId = Guid.NewGuid();
        this.testCompanyId = Guid.NewGuid();
        this.mockCreateCreditsForOrderHandler = new Mock<CreateCreditsForOrderHandler>(this.context);
        this.mockMigrationService = new Mock<IMigrationService>();
        this.createNewOrderHandler = new CreateNewOrderHandler(this.context, this.mockCreateCreditsForOrderHandler.Object, this.mockMigrationService.Object);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new Auth.Domains.Order.OrderController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims(this.testUserId);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new("userId", userId.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    #region CreateNewOrderAsync Tests

    [Test]
    public void CreateNewOrderAsync_WhenUserDoesNotBelongToCompany_ThrowsPermissionDeniedException()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var modules = new List<Guid> { Guid.NewGuid() };
        var command = new CreateNewOrderCommand(this.testUserId, this.testCompanyId, modules);
        var headers = new Headers { CompanyId = this.testCompanyId };

        // Act & Assert
        Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.controller.CreateNewOrderAsync(
                command,
                headers,
                this.createNewOrderHandler,
                wide,
                cancellationToken));
    }

    [Test]
    public async Task CreateNewOrderAsync_WhenModulesDoNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = "test@example.com",
            Name = "Test User",
            Password = "hashedPassword"
        };

        var company = new CompanyModel
        {
            Id = this.testCompanyId,
            Name = "Test Company",
            Cnpj = "12.345.678/0001-90",
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            RoleId = Guid.NewGuid(),
            CompanyId = this.testCompanyId
        };

        this.context.Users.Add(user);
        this.context.Companies.Add(company);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var modules = new List<Guid> { Guid.NewGuid() };
        var command = new CreateNewOrderCommand(this.testUserId, this.testCompanyId, modules);
        var headers = new Headers { CompanyId = this.testCompanyId };

        // Act & Assert
        Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.controller.CreateNewOrderAsync(
                command,
                headers,
                this.createNewOrderHandler,
                wide,
                cancellationToken));
    }

    [Test]
    public async Task CreateNewOrderAsync_WhenValidRequest_ReturnsOkWithOrder()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var moduleId = Guid.NewGuid();
        var module = new ModuleModel
        {
            Id = moduleId,
            Name = "Basic Module",
            Type = ModuleType.Basic,
            Price = 10.0m
        };

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = "test@example.com",
            Name = "Test User",
            Password = "hashedPassword"
        };

        var company = new CompanyModel
        {
            Id = this.testCompanyId,
            Name = "Test Company",
            Cnpj = "12.345.678/0001-90",
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            RoleId = Guid.NewGuid(),
            CompanyId = this.testCompanyId
        };

        this.context.Modules.Add(module);
        this.context.Users.Add(user);
        this.context.Companies.Add(company);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var modules = new List<Guid> { moduleId };
        var command = new CreateNewOrderCommand(this.testUserId, this.testCompanyId, modules);
        var headers = new Headers { CompanyId = this.testCompanyId };

        this.mockMigrationService
            .Setup(x => x.RunMigrationsAsync(It.IsAny<Guid>(), It.IsAny<List<ModuleType>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await this.controller.CreateNewOrderAsync(
            command,
            headers,
            this.createNewOrderHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        var returnedResponse = okResult.Value as CreateNewOrderResponse;
        Assert.That(returnedResponse, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedResponse!.OrderId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
        }

        // Verify order was created
        var createdOrder = await this.context.Orders.FirstOrDefaultAsync(o => o.Id == returnedResponse.OrderId, cancellationToken: cancellationToken);
        Assert.That(createdOrder, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(createdOrder!.UserId, Is.EqualTo(this.testUserId));
            Assert.That(createdOrder.CompanyId, Is.EqualTo(this.testCompanyId));
        }
    }

    [Test]
    public async Task CreateNewOrderAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var moduleId = Guid.NewGuid();
        var module = new ModuleModel
        {
            Id = moduleId,
            Name = "Basic Module",
            Type = ModuleType.Basic,
            Price = 10.0m
        };

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = "test@example.com",
            Name = "Test User",
            Password = "hashedPassword"
        };

        var company = new CompanyModel
        {
            Id = this.testCompanyId,
            Name = "Test Company",
            Cnpj = "12.345.678/0001-90",
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            RoleId = Guid.NewGuid(),
            CompanyId = this.testCompanyId
        };

        this.context.Modules.Add(module);
        this.context.Users.Add(user);
        this.context.Companies.Add(company);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var modules = new List<Guid> { moduleId };
        var command = new CreateNewOrderCommand(this.testUserId, this.testCompanyId, modules);
        var headers = new Headers { CompanyId = this.testCompanyId };

        this.mockMigrationService
            .Setup(x => x.RunMigrationsAsync(It.IsAny<Guid>(), It.IsAny<List<ModuleType>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await this.controller.CreateNewOrderAsync(
            command,
            headers,
            this.createNewOrderHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
    }

    #endregion

    #region Attribute Tests

    [Test]
    public void OrderController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.Order.OrderController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "OrderController should have Authorize attribute");
    }

    [Test]
    public void OrderController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.Order.OrderController);

        // Act
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "OrderController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void OrderController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.Order.OrderController);

        // Act
        var producesAttribute = controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.That(producesAttribute, Is.Not.Null, "OrderController should have Produces attribute");
        Assert.That(producesAttribute!.ContentTypes.FirstOrDefault(), Is.EqualTo("application/json"));
    }

    #endregion
}
