using System.Security.Claims;

using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.User;
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
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.testUserId = Guid.NewGuid();
        this.getUserModuleHandler = new GetUserModuleHandler(this.context);
        this.getUserCompaniesHandler = new GetUserCompaniesHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new UserController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims(this.testUserId);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private UserController controller = null!;
    private AuthContext context = null!;
    private GetUserModuleHandler getUserModuleHandler = null!;
    private GetUserCompaniesHandler getUserCompaniesHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testUserId;
    private Faker faker = null!;

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

        var returnedModules = okResult.Value as List<GetUserModulesResponse>;
        Assert.That(returnedModules, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedModules, Is.Empty);
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
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = this.faker.Finance.Amount(10, 100)
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
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
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

        var returnedModules = okResult.Value as List<GetUserModulesResponse>;
        Assert.That(returnedModules, Is.Not.Null);
        Assert.That(returnedModules, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedModules[0].Id, Is.EqualTo(moduleId));
            Assert.That(returnedModules[0].Name, Is.EqualTo(module.Name));
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
            Assert.That(returnedCompanies, Is.Empty);
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
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
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
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
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
        Assert.That(returnedCompanies, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedCompanies[0].Id, Is.EqualTo(companyId));
            Assert.That(returnedCompanies[0].Role, Is.EqualTo("Admin"));
            Assert.That(returnedCompanies[0].Company.Name, Is.EqualTo(company.Name));
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

    [Test]
    public void UserController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(UserController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "UserController should have Authorize attribute");
    }

    [Test]
    public void UserController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(UserController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "UserController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void UserController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(UserController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "UserController should have ApiController attribute");
    }
}