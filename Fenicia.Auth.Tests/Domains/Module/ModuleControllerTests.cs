using Bogus;
using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Module;
using Fenicia.Common.Enums.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Module;

public class ModuleControllerTests
{
    private readonly ModuleController _controller;
    private readonly Faker _faker;
    private readonly Mock<IModuleService> _mockService;

    public ModuleControllerTests()
    {
        _mockService = new Mock<IModuleService>();
        var mockHttpContext = new Mock<HttpContext>();
        _faker = new Faker();

        _controller = new ModuleController(_mockService.Object)
        { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
    }

    [Fact]
    public async Task GetAllModulesAsync_WhenNoModulesExist_ReturnsOkWithEmptyPagination()
    {
        var query = new PaginationQuery();
        var cancellationToken = CancellationToken.None;

        _mockService.Setup(s => s.GetAllModulesAsync(query, cancellationToken))
            .ReturnsAsync(new Pagination<List<GetModuleResponse>>([], 0, query.Page, query.PerPage));

        var result = await _controller.GetAllModulesAsync(query, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPagination = okResult.Value as Pagination<List<GetModuleResponse>>;
        Assert.NotNull(returnedPagination);
        Assert.Empty(returnedPagination.Data);
        Assert.Equal(0, returnedPagination.Total);
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

        var modules = new List<GetModuleResponse>
        {
            new()
            {
                Id = module1.Id,
                Name = module1.Name,
                Type = module1.Type,
                Description = module1.Description,
                Icon = module1.Icon,
                IsActive = module1.IsActive,
                SortOrder = module1.SortOrder,
                Price = module1.Price
            },
            new()
            {
                Id = module2.Id,
                Name = module2.Name,
                Type = module2.Type,
                Description = module2.Description,
                Icon = module2.Icon,
                IsActive = module2.IsActive,
                SortOrder = module2.SortOrder,
                Price = module2.Price
            }
        };

        var query = new PaginationQuery();
        var cancellationToken = CancellationToken.None;

        _mockService.Setup(s => s.GetAllModulesAsync(query, cancellationToken))
            .ReturnsAsync(new Pagination<List<GetModuleResponse>>(modules, 2, query.Page, query.PerPage));

        var result = await _controller.GetAllModulesAsync(query, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPagination = okResult.Value as Pagination<List<GetModuleResponse>>;
        Assert.NotNull(returnedPagination);
        Assert.Equal(2, returnedPagination.Data.Count);
        Assert.Equal(2, returnedPagination.Total);
    }

    [Fact]
    public async Task GetAllModulesAsync_ExcludesErpAndAuthModuleTypes()
    {
        var basicModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 2
        };

        var modules = new List<GetModuleResponse>
        {
            new()
            {
                Id = basicModule.Id,
                Name = basicModule.Name,
                Type = basicModule.Type,
                Description = basicModule.Description,
                Icon = basicModule.Icon,
                IsActive = basicModule.IsActive,
                SortOrder = basicModule.SortOrder,
                Price = basicModule.Price
            }
        };

        var query = new PaginationQuery();
        var cancellationToken = CancellationToken.None;

        _mockService.Setup(s => s.GetAllModulesAsync(query, cancellationToken))
            .ReturnsAsync(new Pagination<List<GetModuleResponse>>(modules, 1, query.Page, query.PerPage));

        var result = await _controller.GetAllModulesAsync(query, cancellationToken);

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

        var modules = new List<GetModuleResponse>
        {
            new()
            {
                Id = activeModule.Id,
                Name = activeModule.Name,
                Type = activeModule.Type,
                Description = activeModule.Description,
                Icon = activeModule.Icon,
                IsActive = activeModule.IsActive,
                SortOrder = activeModule.SortOrder,
                Price = activeModule.Price
            }
        };

        var query = new PaginationQuery();
        var cancellationToken = CancellationToken.None;

        _mockService.Setup(s => s.GetAllModulesAsync(query, cancellationToken))
            .ReturnsAsync(new Pagination<List<GetModuleResponse>>(modules, 1, query.Page, query.PerPage));

        var result = await _controller.GetAllModulesAsync(query, cancellationToken);

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
    public async Task GetAllModulesAsync_WithPagination_ReturnsCorrectPage()
    {
        var query = new PaginationQuery(2);
        var cancellationToken = CancellationToken.None;

        _mockService.Setup(s => s.GetAllModulesAsync(query, cancellationToken))
            .ReturnsAsync(new Pagination<List<GetModuleResponse>>([], 25, 2, 10));

        var result = await _controller.GetAllModulesAsync(query, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPagination = okResult.Value as Pagination<List<GetModuleResponse>>;
        Assert.NotNull(returnedPagination);
        Assert.Empty(returnedPagination.Data);
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

        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void ModuleController_HasProducesAttribute()
    {
        var controllerType = typeof(ModuleController);

        var producesAttribute =
            controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }

    [Fact]
    public void GetAllModulesAsync_HasAllowAnonymousAttribute()
    {
        var controllerType = typeof(ModuleController);
        var methodInfo = controllerType.GetMethod(nameof(ModuleController.GetAllModulesAsync));

        var allowAnonymousAttribute =
            methodInfo?.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        Assert.NotNull(allowAnonymousAttribute);
    }
}
