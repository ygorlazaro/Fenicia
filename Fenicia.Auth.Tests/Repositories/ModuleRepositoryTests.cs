using Bogus;

using Fenicia.Auth.Domains.Module;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Enums;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class ModuleRepositoryTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private AuthContext context;
    private Faker faker;
    private DbContextOptions<AuthContext> options;
    private ModuleRepository sut;

    [SetUp]
    public void Setup()
    {
        options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        context = new AuthContext(options);
        sut = new ModuleRepository(context);
        faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    [Test]
    public async Task GetAllOrderedAsyncReturnsModulesWithPagination()
    {
        // Arrange
        var modules = new List<ModuleModel>();
        for (var i = 0; i < 15; i++)
        {
            modules.Add(new ModuleModel
            {
                Id = Guid.NewGuid(),
                Type = (ModuleType)(i % 5),
                Name = faker.Commerce.ProductName()
            });
        }

        await context.Modules.AddRangeAsync(modules, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act
        var page1 = await sut.GetAllOrderedAsync(cancellationToken, page: 1, perPage: 10);
        var page2 = await sut.GetAllOrderedAsync(cancellationToken, page: 2, perPage: 10);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(page1, Has.Count.EqualTo(expected: 10));
            Assert.That(page2, Has.Count.EqualTo(expected: 5));
        }

        Assert.That(page1, Is.Ordered.By("Type"));
    }

    [Test]
    public async Task GetAllOrderedAsyncReturnsEmptyListWhenNoModules()
    {
        // Act
        var result = await sut.GetAllOrderedAsync(cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetManyOrdersAsyncReturnsRequestedModules()
    {
        // Arrange
        var modules = new List<ModuleModel>();
        var requestedIDs = new List<Guid>();

        for (var i = 0; i < 5; i++)
        {
            var module = new ModuleModel
            {
                Id = Guid.NewGuid(),
                Type = (ModuleType)i,
                Name = faker.Commerce.ProductName()
            };
            modules.Add(module);
            if (i < 3)
            {
                requestedIDs.Add(module.Id);
            }
        }

        await context.Modules.AddRangeAsync(modules, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act
        var result = await sut.GetManyOrdersAsync(requestedIDs, cancellationToken);

        // Assert
        Assert.That(result, Has.Count.EqualTo(expected: 3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Select(m => m.Id), Is.EquivalentTo(requestedIDs));
            Assert.That(result, Is.Ordered.By("Type"));
        }
    }

    [Test]
    public async Task GetManyOrdersAsyncReturnsEmptyListWhenNoMatchingIDs()
    {
        // Arrange
        var nonExistentIDs = new[] { Guid.NewGuid(), Guid.NewGuid() };

        // Act
        var result = await sut.GetManyOrdersAsync(nonExistentIDs, cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetModuleByTypeAsyncReturnsModuleWhenExists()
    {
        // Arrange
        var moduleType = ModuleType.Accounting;
        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Type = moduleType,
            Name = faker.Commerce.ProductName()
        };

        await context.Modules.AddAsync(module, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act
        var result = await sut.GetModuleByTypeAsync(moduleType, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Type, Is.EqualTo(moduleType));
            Assert.That(result.Id, Is.EqualTo(module.Id));
        }
    }

    [Test]
    public async Task GetModuleByTypeAsyncReturnsNullWhenNotExists()
    {
        // Act
        var result = await sut.GetModuleByTypeAsync(ModuleType.Accounting, cancellationToken);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CountAsyncReturnsCorrectCount()
    {
        // Arrange
        var expectedCount = 5;
        var modules = new List<ModuleModel>();

        for (var i = 0; i < expectedCount; i++)
        {
            modules.Add(new ModuleModel
            {
                Id = Guid.NewGuid(),
                Type = (ModuleType)i,
                Name = faker.Commerce.ProductName()
            });
        }

        await context.Modules.AddRangeAsync(modules, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act
        var count = await sut.CountAsync(cancellationToken);

        // Assert
        Assert.That(count, Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task CountAsyncReturnsZeroWhenNoModules()
    {
        // Act
        var count = await sut.CountAsync(cancellationToken);

        // Assert
        Assert.That(count, Is.Zero);
    }

    [Test]
    public async Task GetAllOrderedAsyncRespectsPaginationWithDifferentPageSizes()
    {
        // Arrange
        var modules = new List<ModuleModel>();
        for (var i = 0; i < 25; i++)
        {
            modules.Add(new ModuleModel
            {
                Id = Guid.NewGuid(),
                Type = (ModuleType)(i % 5),
                Name = faker.Commerce.ProductName()
            });
        }

        await context.Modules.AddRangeAsync(modules, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act & Assert
        var page1Size5 = await sut.GetAllOrderedAsync(cancellationToken, page: 1, perPage: 5);
        Assert.That(page1Size5, Has.Count.EqualTo(expected: 5));

        var page2Size15 = await sut.GetAllOrderedAsync(cancellationToken, page: 2, perPage: 15);
        Assert.That(page2Size15, Has.Count.EqualTo(expected: 10));
    }
}
