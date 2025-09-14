namespace Fenicia.Auth.Tests.Repositories;

using Bogus;

using Common.Enums;

using Domains.SubscriptionCredit.Logic;

using Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

public class SubscriptionCreditRepositoryTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private AuthContext _context;
    private Faker<SubscriptionCreditModel> _creditGenerator;
    private Faker<ModuleModel> _moduleGenerator;
    private DbContextOptions<AuthContext> _options;
    private SubscriptionCreditRepository _sut;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        _context = new AuthContext(_options);
        _sut = new SubscriptionCreditRepository(_context);

        SetupFakers();
    }

    private void SetupFakers()
    {
        _moduleGenerator = new Faker<ModuleModel>().RuleFor(m => m.Id, _ => Guid.NewGuid()).RuleFor(m => m.Type, f => f.PickRandom<ModuleType>()).RuleFor(m => m.Name, f => f.Commerce.ProductName());

        _creditGenerator = new Faker<SubscriptionCreditModel>().RuleFor(c => c.Id, _ => Guid.NewGuid()).RuleFor(c => c.SubscriptionId, _ => Guid.NewGuid()).RuleFor(c => c.ModuleId, _ => Guid.NewGuid()).RuleFor(c => c.IsActive, _ => true).RuleFor(c => c.StartDate, f => f.Date.Past()).RuleFor(c => c.EndDate, f => f.Date.Future());
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
        var now = DateTime.UtcNow;

        var modules = _moduleGenerator.Generate(count: 3);
        var credits = subscriptionIds.Select(subscriptionId => _creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscriptionId).RuleFor(c => c.ModuleId, f => f.PickRandom(modules).Id).RuleFor(c => c.StartDate, now.AddDays(value: -10)).RuleFor(c => c.EndDate, now.AddDays(value: 10)).Generate()).ToList();

        await _context.Modules.AddRangeAsync(modules, _cancellationToken);
        await _context.SubscriptionCredits.AddRangeAsync(credits, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.GetValidModulesTypesAsync([.. subscriptionIds], _cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Has.Count.LessThanOrEqualTo(modules.Count));
    }

    [Test]
    public async Task GetValidModulesTypesAsync_ReturnsEmptyList_WhenNoValidSubscriptions()
    {
        // Arrange
        var nonExistentSubscriptions = new Faker().Make(count: 3, Guid.NewGuid);

        // Act
        var result = await _sut.GetValidModulesTypesAsync([.. nonExistentSubscriptions], _cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidModulesTypesAsync_ExcludesInactiveCredits()
    {
        // Arrange
        var subscription = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var module = _moduleGenerator.Generate();
        var credit = _creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscription).RuleFor(c => c.ModuleId, module.Id).RuleFor(c => c.IsActive, value: false).RuleFor(c => c.StartDate, now.AddDays(value: -10)).RuleFor(c => c.EndDate, now.AddDays(value: 10)).Generate();

        await _context.Modules.AddAsync(module, _cancellationToken);
        await _context.SubscriptionCredits.AddAsync(credit, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.GetValidModulesTypesAsync([subscription], _cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidModulesTypesAsync_ExcludesExpiredCredits()
    {
        // Arrange
        var subscription = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var module = _moduleGenerator.Generate();
        var credit = _creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscription).RuleFor(c => c.ModuleId, module.Id).RuleFor(c => c.StartDate, now.AddDays(value: -20)).RuleFor(c => c.EndDate, now.AddDays(value: -10)).Generate();

        await _context.Modules.AddAsync(module, _cancellationToken);
        await _context.SubscriptionCredits.AddAsync(credit, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.GetValidModulesTypesAsync([subscription], _cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidModulesTypesAsync_ExcludesFutureCredits()
    {
        // Arrange
        var subscription = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var module = _moduleGenerator.Generate();
        var credit = _creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscription).RuleFor(c => c.ModuleId, module.Id).RuleFor(c => c.StartDate, now.AddDays(value: 10)).RuleFor(c => c.EndDate, now.AddDays(value: 20)).Generate();

        await _context.Modules.AddAsync(module, _cancellationToken);
        await _context.SubscriptionCredits.AddAsync(credit, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.GetValidModulesTypesAsync([subscription], _cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidModulesTypesAsync_ReturnsDuplicateModuleTypesOnlyOnce()
    {
        // Arrange
        var subscriptions = new Faker().Make(count: 2, Guid.NewGuid);
        var now = DateTime.UtcNow;

        var module = _moduleGenerator.Generate();
        var credits = subscriptions.Select(subscriptionId => _creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscriptionId).RuleFor(c => c.ModuleId, module.Id).RuleFor(c => c.StartDate, now.AddDays(value: -10)).RuleFor(c => c.EndDate, now.AddDays(value: 10)).Generate()).ToList();

        await _context.Modules.AddAsync(module, _cancellationToken);
        await _context.SubscriptionCredits.AddRangeAsync(credits, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.GetValidModulesTypesAsync([.. subscriptions], _cancellationToken);

        // Assert
        Assert.That(result, Has.Count.EqualTo(expected: 1));
        Assert.That(result[index: 0], Is.EqualTo(module.Type));
    }

    [Test]
    public async Task GetValidModulesTypesAsync_HandlesMultipleValidModules()
    {
        // Arrange
        var subscription = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var moduleTypes = new[] { ModuleType.Accounting, ModuleType.Hr, ModuleType.Ecommerce };
        var modules = moduleTypes.Select(type => _moduleGenerator.Clone().RuleFor(m => m.Type, type).Generate()).ToList();

        var credits = modules.Select(module => _creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscription).RuleFor(c => c.ModuleId, module.Id).RuleFor(c => c.StartDate, now.AddDays(value: -10)).RuleFor(c => c.EndDate, now.AddDays(value: 10)).Generate()).ToList();

        await _context.Modules.AddRangeAsync(modules, _cancellationToken);
        await _context.SubscriptionCredits.AddRangeAsync(credits, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.GetValidModulesTypesAsync([subscription], _cancellationToken);

        // Assert
        Assert.That(result, Has.Count.EqualTo(expected: 3));
        Assert.That(result, Is.Unique);
        Assert.That(result, Is.EquivalentTo(modules.Select(m => m.Type)));
    }
}
