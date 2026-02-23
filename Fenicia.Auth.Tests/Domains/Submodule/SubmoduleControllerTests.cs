using Fenicia.Auth.Domains.Submodule;
using Fenicia.Auth.Domains.Submodule.GetByModuleId;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Submodule;

[TestFixture]
public class SubmoduleControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.testModuleId = Guid.NewGuid();
        this.getByModuleIdHandler = new GetByModuleIdHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new SubmoduleController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private SubmoduleController controller = null!;
    private AuthContext context = null!;
    private GetByModuleIdHandler getByModuleIdHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testModuleId;

    [Test]
    public async Task GetByModuleIdAsync_WhenNoSubmodulesExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByModuleIdAsync(
            this.testModuleId,
            this.getByModuleIdHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedSubmodules = okResult.Value as List<GetByModuleResponse>;
        Assert.That(returnedSubmodules, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedSubmodules.Count, Is.Zero);
            Assert.That(wide.UserId, Is.EqualTo("Guest"));
        }
    }

    [Test]
    public async Task GetByModuleIdAsync_WhenSubmodulesExist_ReturnsOkWithSubmodules()
    {
        // Arrange
        var submodule1 = new SubmoduleModel
        {
            Id = Guid.NewGuid(),
            Name = "Submodule 1",
            Description = "Description 1",
            ModuleId = this.testModuleId,
            Route = "/api/submodule1"
        };

        var submodule2 = new SubmoduleModel
        {
            Id = Guid.NewGuid(),
            Name = "Submodule 2",
            Description = "Description 2",
            ModuleId = this.testModuleId,
            Route = "/api/submodule2"
        };

        this.context.Submodules.AddRange(submodule1, submodule2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByModuleIdAsync(
            this.testModuleId,
            this.getByModuleIdHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedSubmodules = okResult.Value as List<GetByModuleResponse>;
        Assert.That(returnedSubmodules, Is.Not.Null);
        Assert.That(returnedSubmodules, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedSubmodules[0].ModuleId, Is.EqualTo(this.testModuleId));
            Assert.That(wide.UserId, Is.EqualTo("Guest"));
        }
    }

    [Test]
    public async Task GetByModuleIdAsync_WhenSubmodulesForDifferentModule_ReturnsEmptyList()
    {
        // Arrange
        var differentModuleId = Guid.NewGuid();

        var submodule = new SubmoduleModel
        {
            Id = Guid.NewGuid(),
            Name = "Submodule for different module",
            Description = "Description",
            ModuleId = differentModuleId,
            Route = "/api/submodule"
        };

        this.context.Submodules.Add(submodule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByModuleIdAsync(
            this.testModuleId,
            this.getByModuleIdHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedSubmodules = okResult.Value as List<GetByModuleResponse>;
        Assert.That(returnedSubmodules, Is.Not.Null);
        Assert.That(returnedSubmodules.Count, Is.Zero);
    }

    [Test]
    public async Task GetByModuleIdAsync_SetsWideEventContextUserIdToGuest()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        await this.controller.GetByModuleIdAsync(
            this.testModuleId,
            this.getByModuleIdHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(wide.UserId, Is.EqualTo("Guest"));
    }

    [Test]
    public void SubmoduleController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(SubmoduleController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "SubmoduleController should have Authorize attribute");
    }

    [Test]
    public void SubmoduleController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(SubmoduleController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "SubmoduleController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void SubmoduleController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(SubmoduleController);

        // Act
        var producesAttribute =
            controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.That(producesAttribute, Is.Not.Null, "SubmoduleController should have Produces attribute");
        Assert.That(producesAttribute!.ContentTypes.FirstOrDefault(), Is.EqualTo("application/json"));
    }

    [Test]
    public void SubmoduleController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(SubmoduleController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "SubmoduleController should have ApiController attribute");
    }

    [Test]
    public void GetByModuleIdAsync_HasAllowAnonymousAttribute()
    {
        // Arrange
        var controllerType = typeof(SubmoduleController);
        var methodInfo = controllerType.GetMethod(nameof(SubmoduleController.GetByModuleIdAsync));

        // Act
        var allowAnonymousAttribute =
            methodInfo?.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(allowAnonymousAttribute, Is.Not.Null, "GetByModuleIdAsync should have AllowAnonymous attribute");
    }
}