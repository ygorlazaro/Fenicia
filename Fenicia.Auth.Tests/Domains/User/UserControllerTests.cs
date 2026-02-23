using System.Security.Claims;

using Fenicia.Auth.Domains.User.GetUserModules;
using Fenicia.Auth.Domains.UserRole.GetUserCompanies;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.User;

[TestFixture]
public class UserControllerTests
{
    private Auth.Domains.User.UserController controller = null!;
    private AuthContext context = null!;
    private GetUserModuleHandler getUserModuleHandler = null!;
    private GetUserCompaniesHandler getUserCompaniesHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testUserId;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.testUserId = Guid.NewGuid();
        this.getUserModuleHandler = new GetUserModuleHandler(this.context);
        this.getUserCompaniesHandler = new GetUserCompaniesHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new Auth.Domains.User.UserController();
        this.controller.ControllerContext = new ControllerContext
        {
            HttpContext = this.mockHttpContext.Object
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

    #region GetUserModulesAsync Tests

    [Test]
    public async Task GetUserModulesAsync_WhenUserHasNoModules_ReturnsOkWithEmptyList()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetUserModulesAsync(
            headers,
            this.getUserModuleHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedModules = okResult.Value as List<ModuleResponse>;
        Assert.That(returnedModules, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedModules.Count, Is.Zero);
            Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
        }
    }

    [Test]
    public async Task GetUserModulesAsync_WhenUserHasActiveSubscription_ReturnsOkWithModules()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var subscriptionCreditId = Guid.NewGuid();

        var module = new ModuleModel
        {
            Id = moduleId,
            Name = "Basic Module",
            Type = ModuleType.Basic,
            Price = 10.0m
        };

        var subscription = new SubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = companyId,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.Now.AddDays(-10),
            EndDate = DateTime.Now.AddDays(30)
        };

        var subscriptionCredit = new SubscriptionCreditModel
        {
            Id = subscriptionCreditId,
            SubscriptionId = subscriptionId,
            ModuleId = moduleId,
            IsActive = true,
            StartDate = DateTime.Now.AddDays(-10),
            EndDate = DateTime.Now.AddDays(30)
        };

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = "test@example.com",
            Name = "Test User",
            Password = "hashedPassword"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            RoleId = Guid.NewGuid(),
            CompanyId = companyId
        };

        this.context.Modules.Add(module);
        this.context.Subscriptions.Add(subscription);
        this.context.SubscriptionCredits.Add(subscriptionCredit);
        this.context.Users.Add(user);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetUserModulesAsync(
            headers,
            this.getUserModuleHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedModules = okResult.Value as List<ModuleResponse>;
        Assert.That(returnedModules, Is.Not.Null);
        Assert.That(returnedModules.Count, Is.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedModules[0].Id, Is.EqualTo(moduleId));
            Assert.That(returnedModules[0].Name, Is.EqualTo("Basic Module"));
            Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
        }
    }

    [Test]
    public async Task GetUserModulesAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        await this.controller.GetUserModulesAsync(
            headers,
            this.getUserModuleHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
    }

    #endregion

    #region GetUserCompanyAsync Tests

    [Test]
    public async Task GetUserCompanyAsync_WhenUserHasNoCompanies_ReturnsOkWithEmptyList()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetUserCompanyAsync(
            this.getUserCompaniesHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedCompanies = okResult.Value as List<GetUserCompaniesResponse>;
        Assert.That(returnedCompanies, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedCompanies.Count, Is.Zero);
            Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
        }
    }

    [Test]
    public async Task GetUserCompanyAsync_WhenUserHasCompanies_ReturnsOkWithCompanies()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = "Test Company",
            Cnpj = "12.345.678/0001-90",
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = "test@example.com",
            Name = "Test User",
            Password = "hashedPassword"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            RoleId = roleId,
            CompanyId = companyId
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.Users.Add(user);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetUserCompanyAsync(
            this.getUserCompaniesHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedCompanies = okResult.Value as List<GetUserCompaniesResponse>;
        Assert.That(returnedCompanies, Is.Not.Null);
        Assert.That(returnedCompanies.Count, Is.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedCompanies[0].Id, Is.EqualTo(companyId));
            Assert.That(returnedCompanies[0].Role, Is.EqualTo("Admin"));
            Assert.That(returnedCompanies[0].Company.Name, Is.EqualTo("Test Company"));
            Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
        }
    }

    [Test]
    public async Task GetUserCompanyAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        await this.controller.GetUserCompanyAsync(
            this.getUserCompaniesHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
    }

    #endregion

    #region Attribute Tests

    [Test]
    public void UserController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.User.UserController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "UserController should have Authorize attribute");
    }

    [Test]
    public void UserController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.User.UserController);

        // Act
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "UserController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void UserController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.User.UserController);

        // Act
        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "UserController should have ApiController attribute");
    }

    #endregion
}
