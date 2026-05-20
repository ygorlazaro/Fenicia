using Bogus;

using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Module.Handlers;
using Fenicia.Auth.Domains.Module.Responses;

using MediatR;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Module;

/// <summary>
///     Unit tests for the ModuleController.
///     Tests the HTTP endpoint for retrieving modules.
/// </summary>
/// <remarks>
///     These tests verify the core functionality of the module endpoint:
///     - Returns correct pagination responses
///     - Excludes Auth type modules
///     - Sets WideEventContext UserId to "Guest"
///     - Controller has correct attributes (Authorize, Route, Produces)
///     - Endpoint has AllowAnonymous attribute
/// </remarks>
public class ModuleControllerTests : IDisposable
{
    private readonly ModuleController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;

    public ModuleControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        var services = new ServiceCollection();
        services.AddSingleton(db);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetModulesHandler>());

        var provider = services.BuildServiceProvider();
        var sender = provider.GetRequiredService<ISender>();

        var mockHttpContext = new Mock<HttpContext>();
        faker = new Faker();

        controller = new ModuleController(sender) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Tests that when no modules exist, the endpoint returns OK with empty pagination.
    /// </summary>
    [Fact]
    public async Task GetAllModulesAsync_WhenNoModulesExist_ReturnsOkWithEmptyPagination()
    {
        // Arrange
        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await controller.GetAllModulesAsync(query, wide, ct);

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

    /// <summary>
    ///     Tests that when modules exist, they are returned with correct pagination.
    /// </summary>
    [Fact]
    public async Task GetAllModulesAsync_WhenModulesExist_ReturnsOkWithPagination()
    {
        // Arrange
        var module1 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 1
        };

        var module2 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            Type = ModuleType.SocialNetwork,
            Price = 20.0m,
            IsActive = true,
            SortOrder = 2
        };

        db.AuthModules.AddRange(module1, module2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await controller.GetAllModulesAsync(query, wide, ct);

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

    /// <summary>
    ///     Tests that Auth module type is excluded from results.
    /// </summary>
    [Fact]
    public async Task GetAllModulesAsync_ExcludesErpAndAuthModuleTypes()
    {
        // Arrange
        var authModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            Type = ModuleType.Auth,
            Price = 50.0m,
            IsActive = true,
            SortOrder = 1
        };

        var basicModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 2
        };

        db.AuthModules.AddRange(authModule, basicModule);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await controller.GetAllModulesAsync(query, wide, ct);

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
    public async Task GetAllModulesAsync_ExcludesInactiveModules()
    {
        // Arrange
        var activeModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Active Module",
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 1
        };

        var inactiveModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Inactive Module",
            Type = ModuleType.SocialNetwork,
            Price = 20.0m,
            IsActive = false,
            SortOrder = 2
        };

        db.AuthModules.AddRange(activeModule, inactiveModule);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await controller.GetAllModulesAsync(query, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPagination = okResult.Value as Pagination<List<GetModuleResponse>>;
        Assert.NotNull(returnedPagination);
        Assert.Single(returnedPagination.Data);
        Assert.Equal(activeModule.Name, returnedPagination.Data[0].Name);
    }

    /// <summary>
    ///     Tests that WideEventContext UserId is set to "Guest" for unauthenticated requests.
    /// </summary>
    [Fact]
    public async Task GetAllModulesAsync_SetsWideEventContextUserIdToGuest()
    {
        // Arrange
        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        await controller.GetAllModulesAsync(query, wide, ct);

        // Assert
        Assert.Equal("Guest", wide.UserId);
    }

    /// <summary>
    ///     Tests that pagination parameters are applied correctly.
    /// </summary>
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
                Name = $"Module {faker.Commerce.ProductName()} {i}",
                Type = (ModuleType)(i % 10 + 1),
                Price = 10.0m,
                IsActive = true,
                SortOrder = i
            });
        }

        db.AuthModules.AddRange(modules);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(2, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await controller.GetAllModulesAsync(query, wide, ct);

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

    /// <summary>
    ///     Tests that the controller has the AuthorizeAttribute applied.
    /// </summary>
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

    /// <summary>
    ///     Tests that the controller has the RouteAttribute with [controller] template.
    /// </summary>
    [Fact]
    public void ModuleController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ModuleController);

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
    public void ModuleController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(ModuleController);

        // Act
        var producesAttribute = controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }

    /// <summary>
    ///     Tests that the GetAllModulesAsync method has AllowAnonymousAttribute for public access.
    /// </summary>
    [Fact]
    public void GetAllModulesAsync_HasAllowAnonymousAttribute()
    {
        // Arrange
        var controllerType = typeof(ModuleController);
        var methodInfo = controllerType.GetMethod(nameof(ModuleController.GetAllModulesAsync));

        // Act
        var allowAnonymousAttribute = methodInfo?.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(allowAnonymousAttribute);
    }
}
