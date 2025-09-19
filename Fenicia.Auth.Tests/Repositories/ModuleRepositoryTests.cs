namespace Fenicia.Auth.Tests.Repositories;

using Bogus;

using Common.Enums;

using Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;
using Fenicia.Auth.Domains.Module;

public class ModuleRepositoryTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private AuthContext context;
    private Faker faker;
    private DbContextOptions<AuthContext> _options;
    private ModuleRepository sut;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        context = new AuthContext(_options);
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
    public async Task GetAllOrderedAsync_ReturnsModules_WithPagination()
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
    public async Task GetAllOrderedAsync_ReturnsEmptyList_WhenNoModules()
    {
        // Act
        var result = await sut.GetAllOrderedAsync(cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetManyOrdersAsync_ReturnsRequestedModules()
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
    public async Task GetManyOrdersAsync_ReturnsEmptyList_WhenNoMatchingIDs()
    {
        // Arrange
        var nonExistentIDs = new[] { Guid.NewGuid(), Guid.NewGuid() };

        // Act
        var result = await sut.GetManyOrdersAsync(nonExistentIDs, cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetModuleByTypeAsync_ReturnsModule_WhenExists()
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
    public async Task GetModuleByTypeAsync_ReturnsNull_WhenNotExists()
    {
        // Act
        var result = await sut.GetModuleByTypeAsync(ModuleType.Accounting, cancellationToken);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CountAsync_ReturnsCorrectCount()
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
    public async Task CountAsync_ReturnsZero_WhenNoModules()
    {
        // Act
        var count = await sut.CountAsync(cancellationToken);

        // Assert
        Assert.That(count, Is.Zero);
    }

    [Test]
    public async Task GetAllOrderedAsync_RespectsPagination_WithDifferentPageSizes()
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
