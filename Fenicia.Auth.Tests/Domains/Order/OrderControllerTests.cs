using System.Security.Claims;

using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.Order.Command;
using Fenicia.Auth.Domains.Order.Handler;
using Fenicia.Auth.Domains.Order.Response;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MediatR;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Order;

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
        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<CreateNewOrderCommand>(), It.IsAny<CancellationToken>()))
            .Returns((CreateNewOrderCommand command, CancellationToken token) => createNewOrderHandler.Handle(command, token));

        mockHttpContext = new Mock<HttpContext>();

        controller = new OrderController(mockSender.Object) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

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

    [Fact]
    public async Task CreateNewOrderAsync_WhenUserDoesNotBelongToCompany_ReturnsForbid()
    {

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var modules = new List<Guid> { Guid.NewGuid() };
        var command = new CreateNewOrderCommand(testUserId, testCompanyId, modules);
        var headers = new Headers { CompanyId = testCompanyId };

        var result = await controller.CreateNewOrderAsync(command, headers, wide, ct);

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task CreateNewOrderAsync_WhenModulesDoNotExist_ThrowsItemNotExistsException()
    {

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

        var result = await controller.CreateNewOrderAsync(command, headers, wide, ct);

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

        var result = await controller.CreateNewOrderAsync(command, headers, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedResponse = createdResult.Value as CreateNewOrderResponse;
        Assert.NotNull(returnedResponse);

        Assert.NotEqual(Guid.Empty, returnedResponse.OrderId);
        Assert.Equal(testUserId.ToString(), wide.UserId);

        var createdOrder = await db.AuthOrders.FirstOrDefaultAsync(o => o.Id == returnedResponse.OrderId, ct);
        Assert.NotNull(createdOrder);

        Assert.Equal(testUserId, createdOrder.UserId);
        Assert.Equal(testCompanyId, createdOrder.CompanyId);
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

        await controller.CreateNewOrderAsync(command, headers, wide, ct);

        Assert.Equal(testUserId.ToString(), wide.UserId);
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
}
