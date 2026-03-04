using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.DataSource;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.DataSource;

[TestFixture]
public class DataSourceControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.getAllPositionForDataSourceHandler = new GetAllPositionForDataSourceHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new DataSourceController(this.getAllPositionForDataSourceHandler)
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

    private TestCompanyContext companyContext = null!;
    private DataSourceController controller = null!;
    private DefaultContext context = null!;
    private GetAllPositionForDataSourceHandler getAllPositionForDataSourceHandler = null!;
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
    public async Task GetPositionsAsync_WhenNoPositionsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetPositionsAsync(cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedPositions = okResult.Value as List<GetAllPositionForDataSourceResponse>;
        Assert.That(returnedPositions, Is.Not.Null);
        Assert.That(returnedPositions, Is.Empty);
    }

    [Test]
    public async Task GetPositionsAsync_WhenPositionsExist_ReturnsOkWithPositions()
    {
        // Arrange
        var position1 = new BasicPositionModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Department()
        };

        var position2 = new BasicPositionModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Department()
        };

        this.context.BasicPositions.AddRange(position1, position2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetPositionsAsync(cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedPositions = okResult.Value as List<GetAllPositionForDataSourceResponse>;
        Assert.That(returnedPositions, Is.Not.Null);
        Assert.That(returnedPositions, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetPositionsAsync_WhenPositionsExist_ReturnsPositionsOrderedByName()
    {
        // Arrange
        var position1 = new BasicPositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Zebra"
        };

        var position2 = new BasicPositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Alpha"
        };

        var position3 = new BasicPositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Manager"
        };

        this.context.BasicPositions.AddRange(position1, position2, position3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetPositionsAsync(cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedPositions = okResult.Value as List<GetAllPositionForDataSourceResponse>;
        Assert.That(returnedPositions, Is.Not.Null);
        Assert.That(returnedPositions, Has.Count.EqualTo(3));
        Assert.That(returnedPositions[0].Name, Is.EqualTo("Alpha"));
        Assert.That(returnedPositions[1].Name, Is.EqualTo("Manager"));
        Assert.That(returnedPositions[2].Name, Is.EqualTo("Zebra"));
    }

    [Test]
    public void DataSourceController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(DataSourceController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "DataSourceController should have Authorize attribute");
    }

    [Test]
    public void DataSourceController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(DataSourceController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "DataSourceController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void DataSourceController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(DataSourceController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "DataSourceController should have ApiController attribute");
    }
}
