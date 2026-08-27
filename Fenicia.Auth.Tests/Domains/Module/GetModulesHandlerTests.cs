using Bogus;

using Fenicia.Auth.Domains.Module.Handlers;
using Fenicia.Auth.Domains.Module.Queries;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Module;

public class GetModulesHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetModulesHandler handler;

    public GetModulesHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        handler = new GetModulesHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenModulesExist_ReturnsPaginatedModules()
    {

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

        var request = new GetModulesQuery(1, 10);

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(2, result.Data.Count);
        Assert.Equal(2, result.Total);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PerPage);
    }

    [Fact]
    public async Task Handle_WhenModulesExist_ExcludesErpAndAuthTypes()
    {

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

        var request = new GetModulesQuery(1, 10);

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Single(result.Data);
        Assert.Equal(basicModule.Name, result.Data[0].Name);
        Assert.Equal(1, result.Total);
    }

    [Fact]
    public async Task Handle_WhenInactiveModulesExist_ExcludesInactiveModules()
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

        db.AuthModules.AddRange(activeModule, inactiveModule);
        await db.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesQuery(1, 10);

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Single(result.Data);
        Assert.Equal(activeModule.Name, result.Data[0].Name);
    }

    [Fact]
    public async Task Handle_WhenPaginationIsApplied_ReturnsCorrectPage()
    {

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

        var request = new GetModulesQuery(2, 10);

        var result = await handler.Handle(request, CancellationToken.None);

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

        var request = new GetModulesQuery(1, 10);

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task Handle_WhenPageExceedsTotalPages_ReturnsEmptyData()
    {

        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Basic Module",
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 1
        };

        db.AuthModules.Add(module);
        await db.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesQuery(10, 10);

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Empty(result.Data);
        Assert.Equal(1, result.Total);
    }

    [Fact]
    public async Task Handle_ResultsAreOrderedBySortOrder()
    {

        var module1 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Social Network Module",
            Type = ModuleType.SocialNetwork,
            Price = 20.0m,
            IsActive = true,
            SortOrder = 3
        };

        var module2 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Basic Module",
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 1
        };

        var module3 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "HR Module",
            Type = ModuleType.Hr,
            Price = 30.0m,
            IsActive = true,
            SortOrder = 2
        };

        db.AuthModules.AddRange(module1, module2, module3);
        await db.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesQuery(1, 10);

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(3, result.Data.Count);
        Assert.Equal("Basic Module", result.Data[0].Name);
        Assert.Equal(1, result.Data[0].SortOrder);
        Assert.Equal("HR Module", result.Data[1].Name);
        Assert.Equal(2, result.Data[1].SortOrder);
        Assert.Equal("Social Network Module", result.Data[2].Name);
        Assert.Equal(3, result.Data[2].SortOrder);
    }

    [Fact]
    public async Task Handle_WithDefaultRequest_ReturnsFirstPage()
    {

        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 1
        };

        db.AuthModules.Add(module);
        await db.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesQuery();

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PerPage);
    }

    [Fact]
    public async Task Handle_VerifiesResponseContainsAllFields()
    {

        var moduleId = Guid.NewGuid();
        var description = "Test module description";
        var icon = "icon-test";
        var sortOrder = 5;

        var module = new ModuleModel
        {
            Id = moduleId,
            Name = faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            Description = description,
            Icon = icon,
            IsActive = true,
            SortOrder = sortOrder
        };

        db.AuthModules.Add(module);
        await db.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesQuery(1, 10);

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Data);
        var moduleResponse = result.Data[0];

        Assert.Equal(moduleId, moduleResponse.Id);
        Assert.Equal(module.Name, moduleResponse.Name);
        Assert.Equal(ModuleType.Basic, moduleResponse.Type);
        Assert.Equal(description, moduleResponse.Description);
        Assert.Equal(icon, moduleResponse.Icon);
        Assert.True(moduleResponse.IsActive);
        Assert.Equal(sortOrder, moduleResponse.SortOrder);
    }
}
