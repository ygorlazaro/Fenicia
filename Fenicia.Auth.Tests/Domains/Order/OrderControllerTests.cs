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
using Fenicia.Common.Tests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Order;

/// <summary>
///     Unit tests for the OrderController in Auth module.
///     Tests the HTTP endpoint for creating module subscription orders.
/// </summary>
public class OrderControllerTests : IDisposable
{
    private readonly OrderController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testCompanyId;
    private readonly Guid testUserId;

    public OrderControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        testUserId = Guid.NewGuid();
        testCompanyId = Guid.NewGuid();
        var createNewOrderHandler = new CreateNewOrderHandler(db);
        mockHttpContext = new Mock<HttpContext>();

        controller = new OrderController(createNewOrderHandler) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        SetupUserClaims(testUserId);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    /// <summary>
    ///     Tests that a user not belonging to the company throws PermissionDeniedException.
    /// </summary>
    [Fact]
    public async Task CreateNewOrderAsync_WhenUserDoesNotBelongToCompany_ReturnsForbid()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var modules = new List<Guid> { Guid.NewGuid() };
        var command = new CreateNewOrderCommand(testUserId, testCompanyId, modules);
        var headers = new Headers { CompanyId = testCompanyId };

        // Act
        var result = await controller.CreateNewOrderAsync(command, headers, wide, ct);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);
    }

    /// <summary>
    ///     Tests that requesting non-existent modules throws ItemNotExistsException.
    /// </summary>
    [Fact]
    public async Task CreateNewOrderAsync_WhenModulesDoNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = testUserId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = testCompanyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            RoleId = Guid.NewGuid(),
            CompanyId = testCompanyId
        };

        db.AuthUsers.Add(user);
        db.AuthCompanies.Add(company);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var modules = new List<Guid> { Guid.NewGuid() };
        var command = new CreateNewOrderCommand(testUserId, testCompanyId, modules);
        var headers = new Headers { CompanyId = testCompanyId };

        // Act
        var result = await controller.CreateNewOrderAsync(command, headers, wide, ct);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    /// <summary>
    ///     Tests that a valid request returns OK with the created order.
    /// </summary>
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
            Name = faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = faker.Finance.Amount(10,
                100)
        };

        var user = new UserModel
        {
            Id = testUserId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = testCompanyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            RoleId = Guid.NewGuid(),
            CompanyId = testCompanyId
        };

        db.AuthModules.Add(module);
        db.AuthUsers.Add(user);
        db.AuthCompanies.Add(company);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var modules = new List<Guid> { moduleId };
        var command = new CreateNewOrderCommand(testUserId, testCompanyId, modules);
        var headers = new Headers { CompanyId = testCompanyId };

        // Act
        var result = await controller.CreateNewOrderAsync(command, headers, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedResponse = createdResult.Value as CreateNewOrderResponse;
        Assert.NotNull(returnedResponse);

        Assert.NotEqual(Guid.Empty, returnedResponse.OrderId);
        Assert.Equal(testUserId.ToString(), wide.UserId);

        // Verify order was created
        var createdOrder = await db.AuthOrders.FirstOrDefaultAsync(o => o.Id == returnedResponse.OrderId, ct);
        Assert.NotNull(createdOrder);

        Assert.Equal(testUserId, createdOrder.UserId);
        Assert.Equal(testCompanyId, createdOrder.CompanyId);
    }

    /// <summary>
    ///     Tests that WideEventContext UserId is set from the authenticated user.
    /// </summary>
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
            Name = faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = faker.Finance.Amount(10,
                100)
        };

        var user = new UserModel
        {
            Id = testUserId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = testCompanyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            RoleId = Guid.NewGuid(),
            CompanyId = testCompanyId
        };

        db.AuthModules.Add(module);
        db.AuthUsers.Add(user);
        db.AuthCompanies.Add(company);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var modules = new List<Guid> { moduleId };
        var command = new CreateNewOrderCommand(testUserId, testCompanyId, modules);
        var headers = new Headers { CompanyId = testCompanyId };

        // Act
        await controller.CreateNewOrderAsync(command, headers, wide, ct);

        // Assert
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    /// <summary>
    ///     Tests that the controller has the AuthorizeAttribute applied.
    /// </summary>
    [Fact]
    public void OrderController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(OrderController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    /// <summary>
    ///     Tests that the controller has the RouteAttribute with [controller] template.
    /// </summary>
    [Fact]
    public void OrderController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(OrderController);

        // Act
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    /// <summary>
    ///     Tests that the controller has the ProducesAttribute with application/json content type.
    /// </summary>
    [Fact]
    public void OrderController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(OrderController);

        // Act
        var producesAttribute = controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }
}
