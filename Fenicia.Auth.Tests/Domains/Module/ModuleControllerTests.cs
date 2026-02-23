using Bogus;

using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Module.GetModules;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Module;

[TestFixture]
public class ModuleControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.getModulesHandler = new GetModulesHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();
        this.faker = new Faker();

        this.controller = new ModuleController
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

    private ModuleController controller = null!;
    private AuthContext context = null!;
    private GetModulesHandler getModulesHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Faker faker = null!;

    [Test]
    public async Task GetAllModulesAsync_WhenNoModulesExist_ReturnsOkWithEmptyPagination()
    {
        // Arrange
        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetAllModulesAsync(
            query,
            this.getModulesHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedPagination = okResult.Value as Pagination<List<GetModuleResponse>>;
        Assert.That(returnedPagination, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedPagination!.Data, Is.Empty);
            Assert.That(returnedPagination.Total, Is.Zero);
            Assert.That(wide.UserId, Is.EqualTo("Guest"));
        }
    }

    [Test]
    public async Task GetAllModulesAsync_WhenModulesExist_ReturnsOkWithPagination()
    {
        // Arrange
        var module1 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m
        };

        var module2 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.SocialNetwork,
            Price = 20.0m
        };

        this.context.Modules.AddRange(module1, module2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetAllModulesAsync(
            query,
            this.getModulesHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedPagination = okResult.Value as Pagination<List<GetModuleResponse>>;
        Assert.That(returnedPagination, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedPagination!.Data, Has.Count.EqualTo(2));
            Assert.That(returnedPagination.Total, Is.EqualTo(2));
            Assert.That(wide.UserId, Is.EqualTo("Guest"));
        }
    }

    [Test]
    public async Task GetAllModulesAsync_ExcludesErpAndAuthModuleTypes()
    {
        // Arrange
        var erpModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Erp,
            Price = 100.0m
        };

        var authModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Auth,
            Price = 50.0m
        };

        var basicModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m
        };

        this.context.Modules.AddRange(erpModule, authModule, basicModule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetAllModulesAsync(
            query,
            this.getModulesHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedPagination = okResult.Value as Pagination<List<GetModuleResponse>>;
        Assert.That(returnedPagination, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedPagination!.Data, Has.Count.EqualTo(1));
            Assert.That(returnedPagination.Data[0].Name, Is.EqualTo(basicModule.Name));
        }
    }

    [Test]
    public async Task GetAllModulesAsync_SetsWideEventContextUserIdToGuest()
    {
        // Arrange
        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        await this.controller.GetAllModulesAsync(
            query,
            this.getModulesHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(wide.UserId, Is.EqualTo("Guest"));
    }

    [Test]
    public async Task GetAllModulesAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var modules = new List<ModuleModel>();
        for (var i = 0; i < 25; i++)
        {
            modules.Add(new ModuleModel
            {
                Id = Guid.NewGuid(),
                Name = $"Module {this.faker.Commerce.ProductName()} {i}",
                Type = (ModuleType)(i % 10 + 1),
                Price = 10.0m
            });
        }

        this.context.Modules.AddRange(modules);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(2, 10);
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetAllModulesAsync(
            query,
            this.getModulesHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedPagination = okResult.Value as Pagination<List<GetModuleResponse>>;
        Assert.That(returnedPagination, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedPagination!.Data, Has.Count.EqualTo(10));
            Assert.That(returnedPagination.Total, Is.EqualTo(25));
            Assert.That(returnedPagination.Page, Is.EqualTo(2));
            Assert.That(returnedPagination.PerPage, Is.EqualTo(10));
        }
    }

    [Test]
    public void ModuleController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(ModuleController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "ModuleController should have Authorize attribute");
    }

    [Test]
    public void ModuleController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ModuleController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "ModuleController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void ModuleController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(ModuleController);

        // Act
        var producesAttribute =
            controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.That(producesAttribute, Is.Not.Null, "ModuleController should have Produces attribute");
        Assert.That(producesAttribute!.ContentTypes.FirstOrDefault(), Is.EqualTo("application/json"));
    }

    [Test]
    public void GetAllModulesAsync_HasAllowAnonymousAttribute()
    {
        // Arrange
        var controllerType = typeof(ModuleController);
        var methodInfo = controllerType.GetMethod(nameof(ModuleController.GetAllModulesAsync));

        // Act
        var allowAnonymousAttribute =
            methodInfo?.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(allowAnonymousAttribute, Is.Not.Null, "GetAllModulesAsync should have AllowAnonymous attribute");
    }
}