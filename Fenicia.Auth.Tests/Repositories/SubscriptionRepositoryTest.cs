using Bogus;

using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Enums;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class SubscriptionRepositoryTest
{
    private AuthContext _context;
    private SubscriptionRepository _sut;
    private DbContextOptions<AuthContext> _options;
    private Faker<SubscriptionModel> _subscriptionGenerator;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AuthContext(_options);
        _sut = new SubscriptionRepository(_context);

        SetupFakers();
    }

    private void SetupFakers()
    {
        _subscriptionGenerator = new Faker<SubscriptionModel>()
            .RuleFor(s => s.Id, _ => Guid.NewGuid())
            .RuleFor(s => s.CompanyId, _ => Guid.NewGuid())
            .RuleFor(s => s.StartDate, f => f.Date.Past())
            .RuleFor(s => s.EndDate, f => f.Date.Future())
            .RuleFor(s => s.Status, SubscriptionStatus.Active);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task SaveSubscription_ShouldSaveSubscriptionToDatabase()
    {
        // Arrange
        var subscription = _subscriptionGenerator.Generate();

        // Act
        await _sut.SaveSubscription(subscription);

        // Assert
        var savedSubscription = await _context.Subscriptions.FindAsync(subscription.Id);
        Assert.That(savedSubscription, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(savedSubscription!.Id, Is.EqualTo(subscription.Id));
            Assert.That(savedSubscription.CompanyId, Is.EqualTo(subscription.CompanyId));
        });
    }

    [Test]
    public async Task GetValidSubscriptionAsync_ShouldReturnActiveAndValidSubscriptions()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var validSubscription = _subscriptionGenerator
            .Clone()
            .RuleFor(s => s.CompanyId, companyId)
            .RuleFor(s => s.StartDate, now.AddDays(-1))
            .RuleFor(s => s.EndDate, now.AddDays(1))
            .RuleFor(s => s.Status, SubscriptionStatus.Active)
            .Generate();

        await _context.Subscriptions.AddAsync(validSubscription);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetValidSubscriptionAsync(companyId);

        // Assert
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Does.Contain(validSubscription.Id));
    }

    [Test]
    public async Task GetValidSubscriptionAsync_ShouldNotReturnExpiredSubscriptions()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var expiredSubscription = _subscriptionGenerator
            .Clone()
            .RuleFor(s => s.CompanyId, companyId)
            .RuleFor(s => s.StartDate, now.AddDays(-10))
            .RuleFor(s => s.EndDate, now.AddDays(-1))
            .Generate();

        await _context.Subscriptions.AddAsync(expiredSubscription);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetValidSubscriptionAsync(companyId);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidSubscriptionAsync_ShouldNotReturnFutureSubscriptions()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var futureSubscription = _subscriptionGenerator
            .Clone()
            .RuleFor(s => s.CompanyId, companyId)
            .RuleFor(s => s.StartDate, now.AddDays(1))
            .RuleFor(s => s.EndDate, now.AddDays(10))
            .Generate();

        await _context.Subscriptions.AddAsync(futureSubscription);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetValidSubscriptionAsync(companyId);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidSubscriptionAsync_ShouldNotReturnInactiveSubscriptions()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var inactiveSubscription = _subscriptionGenerator
            .Clone()
            .RuleFor(s => s.CompanyId, companyId)
            .RuleFor(s => s.StartDate, now.AddDays(-1))
            .RuleFor(s => s.EndDate, now.AddDays(1))
            .RuleFor(s => s.Status, SubscriptionStatus.Inactive)
            .Generate();

        await _context.Subscriptions.AddAsync(inactiveSubscription);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetValidSubscriptionAsync(companyId);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidSubscriptionAsync_ShouldNotReturnSubscriptionsForDifferentCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var differentCompanyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var subscription = _subscriptionGenerator
            .Clone()
            .RuleFor(s => s.CompanyId, differentCompanyId)
            .RuleFor(s => s.StartDate, now.AddDays(-1))
            .RuleFor(s => s.EndDate, now.AddDays(1))
            .Generate();

        await _context.Subscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetValidSubscriptionAsync(companyId);

        // Assert
        Assert.That(result, Is.Empty);
    }
}
