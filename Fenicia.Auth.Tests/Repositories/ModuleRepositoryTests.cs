using Bogus;

using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Module;
using Fenicia.Common.Enums;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class ModuleRepositoryTests
{
    private AuthContext _context;
    private ModuleRepository _sut;
    private Faker _faker;
    private DbContextOptions<AuthContext> _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AuthContext(_options);
        _sut = new ModuleRepository(_context);
        _faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetAllOrderedAsync_ReturnsModules_WithPagination()
    {
        // Arrange
        var modules = new List<ModuleModel>();
        for (var i = 0; i < 15; i++)
        {
            modules.Add(
                new ModuleModel
                {
                    Id = Guid.NewGuid(),
                    Type = (ModuleType)(i % 5),
                    Name = _faker.Commerce.ProductName()
                }
            );
        }
        await _context.Modules.AddRangeAsync(modules);
        await _context.SaveChangesAsync();

        // Act
        var page1 = await _sut.GetAllOrderedAsync(page: 1, perPage: 10);
        var page2 = await _sut.GetAllOrderedAsync(page: 2, perPage: 10);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(page1, Has.Count.EqualTo(10));
            Assert.That(page2, Has.Count.EqualTo(5));
        });
        Assert.That(page1, Is.Ordered.By("Type"));
    }

    [Test]
    public async Task GetAllOrderedAsync_ReturnsEmptyList_WhenNoModules()
    {
        // Act
        var result = await _sut.GetAllOrderedAsync();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetManyOrdersAsync_ReturnsRequestedModules()
    {
        // Arrange
        var modules = new List<ModuleModel>();
        var requestedIds = new List<Guid>();

        for (var i = 0; i < 5; i++)
        {
            var module = new ModuleModel
            {
                Id = Guid.NewGuid(),
                Type = (ModuleType)i,
                Name = _faker.Commerce.ProductName()
            };
            modules.Add(module);
            if (i < 3) // Request only first 3 modules
            {
                requestedIds.Add(module.Id);
            }
        }

        await _context.Modules.AddRangeAsync(modules);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetManyOrdersAsync(requestedIds);

        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(result.Select(m => m.Id), Is.EquivalentTo(requestedIds));
            Assert.That(result, Is.Ordered.By("Type"));
        });
    }

    [Test]
    public async Task GetManyOrdersAsync_ReturnsEmptyList_WhenNoMatchingIds()
    {
        // Arrange
        var nonExistentIds = new[] { Guid.NewGuid(), Guid.NewGuid() };

        // Act
        var result = await _sut.GetManyOrdersAsync(nonExistentIds);

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
            Name = _faker.Commerce.ProductName()
        };

        await _context.Modules.AddAsync(module);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetModuleByTypeAsync(moduleType);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Type, Is.EqualTo(moduleType));
            Assert.That(result.Id, Is.EqualTo(module.Id));
        });
    }

    [Test]
    public async Task GetModuleByTypeAsync_ReturnsNull_WhenNotExists()
    {
        // Act
        var result = await _sut.GetModuleByTypeAsync(ModuleType.Accounting);

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
            modules.Add(
                new ModuleModel
                {
                    Id = Guid.NewGuid(),
                    Type = (ModuleType)i,
                    Name = _faker.Commerce.ProductName()
                }
            );
        }

        await _context.Modules.AddRangeAsync(modules);
        await _context.SaveChangesAsync();

        // Act
        var count = await _sut.CountAsync();

        // Assert
        Assert.That(count, Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task CountAsync_ReturnsZero_WhenNoModules()
    {
        // Act
        var count = await _sut.CountAsync();

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
            modules.Add(
                new ModuleModel
                {
                    Id = Guid.NewGuid(),
                    Type = (ModuleType)(i % 5),
                    Name = _faker.Commerce.ProductName()
                }
            );
        }
        await _context.Modules.AddRangeAsync(modules);
        await _context.SaveChangesAsync();

        // Act & Assert
        var page1Size5 = await _sut.GetAllOrderedAsync(page: 1, perPage: 5);
        Assert.That(page1Size5, Has.Count.EqualTo(5));

        var page2Size15 = await _sut.GetAllOrderedAsync(page: 2, perPage: 15);
        Assert.That(page2Size15, Has.Count.EqualTo(10));
    }
}
