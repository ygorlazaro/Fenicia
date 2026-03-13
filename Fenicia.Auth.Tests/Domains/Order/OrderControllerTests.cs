using System.Security.Claims;

using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.Order.CreateNewOrder.Commands;
using Fenicia.Auth.Domains.Order.CreateNewOrder.Handlers;
using Fenicia.Auth.Domains.Order.CreateNewOrder.Responses;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Order;

public class OrderControllerTests : IDisposable
{
    private readonly OrderController controller;
    private readonly DefaultContext db;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId;
    private readonly Guid testCompanyId;
    private readonly Faker faker;

    public OrderControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options, new TestCompanyContext());
        this.testUserId = Guid.NewGuid();
        this.testCompanyId = Guid.NewGuid();
        var createNewOrderHandler = new CreateNewOrderHandler(this.db);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new OrderController(createNewOrderHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims(this.testUserId);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new("userId",
                userId.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims,
            "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task CreateNewOrderAsync_WhenUserDoesNotBelongToCompany_ThrowsPermissionDeniedException()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var modules = new List<Guid> { Guid.NewGuid() };
        var command = new CreateNewOrderCommand(this.testUserId,
            this.testCompanyId,
            modules);
        var headers = new Headers { CompanyId = this.testCompanyId };

        // Act & Assert
        await Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.controller.CreateNewOrderAsync(
                command,
                headers,
                wide,
                ct));
    }

    [Fact]
    public async Task CreateNewOrderAsync_WhenModulesDoNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = this.testCompanyId,
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            RoleId = Guid.NewGuid(),
            CompanyId = this.testCompanyId
        };

        this.db.AuthUsers.Add(user);
        this.db.AuthCompanies.Add(company);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var modules = new List<Guid> { Guid.NewGuid() };
        var command = new CreateNewOrderCommand(this.testUserId,
            this.testCompanyId,
            modules);
        var headers = new Headers { CompanyId = this.testCompanyId };

        // Act & Assert
        await Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.controller.CreateNewOrderAsync(
                command,
                headers,
                wide,
                ct));
    }

    [Fact]
    public async Task CreateNewOrderAsync_WhenValidRequest_ReturnsOkWithOrder()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var moduleId = Guid.NewGuid();
        var module = new ModuleModel
        {
            Id = moduleId,
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = this.faker.Finance.Amount(10,
                100)
        };

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = this.testCompanyId,
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            RoleId = Guid.NewGuid(),
            CompanyId = this.testCompanyId
        };

        this.db.AuthModules.Add(module);
        this.db.AuthUsers.Add(user);
        this.db.AuthCompanies.Add(company);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var modules = new List<Guid> { moduleId };
        var command = new CreateNewOrderCommand(this.testUserId,
            this.testCompanyId,
            modules);
        var headers = new Headers { CompanyId = this.testCompanyId };

        // Act
        var result = await this.controller.CreateNewOrderAsync(
            command,
            headers,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200,
            okResult.StatusCode);

        var returnedResponse = okResult.Value as CreateNewOrderResponse;
        Assert.NotNull(returnedResponse);
        
        Assert.NotEqual(Guid.Empty,
            returnedResponse.OrderId);
        Assert.Equal(this.testUserId.ToString(),
            wide.UserId);

        // Verify order was created
        var createdOrder =
            await this.db.AuthOrders.FirstOrDefaultAsync(o => o.Id == returnedResponse.OrderId,
                ct);
        Assert.NotNull(createdOrder);
        
        Assert.Equal(this.testUserId,
            createdOrder.UserId);
        Assert.Equal(this.testCompanyId,
            createdOrder.CompanyId);
    }

    [Fact]
    public async Task CreateNewOrderAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var moduleId = Guid.NewGuid();
        var module = new ModuleModel
        {
            Id = moduleId,
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = this.faker.Finance.Amount(10,
                100)
        };

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = this.testCompanyId,
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            RoleId = Guid.NewGuid(),
            CompanyId = this.testCompanyId
        };

        this.db.AuthModules.Add(module);
        this.db.AuthUsers.Add(user);
        this.db.AuthCompanies.Add(company);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var modules = new List<Guid> { moduleId };
        var command = new CreateNewOrderCommand(this.testUserId,
            this.testCompanyId,
            modules);
        var headers = new Headers { CompanyId = this.testCompanyId };

        // Act
        await this.controller.CreateNewOrderAsync(
            command,
            headers,
            wide,
            ct);

        // Assert
        Assert.Equal(this.testUserId.ToString(),
            wide.UserId);
    }

    [Fact]
    public void OrderController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(OrderController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute),
            false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void OrderController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(OrderController);

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
    public void OrderController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(OrderController);

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
