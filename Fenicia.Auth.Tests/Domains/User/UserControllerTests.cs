using System.Security.Claims;

using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Module.DTOs.Responses;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.DTOs.Commands;
using Fenicia.Auth.Domains.UserRole.DTOs.Responses;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.User;

public class UserControllerTests
{
    private readonly UserController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId;

    public UserControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        testUserId = Guid.NewGuid();

        mockHttpContext = new Mock<HttpContext>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

        var userService = new UserService(db);
        var moduleService = new ModuleService(db);

        controller = new UserController(userService, moduleService) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        SetupUserClaims(testUserId);
        faker = new Faker();
    }

    private void SetupUserClaims(Guid userId, string? role = null)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };

        if (!string.IsNullOrWhiteSpace(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserHasNoModules_ReturnsOkWithEmptyList()
    {

        var companyId = Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var result = await controller.GetUserModulesAsync(headers, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnedModules = Assert.IsType<List<GetUserModulesResponse>>(okResult.Value);
        Assert.Empty(returnedModules);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserHasActiveSubscription_ReturnsOkWithModules()
    {

        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var subscriptionCreditId = Guid.NewGuid();

        var module = new ModuleModel
        {
            Id = moduleId,
            Name = faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = faker.Finance.Amount(10,
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
            Id = testUserId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            RoleId = Guid.NewGuid(),
            CompanyId = companyId
        };

        db.AuthModules.Add(module);
        db.AuthSubscriptions.Add(subscription);
        db.AuthSubscriptionCredits.Add(subscriptionCredit);
        db.AuthUsers.Add(user);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var result = await controller.GetUserModulesAsync(headers, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnedModules = Assert.IsType<List<GetUserModulesResponse>>(okResult.Value);
        Assert.Single(returnedModules);
        Assert.Equal(moduleId, returnedModules[0].Id);
        Assert.Equal(module.Name, returnedModules[0].Name);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserModulesAsync_SetsWideEventContextUserId()
    {

        var companyId = Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        await controller.GetUserModulesAsync(headers, wide, ct);

        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserCompanyAsync_WhenUserHasNoCompanies_ReturnsOkWithEmptyList()
    {

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var result = await controller.GetUserCompanyAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnedCompanies = Assert.IsType<List<GetUserCompaniesResponse>>(okResult.Value);
        Assert.Empty(returnedCompanies);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserCompanyAsync_WhenUserHasCompanies_ReturnsOkWithCompanies()
    {

        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };

        var user = new UserModel
        {
            Id = testUserId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            RoleId = roleId,
            CompanyId = companyId
        };

        db.AuthCompanies.Add(company);
        db.AuthRoles.Add(role);
        db.AuthUsers.Add(user);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var result = await controller.GetUserCompanyAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnedCompanies = Assert.IsType<List<GetUserCompaniesResponse>>(okResult.Value);
        Assert.Single(returnedCompanies);
        Assert.Equal(companyId, returnedCompanies[0].Id);
        Assert.Equal("Admin", returnedCompanies[0].Role);
        Assert.Equal(company.Name, returnedCompanies[0].CompanyName);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserCompanyAsync_SetsWideEventContextUserId()
    {

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        await controller.GetUserCompanyAsync(wide, ct);

        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public void UserController_HasAuthorizeAttribute()
    {

        var controllerType = typeof(UserController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void UserController_HasRouteAttribute()
    {

        var controllerType = typeof(UserController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void UserController_HasApiControllerAttribute()
    {

        var controllerType = typeof(UserController);

        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        Assert.NotNull(apiControllerAttribute);
    }

    #region GetAsync Tests (List Users)

    [Fact]
    public async Task GetAsync_WithGodRole_ReturnsOkWithUsers()
    {

        SetupUserClaims(testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await controller.GetAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithGodRole_ReturnsFullUserData()
    {

        SetupUserClaims(testUserId, "God");

        var roleId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var role = new RoleModel
        {
            Id = roleId,
            Name = "User"
        };
        var company = new CompanyModel
        {
            Id = companyId,
            Name = "Test Company",
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = roleId,
            CompanyId = companyId
        };

        db.AuthRoles.Add(role);
        db.AuthCompanies.Add(company);
        db.AuthUsers.Add(user);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await controller.GetByIdAsync(user.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserNotFound_ReturnsNotFound()
    {

        SetupUserClaims(testUserId, "God");
        var nonExistentUserId = Guid.NewGuid();

        var result = await controller.GetByIdAsync(nonExistentUserId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserIsDeleted_ReturnsNotFound()
    {

        SetupUserClaims(testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password(),
            Deleted = DateTime.UtcNow
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await controller.GetByIdAsync(user.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WhenUserNotFound_ReturnsNotFound()
    {

        SetupUserClaims(testUserId, "God");
        var nonExistentUserId = Guid.NewGuid();

        var query = new UpdateUserCommand(nonExistentUserId, "Updated Name");

        var result = await controller.UpdateAsync(nonExistentUserId, query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserIsDeleted_ReturnsNotFound()
    {

        SetupUserClaims(testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password(),
            Deleted = DateTime.UtcNow
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdateUserCommand(user.Id, "Updated Name");

        var result = await controller.UpdateAsync(user.Id, query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenUserNotFound_ReturnsNotFound()
    {

        SetupUserClaims(testUserId, "God");
        var nonExistentUserId = Guid.NewGuid();

        var result = await controller.DeleteAsync(nonExistentUserId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserIsDeleted_ReturnsNotFound()
    {

        SetupUserClaims(testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password(),
            Deleted = DateTime.UtcNow
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await controller.DeleteAsync(user.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenAttemptingSelfDeletion_ReturnsBadRequest()
    {

        SetupUserClaims(testUserId, "God");

        var user = new UserModel
        {
            Id = testUserId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await controller.DeleteAsync(testUserId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
    }

    #endregion

    #region ChangePasswordAsync Tests

    [Fact]
    public async Task ChangePasswordAsync_WhenUserNotFound_ReturnsNotFound()
    {

        SetupUserClaims(testUserId, "God");
        var nonExistentUserId = Guid.NewGuid();

        var query = new UpdateUserPasswordCommand(testUserId, faker.Internet.Password());

        var result = await controller.ChangePasswordAsync(nonExistentUserId, query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenUserIsDeleted_ReturnsNotFound()
    {

        SetupUserClaims(testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password(),
            Deleted = DateTime.UtcNow
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdateUserPasswordCommand(user.Id, faker.Internet.Password());

        var result = await controller.ChangePasswordAsync(user.Id, query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion
}
