using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Module.Basic.Domains.State;
using Fenicia.Module.Basic.Domains.State.GetAll;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.State;

public class StateControllerTests : IDisposable
{
    public StateControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.getAllStateHandler = new GetAllStateHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new StateController(this.getAllStateHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims();
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.context.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly TestCompanyContext companyContext;
    private readonly StateController controller;
    private readonly DefaultContext context;
    private readonly GetAllStateHandler getAllStateHandler;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Faker faker;

    private void SetupUserClaims()
    {
        var claims = new List<Claim>
        {
            new("userId", Guid.NewGuid().ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task GetAllAsync_WhenNoStatesExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetAllAsync(wide, ct);

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
            Name = this.faker.Address.State(),
            Uf = this.faker.Address.StateAbbr()
        };

        var state2 = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Address.State(),
            Uf = this.faker.Address.StateAbbr()
        };

        this.context.AuthStates.AddRange(state1, state2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetAllAsync(wide, ct);

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

        this.context.AuthStates.AddRange(state1, state2, state3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetAllAsync(wide, ct);

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
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

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
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }
}
