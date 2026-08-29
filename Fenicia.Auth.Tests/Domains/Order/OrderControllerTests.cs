using System.Security.Claims;

using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.Order.DTOs;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
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

public class OrderControllerTests : IDisposable
{
    private readonly OrderController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testCompanyId;
    private readonly Guid _testUserId;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly UserRoleService _userRoleService;

    public OrderControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _testUserId = Guid.NewGuid();
        _testCompanyId = Guid.NewGuid();
        _userRoleRepository = new UserRoleRepository(_db);
        _userRoleService = new UserRoleService(_userRoleRepository);
        var moduleRepository = new ModuleRepository(_db);
        var moduleService = new ModuleService(moduleRepository);
        var orderRepository = new OrderRepository(_db);
        var subscriptionRepository = new SubscriptionRepository(_db);
        var subscriptionService = new SubscriptionService(subscriptionRepository, new UserRepository(_db));
        var orderService = new OrderService(moduleService, orderRepository, subscriptionService, _userRoleService);

        _mockHttpContext = new Mock<HttpContext>();

        _controller = new OrderController(orderService) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };

        SetupUserClaims(_testUserId);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task CreateNewOrderAsync_WhenUserDoesNotBelongToCompany_ReturnsForbid()
    {
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var modules = new List<Guid> { Guid.NewGuid() };
        var command = new CreateNewOrderCommand(_testUserId, _testCompanyId, modules);
        var headers = new Headers { CompanyId = _testCompanyId };

        var result = await _controller.CreateNewOrderAsync(command, headers, wide, ct);

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task CreateNewOrderAsync_WhenModulesDoNotExist_ThrowsItemNotExistsException()
    {
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = _testUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = _testCompanyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            RoleId = Guid.NewGuid(),
            CompanyId = _testCompanyId
        };

        _db.AuthUsers.Add(user);
        _db.AuthCompanies.Add(company);
        _db.AuthUserRoles.Add(userRole);
        _db.SaveChanges();

        var modules = new List<Guid> { Guid.NewGuid() };
        var command = new CreateNewOrderCommand(_testUserId, _testCompanyId, modules);
        var headers = new Headers { CompanyId = _testCompanyId };

        var result = await _controller.CreateNewOrderAsync(command, headers, wide, ct);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateNewOrderAsync_WhenValidRequest_ReturnsOkWithOrder()
    {
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var moduleId = Guid.NewGuid();
        var module = new ModuleModel
        {
            Id = moduleId,
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = _faker.Finance.Amount(10, 100)
        };

        var user = new UserModel
        {
            Id = _testUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = _testCompanyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            RoleId = Guid.NewGuid(),
            CompanyId = _testCompanyId
        };

        _db.AuthModules.Add(module);
        _db.AuthUsers.Add(user);
        _db.AuthCompanies.Add(company);
        _db.AuthUserRoles.Add(userRole);
        _db.SaveChanges();

        var modules = new List<Guid> { moduleId };
        var command = new CreateNewOrderCommand(_testUserId, _testCompanyId, modules);
        var headers = new Headers { CompanyId = _testCompanyId };

        var result = await _controller.CreateNewOrderAsync(command, headers, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedResponse = createdResult.Value as CreateNewOrderResponse;
        Assert.NotNull(returnedResponse);

        Assert.NotEqual(Guid.Empty, returnedResponse.OrderId);
        Assert.Equal(_testUserId.ToString(), wide.UserId);

        var createdOrder = await _db.AuthOrders.FirstOrDefaultAsync(o => o.Id == returnedResponse.OrderId, ct);
        Assert.NotNull(createdOrder);

        Assert.Equal(_testUserId, createdOrder.UserId);
        Assert.Equal(_testCompanyId, createdOrder.CompanyId);
    }

    [Fact]
    public async Task CreateNewOrderAsync_SetsWideEventContextUserId()
    {
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var moduleId = Guid.NewGuid();
        var module = new ModuleModel
        {
            Id = moduleId,
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = _faker.Finance.Amount(10, 100)
        };

        var user = new UserModel
        {
            Id = _testUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = _testCompanyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            RoleId = Guid.NewGuid(),
            CompanyId = _testCompanyId
        };

        _db.AuthModules.Add(module);
        _db.AuthUsers.Add(user);
        _db.AuthCompanies.Add(company);
        _db.AuthUserRoles.Add(userRole);
        _db.SaveChanges();

        var modules = new List<Guid> { moduleId };
        var command = new CreateNewOrderCommand(_testUserId, _testCompanyId, modules);
        var headers = new Headers { CompanyId = _testCompanyId };

        await _controller.CreateNewOrderAsync(command, headers, wide, ct);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public void OrderController_HasAuthorizeAttribute()
    {
        var controllerType = typeof(OrderController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void OrderController_HasRouteAttribute()
    {
        var controllerType = typeof(OrderController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void OrderController_HasProducesAttribute()
    {
        var controllerType = typeof(OrderController);

        var producesAttribute = controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}
