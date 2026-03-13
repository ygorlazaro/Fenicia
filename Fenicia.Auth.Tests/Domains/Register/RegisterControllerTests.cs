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

public class RegisterControllerTests : IDisposable
{
    public RegisterControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options,
            new TestCompanyContext());
        var createNewUserHandler = new CreateNewUserHandler(this.db);

        var mockHttpContext = new Mock<HttpContext>();

        this.controller = new RegisterController(createNewUserHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            }
        };
        this.adminRoleId = Guid.NewGuid();
        SeedAdminRole();
    }

    private void SeedAdminRole()
    {
        var adminRole = new RoleModel { Id = this.adminRoleId, Name = "Admin" };
        this.db.AuthRoles.Add(adminRole); 
        this.db.SaveChanges();
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly RegisterController controller;
    private readonly DefaultContext db;
    private readonly Guid adminRoleId;

    [Fact]
    public async Task CreateNewUserAsync_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        // Arrange
        var wide = new WideEventContext();
        var xrt = CancellationToken.None;

        var companyQuery = new CreateNewUserCompanyCommand("12.345.678/0001-90",
            "Company Name");
        var query = new CreateNewUserCommand("existing@example.com",
            "password123",
            "Test User",
            companyQuery);

        var existingUser = new UserModel { Email = query.Email, Name = "Existing User", Password = "password" };
        this.db.AuthUsers.Add(existingUser);
        await this.db.SaveChangesAsync(CancellationToken.None);

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

        var companyQuery = new CreateNewUserCompanyCommand("12.345.678/0001-90",
            "Existing Company");
        var query = new CreateNewUserCommand("test@example.com",
            "password123",
            "Test User",
            companyQuery);

        var existingCompany = new CompanyModel { Cnpj = companyQuery.Cnpj, Name = "Existing Company" };
        this.db.AuthCompanies.Add(existingCompany);
        await this.db.SaveChangesAsync(CancellationToken.None);

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

        var companyQuery = new CreateNewUserCompanyCommand("12.345.678/0001-90",
            "Company Name");
        var query = new CreateNewUserCommand("test@example.com",
            "password123",
            "Test User",
            companyQuery);

        var adminRole = this.db.AuthRoles.First();
        this.db.AuthRoles.Remove(adminRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

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

        var companyQuery = new CreateNewUserCompanyCommand("12.345.678/0001-90",
            "Company Name");
        var query = new CreateNewUserCommand("test@example.com",
            "password123",
            "Test User",
            companyQuery);

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
        Assert.Equal(200,
            okResult.StatusCode);

        var returnedResponse = okResult.Value as CreateNewUserResponse;
        Assert.NotNull(returnedResponse);
        Assert.Equal(query.Email,
            returnedResponse.Email);
        Assert.Equal(query.Name,
            returnedResponse.Name);
        Assert.Equal(companyQuery.Name,
            returnedResponse.Company.Name);
        Assert.Equal(query.Email,
            wide.UserId);

        // Verify user was created
        var createdUser = await this.db.AuthUsers.FirstOrDefaultAsync(u => u.Email == query.Email,
            ct);
        Assert.NotNull(createdUser);
        Assert.NotEqual(query.Password,
            createdUser.Password);
        Assert.StartsWith("$2a$",
            createdUser.Password);

        // Verify company was created
        var createdCompany = await this.db.AuthCompanies.FirstOrDefaultAsync(c => c.Cnpj == companyQuery.Cnpj,
            ct);
        Assert.NotNull(createdCompany);
        Assert.Equal(companyQuery.Name,
            createdCompany.Name);

        // Verify user role was created
        var userRole = await this.db.AuthUserRoles.FirstOrDefaultAsync(ur => ur.UserId == createdUser.Id,
            ct);
        Assert.NotNull(userRole);
        Assert.Equal(this.adminRoleId,
            userRole.RoleId);
    }

    [Fact]
    public async Task CreateNewUserAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var companyQuery = new CreateNewUserCompanyCommand("12.345.678/0001-90",
            "Company Name");
        var query = new CreateNewUserCommand("test@example.com",
            "password123",
            "Test User",
            companyQuery);

        // Act
        await this.controller.CreateNewUserAsync(
            query,
            wide,
            ct);

        // Assert
        Assert.Equal(query.Email,
            wide.UserId);
    }

    [Fact]
    public void RegisterController_HasAllowAnonymousAttribute()
    {
        // Arrange
        var controllerType = typeof(RegisterController);

        // Act
        var allowAnonymousAttribute =
            controllerType.GetCustomAttributes(typeof(AllowAnonymousAttribute),
                false).FirstOrDefault();

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
            controllerType.GetCustomAttributes(typeof(RouteAttribute),
                false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]",
            routeAttribute.Template);
    }

    [Fact]
    public void RegisterController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(RegisterController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute),
                false).FirstOrDefault();

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
            controllerType.GetCustomAttributes(typeof(ProducesAttribute),
                false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json",
            producesAttribute.ContentTypes.FirstOrDefault());
    }
}
