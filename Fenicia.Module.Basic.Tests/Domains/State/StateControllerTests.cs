using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.State;
using Fenicia.Module.Basic.Domains.State.Handlers;
using Fenicia.Module.Basic.Domains.State.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.State;

public class StateControllerTests : IDisposable
{
    private readonly TestCompanyContext companyContext;
    private readonly StateController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetAllStateHandler getAllStateHandler;
    private readonly Mock<HttpContext> mockHttpContext;

    public StateControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        getAllStateHandler = new GetAllStateHandler(db);
        mockHttpContext = new Mock<HttpContext>();

        controller = new StateController(getAllStateHandler) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        SetupUserClaims();
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    private void SetupUserClaims()
    {
        var claims = new List<Claim> { new("userId", Guid.NewGuid().ToString()) };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task GetAllAsync_WhenNoStatesExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetAllAsync(wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedStates = okResult.Value as List<GetAllStateResponse>;
        Assert.NotNull(returnedStates);
        Assert.Empty(returnedStates);
    }

    [Fact]
    public async Task GetAllAsync_WhenStatesExist_ReturnsOkWithStates()
    {
        // Arrange
        var state1 = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Address.State(),
            Uf = faker.Address.StateAbbr()
        };

        var state2 = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Address.State(),
            Uf = faker.Address.StateAbbr()
        };

        db.AuthStates.AddRange(state1, state2);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetAllAsync(wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedStates = okResult.Value as List<GetAllStateResponse>;
        Assert.NotNull(returnedStates);
        Assert.Equal(2, returnedStates.Count);
    }

    [Fact]
    public async Task GetAllAsync_WhenStatesExist_ReturnsStatesOrderedByUf()
    {
        // Arrange
        var state1 = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };

        var state2 = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "Acre",
            Uf = "AC"
        };

        var state3 = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "Rio de Janeiro",
            Uf = "RJ"
        };

        db.AuthStates.AddRange(state1, state2, state3);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetAllAsync(wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedStates = okResult.Value as List<GetAllStateResponse>;
        Assert.NotNull(returnedStates);
        Assert.Equal(3, returnedStates.Count);
        Assert.Equal("AC", returnedStates[0].Uf);
        Assert.Equal("RJ", returnedStates[1].Uf);
        Assert.Equal("SP", returnedStates[2].Uf);
    }

    [Fact]
    public void StateController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(StateController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void StateController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(StateController);

        // Act
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void StateController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(StateController);

        // Act
        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }
}
