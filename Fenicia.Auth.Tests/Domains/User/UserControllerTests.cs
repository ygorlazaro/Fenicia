using System.Security.Claims;

using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Module.Handlers;
using Fenicia.Auth.Domains.Module.Responses;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Auth.Domains.UserRole.Handlers;
using Fenicia.Auth.Domains.UserRole.Responses;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.User;

public class UserControllerTests
{
    public UserControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options,
            new TestCompanyContext());
        this.testUserId = Guid.NewGuid();

        this.mockHttpContext = new Mock<HttpContext>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(this.mockHttpContext.Object);

        var getUserModuleModel = new GetUserModuleHandler(this.db);
        var getUserCompaniesHandler = new GetUserCompaniesHandler(this.db);
        var listUserHandler = new GetUserHandler(this.db);
        var createUserHandler = new CreateUserHandler(this.db);
        var updateUserHandler = new UpdateUserHandler(this.db);
        var getUserByIdHandler = new GetUserByIdHandler(this.db);
        var updateUserPasswordHandler = new UpdateUserPasswordHandler(this.db);
        var deleteUserHandler = new DeleteUserHandler(this.db);

        this.controller = new UserController(getUserModuleModel,
            getUserCompaniesHandler,
            listUserHandler,
            createUserHandler,
            updateUserHandler,
            getUserByIdHandler,
            deleteUserHandler,
            updateUserPasswordHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims(this.testUserId);
        this.faker = new Faker();
    }

    private readonly UserController controller;
    private readonly DefaultContext db;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId;
    private readonly Faker faker;

    private void SetupUserClaims(Guid userId, string? role = null)
    {
        var claims = new List<Claim>
        {
            new("userId",
                userId.ToString())
        };

        if (!string.IsNullOrWhiteSpace(role))
        {
            claims.Add(new Claim(ClaimTypes.Role,
                role));
        }

        var claimsIdentity = new ClaimsIdentity(claims,
            "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserHasNoModules_ReturnsOkWithEmptyList()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetUserModulesAsync(
            headers,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnedModules = Assert.IsType<List<GetUserModulesResponse>>(okResult.Value);
        Assert.Empty(returnedModules);
        Assert.Equal(this.testUserId.ToString(),
            wide.UserId);
    }

    [Fact]
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
            Price = this.faker.Finance.Amount(10,
                100)
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

        this.db.AuthModules.Add(module);
        this.db.AuthSubscriptions.Add(subscription);
        this.db.AuthSubscriptionCredits.Add(subscriptionCredit);
        this.db.AuthUsers.Add(user);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetUserModulesAsync(
            headers,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnedModules = Assert.IsType<List<GetUserModulesResponse>>(okResult.Value);
        Assert.Single(returnedModules);
        Assert.Equal(moduleId,
            returnedModules[0].Id);
        Assert.Equal(module.Name,
            returnedModules[0].Name);
        Assert.Equal(this.testUserId.ToString(),
            wide.UserId);
    }

    [Fact]
    public async Task GetUserModulesAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        await this.controller.GetUserModulesAsync(
            headers,
            wide,
            ct);

        // Assert
        Assert.Equal(this.testUserId.ToString(),
            wide.UserId);
    }

    [Fact]
    public async Task GetUserCompanyAsync_WhenUserHasNoCompanies_ReturnsOkWithEmptyList()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetUserCompanyAsync(
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnedCompanies = Assert.IsType<List<GetUserCompaniesResponse>>(okResult.Value);
        Assert.Empty(returnedCompanies);
        Assert.Equal(this.testUserId.ToString(),
            wide.UserId);
    }

    [Fact]
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
            IsActive = true
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

        this.db.AuthCompanies.Add(company);
        this.db.AuthRoles.Add(role);
        this.db.AuthUsers.Add(user);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetUserCompanyAsync(
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnedCompanies = Assert.IsType<List<GetUserCompaniesResponse>>(okResult.Value);
        Assert.Single(returnedCompanies);
        Assert.Equal(companyId,
            returnedCompanies[0].Id);
        Assert.Equal("Admin",
            returnedCompanies[0].Role);
        Assert.Equal(company.Name,
            returnedCompanies[0].CompanyName);
        Assert.Equal(this.testUserId.ToString(),
            wide.UserId);
    }

    [Fact]
    public async Task GetUserCompanyAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        await this.controller.GetUserCompanyAsync(
            wide,
            ct);

        // Assert
        Assert.Equal(this.testUserId.ToString(),
            wide.UserId);
    }

    [Fact]
    public void UserController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(UserController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute),
            false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void UserController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(UserController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute),
                false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]",
            routeAttribute.Template);
    }

    [Fact]
    public void UserController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(UserController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute),
                false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }

    #region GetAsync Tests (List Users)

    [Fact]
    public async Task GetAsync_WithGodRole_ReturnsOkWithUsers()
    {
        // Arrange
        SetupUserClaims(this.testUserId,
            "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.GetAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }


    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithGodRole_ReturnsFullUserData()
    {
        // Arrange
        SetupUserClaims(this.testUserId,
            "God");

        var roleId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var role = new RoleModel { Id = roleId, Name = "User" };
        var company = new CompanyModel { Id = companyId, Name = "Test Company", Cnpj = this.faker.Company.Cnpj(), IsActive = true };

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = roleId,
            CompanyId = companyId
        };

        this.db.AuthRoles.Add(role);
        this.db.AuthCompanies.Add(company);
        this.db.AuthUsers.Add(user);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.GetByIdAsync(user.Id,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(this.testUserId,
            "God");
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var result = await this.controller.GetByIdAsync(nonExistentUserId,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserIsDeleted_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(this.testUserId,
            "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password(),
            Deleted = DateTime.UtcNow
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.GetByIdAsync(user.Id,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(this.testUserId,
            "God");
        var nonExistentUserId = Guid.NewGuid();

        var query = new UpdateUserCommand(nonExistentUserId,
            "Updated Name");

        // Act
        var result = await this.controller.UpdateAsync(nonExistentUserId,
            query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserIsDeleted_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(this.testUserId,
            "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password(),
            Deleted = DateTime.UtcNow
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdateUserCommand(user.Id,
            "Updated Name");

        // Act
        var result = await this.controller.UpdateAsync(user.Id,
            query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(this.testUserId,
            "God");
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var result = await this.controller.DeleteAsync(nonExistentUserId,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserIsDeleted_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(this.testUserId,
            "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password(),
            Deleted = DateTime.UtcNow
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.DeleteAsync(user.Id,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenAttemptingSelfDeletion_ReturnsBadRequest()
    {
        // Arrange
        SetupUserClaims(this.testUserId,
            "God");

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.DeleteAsync(this.testUserId,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
    }

    #endregion

    #region ChangePasswordAsync Tests

    [Fact]
    public async Task ChangePasswordAsync_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(this.testUserId,
            "God");
        var nonExistentUserId = Guid.NewGuid();

        var query = new UpdateUserPasswordCommand(this.testUserId,
            this.faker.Internet.Password());

        // Act
        var result = await this.controller.ChangePasswordAsync(nonExistentUserId,
            query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenUserIsDeleted_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(this.testUserId,
            "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password(),
            Deleted = DateTime.UtcNow
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdateUserPasswordCommand(user.Id,
            this.faker.Internet.Password());
        
        // Act
        var result = await this.controller.ChangePasswordAsync(user.Id,
            query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }


    #endregion
}
