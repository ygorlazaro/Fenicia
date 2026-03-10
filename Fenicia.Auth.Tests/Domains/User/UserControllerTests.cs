using System.Security.Claims;

using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.ChangeUserPassword;
using Fenicia.Auth.Domains.User.CreateUser;
using Fenicia.Auth.Domains.User.DeleteUser;
using Fenicia.Auth.Domains.User.GetUserModules;
using Fenicia.Auth.Domains.User.ListUsers;
using Fenicia.Auth.Domains.User.UpdateUser;
using Fenicia.Auth.Domains.UserRole.GetUserCompanies;
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

using UserCompanyRoleCommand = Fenicia.Auth.Domains.User.CreateUser.UserCompanyRoleCommand;

namespace Fenicia.Auth.Tests.Domains.User;

public class UserControllerTests
{
    public UserControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());

        var checkUserExistsHandler = new CheckUserExistsHandler(this.context);
        var hashPasswordHandler = new HashPasswordHandler();

        this.testUserId = Guid.NewGuid();
        var getUserModuleModel = new GetUserModuleHandler(this.context);
        var getUserCompaniesHandler = new GetUserCompaniesHandler(this.context);
        var listUserHandler = new ListUsersHandler(this.context);
        var createUserHandler = new CreateUserHandler(this.context, checkUserExistsHandler, hashPasswordHandler);
        var updateUserHandler = new UpdateUserHandler(this.context);
        this.deleteUserHandler = new DeleteUserHandler(this.context);
        this.changeUserPasswordHandler = new ChangeUserPasswordHandler(this.context, hashPasswordHandler);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new UserController(getUserModuleModel, getUserCompaniesHandler, listUserHandler, createUserHandler, updateUserHandler)
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
    private readonly DefaultContext context;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId;
    private readonly Faker faker;
    private readonly DeleteUserHandler deleteUserHandler;
    private readonly ChangeUserPasswordHandler changeUserPasswordHandler;

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
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
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

        this.context.AuthModules.Add(module);
        this.context.AuthSubscriptions.Add(subscription);
        this.context.AuthSubscriptionCredits.Add(subscriptionCredit);
        this.context.AuthUsers.Add(user);
        this.context.AuthUserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

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
        Assert.Equal(moduleId, returnedModules[0].Id);
        Assert.Equal(module.Name, returnedModules[0].Name);
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
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
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
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
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
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

        this.context.AuthCompanies.Add(company);
        this.context.AuthRoles.Add(role);
        this.context.AuthUsers.Add(user);
        this.context.AuthUserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

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
        Assert.Equal(companyId, returnedCompanies[0].Id);
        Assert.Equal("Admin", returnedCompanies[0].Role);
        Assert.Equal(company.Name, returnedCompanies[0].CompanyName);
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
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
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public void UserController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(UserController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

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
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void UserController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(UserController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }

    #region GetAsync Tests (List Users)

    [Fact]
    public async Task GetAsync_WithGodRole_ReturnsOkWithUsers()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var page = 1;
        var pageSize = 10;
        string? searchTerm = null;

        // Act
        var result = await this.controller.GetAsync(page, pageSize, searchTerm, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetAsync_WithoutGodRole_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "User");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await this.controller.GetAsync(1, 10, null, CancellationToken.None));
    }

    [Fact]
    public async Task GetAsync_WithSearchTerm_FiltersUsers()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "God");

        var user1 = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = "john@example.com",
            Name = "John Doe",
            Password = this.faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = "jane@example.com",
            Name = "Jane Smith",
            Password = this.faker.Internet.Password()
        };

        this.context.AuthUsers.Add(user1);
        this.context.AuthUsers.Add(user2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.GetAsync(1, 10, "John", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ListUsersResponse>(okResult.Value);
        Assert.Single(response.Users);
        Assert.Contains("John", response.Users.First().Name);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithGodRole_ReturnsFullUserData()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "God");

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

        this.context.AuthRoles.Add(role);
        this.context.AuthCompanies.Add(company);
        this.context.AuthUsers.Add(user);
        this.context.AuthUserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.GetByIdAsync(user.Id, this.context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "God");
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var result = await this.controller.GetByIdAsync(nonExistentUserId, this.context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserIsDeleted_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password(),
            Deleted = DateTime.UtcNow
        };

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.GetByIdAsync(user.Id, this.context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithAdminRole_ThrowsUnauthorized()
    {
        // Arrange - Admin users cannot access this endpoint, only God can
        SetupUserClaims(this.testUserId, "Admin");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await this.controller.GetByIdAsync(Guid.NewGuid(), this.context, CancellationToken.None));
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithGodRole_CreatesUserSuccessfully()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "God");

        var query = new CreateUserQuery(
            this.faker.Internet.Email(),
            this.faker.Internet.Password(),
            this.faker.Person.FullName);

        // Act
        var result = await this.controller.CreateAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task CreateAsync_WithoutGodRole_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "User");

        var query = new CreateUserQuery(this.faker.Internet.Email(), this.faker.Internet.Password(),  this.faker.Person.FullName);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await this.controller.CreateAsync(query, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_WithAdminRole_ForAccessibleCompany_ThrowsUnauthorized()
    {
        // Arrange - Admin users cannot access this endpoint, only God can
        SetupUserClaims(this.testUserId, "Admin");

        var adminCompanyId = Guid.NewGuid();
        var adminRole = new RoleModel { Id = Guid.NewGuid(), Name = "Admin" };

        var query = new CreateUserQuery
        (
            this.faker.Internet.Email(),
            this.faker.Internet.Password(),
            this.faker.Person.FullName,
            [new UserCompanyRoleCommand(adminCompanyId, adminRole.Id)]
        );

        // Act & Assert - Admin users should not be able to access this endpoint
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await this.controller.CreateAsync(query, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_WithAdminRole_ForInaccessibleCompany_ThrowsUnauthorized()
    {
        // Arrange - Admin users cannot access this endpoint at all
        SetupUserClaims(this.testUserId, "Admin");

        var inaccessibleCompanyId = Guid.NewGuid();
        var adminRole = new RoleModel { Id = Guid.NewGuid(), Name = "Admin" };

        var query = new CreateUserQuery
        (
            this.faker.Internet.Email(),
            this.faker.Internet.Password(),
            this.faker.Person.FullName,
            [new UserCompanyRoleCommand(inaccessibleCompanyId, adminRole.Id)]
        );

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await this.controller.CreateAsync(query, CancellationToken.None));
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithGodRole_UpdatesUserSuccessfully()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new UpdateUserQuery(user.Id, "Updated Name", "updated@example.com");

        // Act
        var result = await this.controller.UpdateAsync(user.Id, query, this.context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_WithoutGodRole_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "User");

        var query = new UpdateUserQuery(this.testUserId, "Updated Name");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await this.controller.UpdateAsync(Guid.NewGuid(), query, this.context, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "God");
        var nonExistentUserId = Guid.NewGuid();

        var query = new UpdateUserQuery(nonExistentUserId, "Updated Name");

        // Act
        var result = await this.controller.UpdateAsync(nonExistentUserId, query, this.context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserIsDeleted_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password(),
            Deleted = DateTime.UtcNow
        };

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new UpdateUserQuery(user.Id, "Updated Name");

        // Act
        var result = await this.controller.UpdateAsync(user.Id, query, this.context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithGodRole_DeletesUserSuccessfully()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "God");

        var userToDelete = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.AuthUsers.Add(userToDelete);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.DeleteAsync(userToDelete.Id, this.deleteUserHandler, this.context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WithoutGodRole_ReturnsUnauthorized()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "Admin");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await this.controller.DeleteAsync(Guid.NewGuid(), this.deleteUserHandler, this.context, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "God");
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var result = await this.controller.DeleteAsync(nonExistentUserId, this.deleteUserHandler, this.context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserIsDeleted_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password(),
            Deleted = DateTime.UtcNow
        };

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.DeleteAsync(user.Id, this.deleteUserHandler, this.context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenAttemptingSelfDeletion_ReturnsBadRequest()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "God");

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.DeleteAsync(this.testUserId, this.deleteUserHandler, this.context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Cannot delete yourself", badRequest.Value?.ToString() ?? string.Empty);
    }

    #endregion

    #region ChangePasswordAsync Tests

    [Fact]
    public async Task ChangePasswordAsync_WithGodRole_ChangesPasswordSuccessfully()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new ChangeUserPasswordQuery(user.Id, this.faker.Internet.Password());

        // Act
        var result = await this.controller.ChangePasswordAsync(user.Id, query, this.changeUserPasswordHandler, this.context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithoutGodRole_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "User");

        var query = new ChangeUserPasswordQuery(this.testUserId, this.faker.Internet.Password());

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await this.controller.ChangePasswordAsync(Guid.NewGuid(), query, this.changeUserPasswordHandler));
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "God");
        var nonExistentUserId = Guid.NewGuid();

        var query = new ChangeUserPasswordQuery(this.testUserId, this.faker.Internet.Password());

        // Act
        var result = await this.controller.ChangePasswordAsync(nonExistentUserId, query, this.changeUserPasswordHandler, this.context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenUserIsDeleted_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(this.testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password(),
            Deleted = DateTime.UtcNow
        };

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new ChangeUserPasswordQuery(user.Id, this.faker.Internet.Password());
        
        // Act
        var result = await this.controller.ChangePasswordAsync(user.Id, query, this.changeUserPasswordHandler, this.context, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithAdminRole_ForAccessibleCompany_ThrowsUnauthorized()
    {
        // Arrange - Admin users cannot access this endpoint, only God can
        SetupUserClaims(this.testUserId, "Admin");

        var query = new ChangeUserPasswordQuery(Guid.NewGuid(), this.faker.Internet.Password());

        // Act & Assert - Admin users should not be able to access this endpoint
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await this.controller.ChangePasswordAsync(Guid.NewGuid(), query, this.changeUserPasswordHandler));
    }

    [Fact]
    public async Task ChangePasswordAsync_WithAdminRole_ForInaccessibleCompany_ThrowsUnauthorized()
    {
        // Arrange - Admin users cannot access this endpoint at all
        SetupUserClaims(this.testUserId, "Admin");

        var query = new ChangeUserPasswordQuery(Guid.NewGuid(), this.faker.Internet.Password());

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await this.controller.ChangePasswordAsync(Guid.NewGuid(), query, this.changeUserPasswordHandler));
    }

    #endregion

    private void SetupUserClaims(Guid userId, params string[] roles)
    {
        var claims = new List<Claim>
        {
            new("userId", userId.ToString())
        };

        claims.AddRange(roles.Select(role => new Claim("role", role)));

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}
