using Bogus;

using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Module.Handlers;
using Fenicia.Auth.Domains.Module.Responses;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Module;

public class ModuleControllerTests : IDisposable
{
    public ModuleControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options, new TestCompanyContext());
        var getModulesHandler = new GetModulesHandler(this.db);
        var mockHttpContext = new Mock<HttpContext>();
        this.faker = new Faker();

        this.controller = new ModuleController(getModulesHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            }
        };
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly ModuleController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;

    [Fact]
    public async Task GetAllModulesAsync_WhenNoModulesExist_ReturnsOkWithEmptyPagination()
    {
        // Arrange
        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetAllModulesAsync(
            query,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPagination = okResult.Value as Pagination<List<GetModuleResponse>>;
        Assert.NotNull(returnedPagination);
        Assert.Empty(returnedPagination.Data);
        Assert.Equal(0, returnedPagination.Total);
        Assert.Equal("Guest", wide.UserId);
    }

    [Fact]
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

        this.db.AuthModules.AddRange(module1, module2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetAllModulesAsync(
            query,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPagination = okResult.Value as Pagination<List<GetModuleResponse>>;
        Assert.NotNull(returnedPagination);
        Assert.Equal(2, returnedPagination.Data.Count);
        Assert.Equal(2, returnedPagination.Total);
        Assert.Equal("Guest", wide.UserId);
    }

    [Fact]
    public async Task GetAllModulesAsync_ExcludesErpAndAuthModuleTypes()
    {
        // Arrange
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

        this.db.AuthModules.AddRange(authModule, basicModule);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetAllModulesAsync(
            query,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPagination = okResult.Value as Pagination<List<GetModuleResponse>>;
        Assert.NotNull(returnedPagination);
        Assert.Single(returnedPagination.Data);
        Assert.Equal(basicModule.Name, returnedPagination.Data[0].Name);
    }

    [Fact]
    public async Task GetAllModulesAsync_SetsWideEventContextUserIdToGuest()
    {
        // Arrange
        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        await this.controller.GetAllModulesAsync(
            query,
            wide,
            ct);

        // Assert
        Assert.Equal("Guest", wide.UserId);
    }

    [Fact]
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

        this.db.AuthModules.AddRange(modules);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(2, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetAllModulesAsync(
            query,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPagination = okResult.Value as Pagination<List<GetModuleResponse>>;
        Assert.NotNull(returnedPagination);
        Assert.Equal(10, returnedPagination.Data.Count);
        Assert.Equal(25, returnedPagination.Total);
        Assert.Equal(2, returnedPagination.Page);
        Assert.Equal(10, returnedPagination.PerPage);
    }

    [Fact]
    public void ModuleController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(ModuleController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void ModuleController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ModuleController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void ModuleController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(ModuleController);

        // Act
        var producesAttribute =
            controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }

    [Fact]
    public void GetAllModulesAsync_HasAllowAnonymousAttribute()
    {
        // Arrange
        var controllerType = typeof(ModuleController);
        var methodInfo = controllerType.GetMethod(nameof(ModuleController.GetAllModulesAsync));

        // Act
        var allowAnonymousAttribute =
            methodInfo?.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(allowAnonymousAttribute);
    }
}
