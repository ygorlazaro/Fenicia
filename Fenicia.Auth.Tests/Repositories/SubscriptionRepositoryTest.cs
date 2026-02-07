using Bogus;

using Fenicia.Auth.Domains.Subscription;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

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
        options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        context = new AuthContext(options);
        sut = new SubscriptionRepository(context);

        SetupFakers();
    }

    [TearDown]
    public void TearDown()
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    [Test]
    public async Task SaveSubscriptionShouldSaveSubscriptionToDatabase()
    {
        var subscription = subscriptionGenerator.Generate();

        sut.Add(subscription);

        await sut.SaveChangesAsync(cancellationToken);

        var savedSubscription = await context.Subscriptions.FindAsync([subscription.Id], cancellationToken);
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
        var companyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var validSubscription = subscriptionGenerator.Clone().RuleFor(s => s.CompanyId, companyId).RuleFor(s => s.StartDate, now.AddDays(-1)).RuleFor(s => s.EndDate, now.AddDays(1)).RuleFor(s => s.Status, SubscriptionStatus.Active).Generate();

        await context.Subscriptions.AddAsync(validSubscription, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetValidSubscriptionAsync(companyId, cancellationToken);

        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Does.Contain(validSubscription.Id));
    }

    [Test]
    public async Task GetValidSubscriptionAsyncShouldNotReturnExpiredSubscriptions()
    {
        var companyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var expiredSubscription = subscriptionGenerator.Clone().RuleFor(s => s.CompanyId, companyId).RuleFor(s => s.StartDate, now.AddDays(-10)).RuleFor(s => s.EndDate, now.AddDays(-1)).Generate();

        await context.Subscriptions.AddAsync(expiredSubscription, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetValidSubscriptionAsync(companyId, cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidSubscriptionAsyncShouldNotReturnFutureSubscriptions()
    {
        var companyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var futureSubscription = subscriptionGenerator.Clone().RuleFor(s => s.CompanyId, companyId).RuleFor(s => s.StartDate, now.AddDays(1)).RuleFor(s => s.EndDate, now.AddDays(10)).Generate();

        await context.Subscriptions.AddAsync(futureSubscription, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetValidSubscriptionAsync(companyId, cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidSubscriptionAsyncShouldNotReturnInactiveSubscriptions()
    {
        var companyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var inactiveSubscription = subscriptionGenerator.Clone().RuleFor(s => s.CompanyId, companyId).RuleFor(s => s.StartDate, now.AddDays(-1)).RuleFor(s => s.EndDate, now.AddDays(1)).RuleFor(s => s.Status, SubscriptionStatus.Inactive).Generate();

        await context.Subscriptions.AddAsync(inactiveSubscription, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetValidSubscriptionAsync(companyId, cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidSubscriptionAsyncShouldNotReturnSubscriptionsForDifferentCompany()
    {
        var companyId = Guid.NewGuid();
        var differentCompanyId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var subscription = subscriptionGenerator.Clone().RuleFor(s => s.CompanyId, differentCompanyId).RuleFor(s => s.StartDate, now.AddDays(-1)).RuleFor(s => s.EndDate, now.AddDays(1)).Generate();

        await context.Subscriptions.AddAsync(subscription, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetValidSubscriptionAsync(companyId, cancellationToken);

        Assert.That(result, Is.Empty);
    }

    private void SetupFakers()
    {
        subscriptionGenerator = new Faker<SubscriptionModel>().RuleFor(s => s.Id, _ => Guid.NewGuid()).RuleFor(s => s.CompanyId, _ => Guid.NewGuid()).RuleFor(s => s.StartDate, f => f.Date.Past()).RuleFor(s => s.EndDate, f => f.Date.Future()).RuleFor(s => s.Status, SubscriptionStatus.Active);
    }
}
