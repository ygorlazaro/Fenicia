using Bogus;

using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Module.DTOs;
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

public class ModuleControllerTests : IDisposable
{
    private readonly ModuleController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;

    public ModuleControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        var services = new ServiceCollection();
        services.AddSingleton(_db);
        services.AddLogging();
        services.AddSingleton(new ModuleService(new ModuleRepository(_db)));

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<ModuleService>();

        var mockHttpContext = new Mock<HttpContext>();
        _faker = new Faker();

        _controller = new ModuleController(service) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllModulesAsync_WhenNoModulesExist_ReturnsOkWithEmptyPagination()
    {
        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var result = await _controller.GetAllModulesAsync(query, wide, ct);

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
        var module1 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 1
        };

        var module2 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.SocialNetwork,
            Price = 20.0m,
            IsActive = true,
            SortOrder = 2
        };

        _db.AuthModules.AddRange(module1, module2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var result = await _controller.GetAllModulesAsync(query, wide, ct);

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
        var authModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Auth,
            Price = 50.0m,
            IsActive = true,
            SortOrder = 1
        };

        var basicModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 2
        };

        _db.AuthModules.AddRange(authModule, basicModule);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var result = await _controller.GetAllModulesAsync(query, wide, ct);

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

        _db.AuthModules.AddRange(activeModule, inactiveModule);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var result = await _controller.GetAllModulesAsync(query, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPagination = okResult.Value as Pagination<List<GetModuleResponse>>;
        Assert.NotNull(returnedPagination);
        Assert.Single(returnedPagination.Data);
        Assert.Equal(activeModule.Name, returnedPagination.Data[0].Name);
    }

    [Fact]
    public async Task GetAllModulesAsync_SetsWideEventContextUserIdToGuest()
    {
        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        await _controller.GetAllModulesAsync(query, wide, ct);

        Assert.Equal("Guest", wide.UserId);
    }

    [Fact]
    public async Task GetAllModulesAsync_WithPagination_ReturnsCorrectPage()
    {
        var modules = new List<ModuleModel>();
        for (var i = 0; i < 25; i++)
        {
            modules.Add(new ModuleModel
            {
                Id = Guid.NewGuid(),
                Name = $"Module {_faker.Commerce.ProductName()} {i}",
                Type = (ModuleType)((i % 10) + 1),
                Price = 10.0m,
                IsActive = true,
                SortOrder = i
            });
        }

        _db.AuthModules.AddRange(modules);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(2, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var result = await _controller.GetAllModulesAsync(query, wide, ct);

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
        var controllerType = typeof(ModuleController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void ModuleController_HasRouteAttribute()
    {
        var controllerType = typeof(ModuleController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void ModuleController_HasProducesAttribute()
    {
        var controllerType = typeof(ModuleController);

        var producesAttribute = controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }

    [Fact]
    public void GetAllModulesAsync_HasAllowAnonymousAttribute()
    {
        var controllerType = typeof(ModuleController);
        var methodInfo = controllerType.GetMethod(nameof(ModuleController.GetAllModulesAsync));

        var allowAnonymousAttribute = methodInfo?.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        Assert.NotNull(allowAnonymousAttribute);
    }
}
