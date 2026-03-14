using Bogus;

using Fenicia.Auth.Domains.Module.Handlers;
using Fenicia.Auth.Domains.Module.Queries;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Module;

/// <summary>
/// Unit tests for the GetModulesHandler.
/// Tests retrieval of paginated modules from database.
/// </summary>
/// <remarks>
/// These tests verify the core functionality of module retrieval:
/// - Pagination returns correct data and metadata
/// - Auth type modules are excluded from results
/// - Results are ordered by module type
/// - Empty results are handled correctly
/// - Response contains all required fields
/// </remarks>
public class GetModulesHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly GetModulesHandler handler;
    private readonly Faker faker;

    public GetModulesHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options, new TestCompanyContext());
        this.handler = new GetModulesHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Tests that when modules exist, they are returned with correct pagination metadata.
    /// </summary>
    [Fact]
    public async Task Handle_WhenModulesExist_ReturnsPaginatedModules()
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

        this.db.AuthModules.AddRange(module1,
            module2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesQuery(1,
            10);

        // Act
        var result = await this.handler.Handle(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Equal(2,
            result.Data.Count);
        Assert.Equal(2,
            result.Total);
        Assert.Equal(1,
            result.Page);
        Assert.Equal(10,
            result.PerPage);
    }

    /// <summary>
    /// Tests that Auth type modules are excluded from results while Basic modules are included.
    /// </summary>
    [Fact]
    public async Task Handle_WhenModulesExist_ExcludesErpAndAuthTypes()
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

        this.db.AuthModules.AddRange(authModule,
            basicModule);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesQuery(1,
            10);

        // Act
        var result = await this.handler.Handle(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Single(result.Data);
        Assert.Equal(basicModule.Name,
            result.Data[0].Name);
        Assert.Equal(1,
            result.Total);
    }

    /// <summary>
    /// Tests that pagination returns the correct page of results.
    /// </summary>
    [Fact]
    public async Task Handle_WhenPaginationIsApplied_ReturnsCorrectPage()
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

        var request = new GetModulesQuery(2,
            10);

        // Act
        var result = await this.handler.Handle(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Equal(10,
            result.Data.Count);
        Assert.Equal(25,
            result.Total);
        Assert.Equal(2,
            result.Page);
        Assert.Equal(10,
            result.PerPage);
        Assert.Equal(3,
            result.Pages);
    }

    /// <summary>
    /// Tests that when no modules exist, an empty pagination result is returned.
    /// </summary>
    [Fact]
    public async Task Handle_WhenNoModulesExist_ReturnsEmptyPagination()
    {
        // Arrange
        var request = new GetModulesQuery(1,
            10);

        // Act
        var result = await this.handler.Handle(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Empty(result.Data);
        Assert.Equal(0,
            result.Total);
    }

    /// <summary>
    /// Tests that when page number exceeds available pages, empty data is returned.
    /// </summary>
    [Fact]
    public async Task Handle_WhenPageExceedsTotalPages_ReturnsEmptyData()
    {
        // Arrange
        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Basic Module",
            Type = ModuleType.Basic,
            Price = 10.0m
        };

        this.db.AuthModules.Add(module);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesQuery(10,
            10);

        // Act
        var result = await this.handler.Handle(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Empty(result.Data);
        Assert.Equal(1,
            result.Total);
    }

    /// <summary>
    /// Tests that results are ordered by module type in ascending order.
    /// </summary>
    [Fact]
    public async Task Handle_ResultsAreOrderedByType()
    {
        // Arrange
        var module1 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.SocialNetwork,
            Price = 20.0m
        };

        var module2 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m
        };

        var module3 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Hr,
            Price = 30.0m
        };

        this.db.AuthModules.AddRange(module1,
            module2,
            module3);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesQuery(1,
            10);

        // Act
        var result = await this.handler.Handle(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Equal(3,
            result.Data.Count);
        Assert.Equal(ModuleType.Basic,
            result.Data[0].Type);
        Assert.Equal(ModuleType.SocialNetwork,
            result.Data[1].Type);
        Assert.Equal(ModuleType.Hr,
            result.Data[2].Type);
    }

    /// <summary>
    /// Tests that default request values (page 1, 20 items) are applied correctly.
    /// </summary>
    [Fact]
    public async Task Handle_WithDefaultRequest_ReturnsFirstPage()
    {
        // Arrange
        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m
        };

        this.db.AuthModules.Add(module);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesQuery();

        // Act
        var result = await this.handler.Handle(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Equal(1,
            result.Page);
        Assert.Equal(20,
            result.PerPage);
    }

    /// <summary>
    /// Tests that the response contains all required fields (Id, Name, Type).
    /// </summary>
    [Fact]
    public async Task Handle_VerifiesResponseContainsAllFields()
    {
        // Arrange
        var moduleId = Guid.NewGuid();
        var module = new ModuleModel
        {
            Id = moduleId,
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m
        };

        this.db.AuthModules.Add(module);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesQuery(1,
            10);

        // Act
        var result = await this.handler.Handle(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Data);
        var moduleResponse = result.Data[0];
        
        Assert.Equal(moduleId,
            moduleResponse.Id);
        Assert.Equal(module.Name,
            moduleResponse.Name);
        Assert.Equal(ModuleType.Basic,
            moduleResponse.Type);
    }
}
