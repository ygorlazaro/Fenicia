using Fenicia.Auth.Domains.Register;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Register;

/// <summary>
///     Unit tests for the RegisterController.
///     Tests user registration with company creation.
/// </summary>
/// <remarks>
///     These tests verify:
///     - Duplicate email validation
///     - Duplicate CNPJ validation
///     - Admin role existence validation
///     - Successful user and company creation
///     - WideEventContext UserId setting
///     - Controller attributes
/// </remarks>
public class RegisterControllerTests : IDisposable
{
    private readonly Guid adminRoleId;

    private readonly RegisterController controller;
    private readonly DefaultContext db;

    public RegisterControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        var createNewUserHandler = new CreateNewUserHandler(db);

        var mockHttpContext = new Mock<HttpContext>();

        controller = new RegisterController(createNewUserHandler) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        adminRoleId = Guid.NewGuid();
        SeedAdminRole();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    private void SeedAdminRole()
    {
        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };
        db.AuthRoles.Add(adminRole);
        db.SaveChanges();
    }

    /// <summary>
    ///     Tests that registering with an existing email throws exception.
    /// </summary>
    [Fact]
    public async Task CreateNewUserAsync_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        // Arrange
        var wide = new WideEventContext();
        var xrt = CancellationToken.None;

        var companyQuery = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Company Name");
        var query = new CreateNewUserCommand("existing@example.com", "password123", "Test User", companyQuery);

        var existingUser = new UserModel
        {
            Email = query.Email,
            Name = "Existing User",
            Password = "password"
        };
        db.AuthUsers.Add(existingUser);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await controller.CreateNewUserAsync(query, wide, xrt);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    /// <summary>
    ///     Tests that registering with an existing company CNPJ throws exception.
    /// </summary>
    [Fact]
    public async Task CreateNewUserAsync_WhenCompanyAlreadyExists_ThrowsArgumentException()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var companyQuery = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Existing Company");
        var query = new CreateNewUserCommand("test@example.com", "password123", "Test User", companyQuery);

        var existingCompany = new CompanyModel
        {
            Cnpj = companyQuery.Cnpj,
            Name = "Existing Company"
        };
        db.AuthCompanies.Add(existingCompany);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await controller.CreateNewUserAsync(query, wide, ct);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    /// <summary>
    ///     Tests that registering when Admin role doesn't exist throws exception.
    /// </summary>
    [Fact]
    public async Task CreateNewUserAsync_WhenAdminRoleDoesNotExist_ReturnsBadRequest()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var companyQuery = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Company Name");
        var query = new CreateNewUserCommand("test@example.com", "password123", "Test User", companyQuery);

        var adminRole = db.AuthRoles.First();
        db.AuthRoles.Remove(adminRole);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await controller.CreateNewUserAsync(query, wide, ct);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    /// <summary>
    ///     Tests that valid registration creates user, company, and role assignment.
    /// </summary>
    [Fact]
    public async Task CreateNewUserAsync_WhenValidRequest_ReturnsCreatedWithUser()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var companyQuery = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Company Name");
        var query = new CreateNewUserCommand("test@example.com", "password123", "Test User", companyQuery);

        // Act
        var result = await controller.CreateNewUserAsync(query, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedResponse = createdResult.Value as CreateNewUserResponse;
        Assert.NotNull(returnedResponse);
        Assert.Equal(query.Email, returnedResponse.Email);
        Assert.Equal(query.Name, returnedResponse.Name);
        Assert.Equal(companyQuery.Name, returnedResponse.Company.Name);
        Assert.Equal(query.Email, wide.UserId);

        // Verify user was created
        var createdUser = await db.AuthUsers.FirstOrDefaultAsync(u => u.Email == query.Email, ct);
        Assert.NotNull(createdUser);
        Assert.NotEqual(query.Password, createdUser.Password);
        Assert.StartsWith("$2a$", createdUser.Password);

        // Verify company was created
        var createdCompany = await db.AuthCompanies.FirstOrDefaultAsync(c => c.Cnpj == companyQuery.Cnpj, ct);
        Assert.NotNull(createdCompany);
        Assert.Equal(companyQuery.Name, createdCompany.Name);

        // Verify user role was created
        var userRole = await db.AuthUserRoles.FirstOrDefaultAsync(ur => ur.UserId == createdUser.Id, ct);
        Assert.NotNull(userRole);
        Assert.Equal(adminRoleId, userRole.RoleId);
    }

    [Fact]
    public async Task CreateNewUserAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var companyQuery = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Company Name");
        var query = new CreateNewUserCommand("test@example.com", "password123", "Test User", companyQuery);

        // Act
        await controller.CreateNewUserAsync(query, wide, ct);

        // Assert
        Assert.Equal(query.Email, wide.UserId);
    }

    [Fact]
    public void RegisterController_HasAllowAnonymousAttribute()
    {
        // Arrange
        var controllerType = typeof(RegisterController);

        // Act
        var allowAnonymousAttribute = controllerType.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(allowAnonymousAttribute);
    }

    [Fact]
    public void RegisterController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(RegisterController);

        // Act
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

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
        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public void RegisterController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(RegisterController);

        // Act
        var producesAttribute = controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }
}
