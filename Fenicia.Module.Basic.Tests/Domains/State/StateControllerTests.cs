using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.State;
using Fenicia.Module.Basic.Domains.State.GetAll;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.State;

[TestFixture]
public class StateControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
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

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private StateController controller = null!;
    private BasicContext context = null!;
    private GetAllStateHandler getAllStateHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Faker faker = null!;

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

    [Test]
    public async Task GetAllAsync_WhenNoStatesExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetAllAsync(cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedStates = okResult.Value as List<GetAllStateResponse>;
        Assert.That(returnedStates, Is.Not.Null);
        Assert.That(returnedStates, Is.Empty);
    }

    [Test]
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

        this.context.States.AddRange(state1, state2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetAllAsync(cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedStates = okResult.Value as List<GetAllStateResponse>;
        Assert.That(returnedStates, Is.Not.Null);
        Assert.That(returnedStates, Has.Count.EqualTo(2));
    }

    [Test]
    public void StateController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(StateController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "StateController should have Authorize attribute");
    }

    [Test]
    public void StateController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(StateController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "StateController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void StateController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(StateController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "StateController should have ApiController attribute");
    }
}
