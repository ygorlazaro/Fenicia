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
    private DbContextOptions<AuthContext> options;
    private ModuleRepository sut;

    [SetUp]
    public void Setup()
    {
        this.options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        this.context = new AuthContext(this.options);
        this.sut = new ModuleRepository(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
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
                Name = this.faker.Commerce.ProductName()
            });
        }

        await this.context.Modules.AddRangeAsync(modules, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var page1 = await this.sut.GetAllOrderedAsync(this.cancellationToken, page: 1, perPage: 10);
        var page2 = await this.sut.GetAllOrderedAsync(this.cancellationToken, page: 2, perPage: 10);

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
        var result = await this.sut.GetAllOrderedAsync(this.cancellationToken);

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
                Name = this.faker.Commerce.ProductName()
            };
            modules.Add(module);
            if (i < 3)
            {
                requestedIDs.Add(module.Id);
            }
        }

        await this.context.Modules.AddRangeAsync(modules, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.GetManyOrdersAsync(requestedIDs, this.cancellationToken);

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
        var result = await this.sut.GetManyOrdersAsync(nonExistentIDs, this.cancellationToken);

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
            Name = this.faker.Commerce.ProductName()
        };

        await this.context.Modules.AddAsync(module, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.GetModuleByTypeAsync(moduleType, this.cancellationToken);

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
        var result = await this.sut.GetModuleByTypeAsync(ModuleType.Accounting, this.cancellationToken);

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
                Name = this.faker.Commerce.ProductName()
            });
        }

        await this.context.Modules.AddRangeAsync(modules, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var count = await this.sut.CountAsync(this.cancellationToken);

        // Assert
        Assert.That(count, Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task CountAsync_ReturnsZero_WhenNoModules()
    {
        // Act
        var count = await this.sut.CountAsync(this.cancellationToken);

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
                Name = this.faker.Commerce.ProductName()
            });
        }

        await this.context.Modules.AddRangeAsync(modules, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act & Assert
        var page1Size5 = await this.sut.GetAllOrderedAsync(this.cancellationToken, page: 1, perPage: 5);
        Assert.That(page1Size5, Has.Count.EqualTo(expected: 5));

        var page2Size15 = await this.sut.GetAllOrderedAsync(this.cancellationToken, page: 2, perPage: 15);
        Assert.That(page2Size15, Has.Count.EqualTo(expected: 10));
    }
}
