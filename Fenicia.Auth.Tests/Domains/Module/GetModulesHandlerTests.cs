using Bogus;

using Fenicia.Auth.Domains.Module.GetModules;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Module;

public class GetModulesHandlerTests : IDisposable
{
    private readonly DefaultContext context;
    private readonly GetModulesHandler handler;
    private readonly Faker faker;

    public GetModulesHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.handler = new GetModulesHandler(this.context);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

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

        this.context.AuthModules.AddRange(module1, module2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesRequest(1, 10);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(2, result.Total);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PerPage);
    }

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

        this.context.AuthModules.AddRange(authModule, basicModule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesRequest(1, 10);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Single(result.Data);
        Assert.Equal(basicModule.Name, result.Data[0].Name);
        Assert.Equal(1, result.Total);
    }

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

        this.context.AuthModules.AddRange(modules);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesRequest(2, 10);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Equal(10, result.Data.Count);
        Assert.Equal(25, result.Total);
        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PerPage);
        Assert.Equal(3, result.Pages);
    }

    [Fact]
    public async Task Handle_WhenNoModulesExist_ReturnsEmptyPagination()
    {
        // Arrange
        var request = new GetModulesRequest(1, 10);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
    }

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

        this.context.AuthModules.Add(module);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesRequest(10, 10);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Empty(result.Data);
        Assert.Equal(1, result.Total);
    }

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

        this.context.AuthModules.AddRange(module1, module2, module3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesRequest(1, 10);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Equal(3, result.Data.Count);
        Assert.Equal(ModuleType.Basic, result.Data[0].Type);
        Assert.Equal(ModuleType.SocialNetwork, result.Data[1].Type);
        Assert.Equal(ModuleType.Hr, result.Data[2].Type);
    }

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

        this.context.AuthModules.Add(module);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesRequest();

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PerPage);
    }

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

        this.context.AuthModules.Add(module);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesRequest(1, 10);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Data);
        var moduleResponse = result.Data[0];
        
        Assert.Equal(moduleId, moduleResponse.Id);
        Assert.Equal(module.Name, moduleResponse.Name);
        Assert.Equal(ModuleType.Basic, moduleResponse.Type);
    }
}
