using Bogus;

using Fenicia.Auth.Domains.Subscription;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

namespace Fenicia.Auth.Tests.Repositories;

public class SubscriptionRepositoryTest
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private AuthContext context;
    private DbContextOptions<AuthContext> options;
    private Faker<SubscriptionModel> subscriptionGenerator;
    private SubscriptionRepository sut;

    [SetUp]
    public void Setup()
    {
        var mockLogger = new Mock<ILogger<SubscriptionRepository>>().Object;
        this.options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        this.context = new AuthContext(this.options);
        this.sut = new SubscriptionRepository(this.context, mockLogger);

        this.SetupFakers();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }

    [Test]
    public async Task SaveSubscriptionShouldSaveSubscriptionToDatabase()
    {
        // Arrange
        var subscription = this.subscriptionGenerator.Generate();

        // Act
        await this.sut.SaveSubscriptionAsync(subscription, this.cancellationToken);

        // Assert
        var savedSubscription = await this.context.Subscriptions.FindAsync([subscription.Id], this.cancellationToken);
        Assert.That(savedSubscription, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(savedSubscription!.Id, Is.EqualTo(subscription.Id));
            Assert.That(savedSubscription.CompanyId, Is.EqualTo(subscription.CompanyId));
        }
    }

    [Test]
    public async Task GetValidSubscriptionAsyncShouldReturnActiveAndValidSubscriptions()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var validSubscription = this.subscriptionGenerator.Clone().RuleFor(s => s.CompanyId, companyId).RuleFor(s => s.StartDate, now.AddDays(value: -1)).RuleFor(s => s.EndDate, now.AddDays(value: 1)).RuleFor(s => s.Status, SubscriptionStatus.Active).Generate();

        await this.context.Subscriptions.AddAsync(validSubscription, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.GetValidSubscriptionAsync(companyId, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Does.Contain(validSubscription.Id));
    }

    [Test]
    public async Task GetValidSubscriptionAsyncShouldNotReturnExpiredSubscriptions()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var expiredSubscription = this.subscriptionGenerator.Clone().RuleFor(s => s.CompanyId, companyId).RuleFor(s => s.StartDate, now.AddDays(value: -10)).RuleFor(s => s.EndDate, now.AddDays(value: -1)).Generate();

        await this.context.Subscriptions.AddAsync(expiredSubscription, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.GetValidSubscriptionAsync(companyId, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidSubscriptionAsyncShouldNotReturnFutureSubscriptions()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var futureSubscription = this.subscriptionGenerator.Clone().RuleFor(s => s.CompanyId, companyId).RuleFor(s => s.StartDate, now.AddDays(value: 1)).RuleFor(s => s.EndDate, now.AddDays(value: 10)).Generate();

        await this.context.Subscriptions.AddAsync(futureSubscription, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.GetValidSubscriptionAsync(companyId, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidSubscriptionAsyncShouldNotReturnInactiveSubscriptions()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var inactiveSubscription = this.subscriptionGenerator.Clone().RuleFor(s => s.CompanyId, companyId).RuleFor(s => s.StartDate, now.AddDays(value: -1)).RuleFor(s => s.EndDate, now.AddDays(value: 1)).RuleFor(s => s.Status, SubscriptionStatus.Inactive).Generate();

        await this.context.Subscriptions.AddAsync(inactiveSubscription, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.GetValidSubscriptionAsync(companyId, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidSubscriptionAsyncShouldNotReturnSubscriptionsForDifferentCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var differentCompanyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var subscription = this.subscriptionGenerator.Clone().RuleFor(s => s.CompanyId, differentCompanyId).RuleFor(s => s.StartDate, now.AddDays(value: -1)).RuleFor(s => s.EndDate, now.AddDays(value: 1)).Generate();

        await this.context.Subscriptions.AddAsync(subscription, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.GetValidSubscriptionAsync(companyId, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }

    private void SetupFakers()
    {
        this.subscriptionGenerator = new Faker<SubscriptionModel>().RuleFor(s => s.Id, _ => Guid.NewGuid()).RuleFor(s => s.CompanyId, _ => Guid.NewGuid()).RuleFor(s => s.StartDate, f => f.Date.Past()).RuleFor(s => s.EndDate, f => f.Date.Future()).RuleFor(s => s.Status, SubscriptionStatus.Active);
    }
}
