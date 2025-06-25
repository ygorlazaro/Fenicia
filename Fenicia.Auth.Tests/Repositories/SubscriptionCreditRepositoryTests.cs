using Bogus;

using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.SubscriptionCredit;
using Fenicia.Common.Enums;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class SubscriptionCreditRepositoryTests
{
    private AuthContext _context;
    private SubscriptionCreditRepository _sut;
    private DbContextOptions<AuthContext> _options;
    private Faker<ModuleModel> _moduleGenerator;
    private Faker<SubscriptionCreditModel> _creditGenerator;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AuthContext(_options);
        _sut = new SubscriptionCreditRepository(_context);

        SetupFakers();
    }

    private void SetupFakers()
    {
        _moduleGenerator = new Faker<ModuleModel>()
            .RuleFor(m => m.Id, _ => Guid.NewGuid())
            .RuleFor(m => m.Type, f => f.PickRandom<ModuleType>())
            .RuleFor(m => m.Name, f => f.Commerce.ProductName());

        _creditGenerator = new Faker<SubscriptionCreditModel>()
            .RuleFor(c => c.Id, _ => Guid.NewGuid())
            .RuleFor(c => c.SubscriptionId, _ => Guid.NewGuid())
            .RuleFor(c => c.ModuleId, _ => Guid.NewGuid())
            .RuleFor(c => c.IsActive, _ => true)
            .RuleFor(c => c.StartDate, f => f.Date.Past())
            .RuleFor(c => c.EndDate, f => f.Date.Future());
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetValidModulesTypesAsync_ReturnsValidModules()
    {
        // Arrange
        var subscriptionIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var now = DateTime.Now;

        var modules = _moduleGenerator.Generate(3);
        var credits = subscriptionIds
            .Select(subscriptionId =>
                _creditGenerator
                    .Clone()
                    .RuleFor(c => c.SubscriptionId, subscriptionId)
                    .RuleFor(c => c.ModuleId, f => f.PickRandom(modules).Id)
                    .RuleFor(c => c.StartDate, now.AddDays(-10))
                    .RuleFor(c => c.EndDate, now.AddDays(10))
                    .Generate()
            )
            .ToList();

        await _context.Modules.AddRangeAsync(modules);
        await _context.SubscriptionCredits.AddRangeAsync(credits);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetValidModulesTypesAsync(subscriptionIds.ToList());

        // Assert
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Has.Count.LessThanOrEqualTo(modules.Count));
    }

    [Test]
    public async Task GetValidModulesTypesAsync_ReturnsEmptyList_WhenNoValidSubscriptions()
    {
        // Arrange
        var nonExistentSubscriptions = new Faker().Make(3, () => Guid.NewGuid());

        // Act
        var result = await _sut.GetValidModulesTypesAsync(nonExistentSubscriptions.ToList());

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidModulesTypesAsync_ExcludesInactiveCredits()
    {
        // Arrange
        var subscription = Guid.NewGuid();
        var now = DateTime.Now;

        var module = _moduleGenerator.Generate();
        var credit = _creditGenerator
            .Clone()
            .RuleFor(c => c.SubscriptionId, subscription)
            .RuleFor(c => c.ModuleId, module.Id)
            .RuleFor(c => c.IsActive, false)
            .RuleFor(c => c.StartDate, now.AddDays(-10))
            .RuleFor(c => c.EndDate, now.AddDays(10))
            .Generate();

        await _context.Modules.AddAsync(module);
        await _context.SubscriptionCredits.AddAsync(credit);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetValidModulesTypesAsync([subscription]);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidModulesTypesAsync_ExcludesExpiredCredits()
    {
        // Arrange
        var subscription = Guid.NewGuid();
        var now = DateTime.Now;

        var module = _moduleGenerator.Generate();
        var credit = _creditGenerator
            .Clone()
            .RuleFor(c => c.SubscriptionId, subscription)
            .RuleFor(c => c.ModuleId, module.Id)
            .RuleFor(c => c.StartDate, now.AddDays(-20))
            .RuleFor(c => c.EndDate, now.AddDays(-10))
            .Generate();

        await _context.Modules.AddAsync(module);
        await _context.SubscriptionCredits.AddAsync(credit);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetValidModulesTypesAsync([subscription]);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidModulesTypesAsync_ExcludesFutureCredits()
    {
        // Arrange
        var subscription = Guid.NewGuid();
        var now = DateTime.Now;

        var module = _moduleGenerator.Generate();
        var credit = _creditGenerator
            .Clone()
            .RuleFor(c => c.SubscriptionId, subscription)
            .RuleFor(c => c.ModuleId, module.Id)
            .RuleFor(c => c.StartDate, now.AddDays(10))
            .RuleFor(c => c.EndDate, now.AddDays(20))
            .Generate();

        await _context.Modules.AddAsync(module);
        await _context.SubscriptionCredits.AddAsync(credit);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetValidModulesTypesAsync([subscription]);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidModulesTypesAsync_ReturnsDuplicateModuleTypesOnlyOnce()
    {
        // Arrange
        var subscriptions = new Faker().Make(2, () => Guid.NewGuid());
        var now = DateTime.Now;

        var module = _moduleGenerator.Generate();
        var credits = subscriptions
            .Select(subscriptionId =>
                _creditGenerator
                    .Clone()
                    .RuleFor(c => c.SubscriptionId, subscriptionId)
                    .RuleFor(c => c.ModuleId, module.Id)
                    .RuleFor(c => c.StartDate, now.AddDays(-10))
                    .RuleFor(c => c.EndDate, now.AddDays(10))
                    .Generate()
            )
            .ToList();

        await _context.Modules.AddAsync(module);
        await _context.SubscriptionCredits.AddRangeAsync(credits);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetValidModulesTypesAsync(subscriptions.ToList());

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0], Is.EqualTo(module.Type));
    }

    [Test]
    public async Task GetValidModulesTypesAsync_HandlesMultipleValidModules()
    {
        // Arrange
        var subscription = Guid.NewGuid();
        var now = DateTime.Now;

        var moduleTypes = new[] { ModuleType.Accounting, ModuleType.Hr, ModuleType.Ecommerce };
        var modules = moduleTypes
            .Select(type => _moduleGenerator.Clone().RuleFor(m => m.Type, type).Generate())
            .ToList();

        var credits = modules
            .Select(module =>
                _creditGenerator
                    .Clone()
                    .RuleFor(c => c.SubscriptionId, subscription)
                    .RuleFor(c => c.ModuleId, module.Id)
                    .RuleFor(c => c.StartDate, now.AddDays(-10))
                    .RuleFor(c => c.EndDate, now.AddDays(10))
                    .Generate()
            )
            .ToList();

        await _context.Modules.AddRangeAsync(modules);
        await _context.SubscriptionCredits.AddRangeAsync(credits);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetValidModulesTypesAsync([subscription]);

        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result, Is.Unique);
        Assert.That(result, Is.EquivalentTo(modules.Select(m => m.Type)));
    }
}
