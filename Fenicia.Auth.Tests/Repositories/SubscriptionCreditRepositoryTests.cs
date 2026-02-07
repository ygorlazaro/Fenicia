using Bogus;

using Fenicia.Auth.Domains.SubscriptionCredit;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class SubscriptionCreditRepositoryTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private AuthContext context;
    private Faker<SubscriptionCreditModel> creditGenerator;
    private Faker<ModuleModel> moduleGenerator;
    private DbContextOptions<AuthContext> options;
    private SubscriptionCreditRepository sut;

    [SetUp]
    public void Setup()
    {
        options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        context = new AuthContext(options);
        sut = new SubscriptionCreditRepository(context);

        SetupFakers();
    }

    [TearDown]
    public void TearDown()
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    [Test]
    public async Task GetValidModulesTypesAsyncReturnsValidModules()
    {
        var subscriptionIDs = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var now = DateTime.UtcNow;

        var modules = moduleGenerator.Generate(3);
        var credits = subscriptionIDs.Select(subscriptionId => creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscriptionId).RuleFor(c => c.ModuleId, f => f.PickRandom(modules).Id).RuleFor(c => c.StartDate, now.AddDays(-10)).RuleFor(c => c.EndDate, now.AddDays(10)).Generate()).ToList();

        await context.Modules.AddRangeAsync(modules, cancellationToken);
        await context.SubscriptionCredits.AddRangeAsync(credits, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetValidModulesTypesAsync([.. subscriptionIDs], cancellationToken);

        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Has.Count.LessThanOrEqualTo(modules.Count));
    }

    [Test]
    public async Task GetValidModulesTypesAsyncReturnsEmptyListWhenNoValidSubscriptions()
    {
        var nonExistentSubscriptions = new Faker().Make(3, Guid.NewGuid);

        var result = await sut.GetValidModulesTypesAsync([.. nonExistentSubscriptions], cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidModulesTypesAsyncExcludesInactiveCredits()
    {
        var subscription = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var module = moduleGenerator.Generate();
        var credit = creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscription).RuleFor(c => c.ModuleId, module.Id).RuleFor(c => c.IsActive, false).RuleFor(c => c.StartDate, now.AddDays(-10)).RuleFor(c => c.EndDate, now.AddDays(10)).Generate();

        await context.Modules.AddAsync(module, cancellationToken);
        await context.SubscriptionCredits.AddAsync(credit, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetValidModulesTypesAsync([subscription], cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidModulesTypesAsyncExcludesExpiredCredits()
    {
        var subscription = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var module = moduleGenerator.Generate();
        var credit = creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscription).RuleFor(c => c.ModuleId, module.Id).RuleFor(c => c.StartDate, now.AddDays(-20)).RuleFor(c => c.EndDate, now.AddDays(-10)).Generate();

        await context.Modules.AddAsync(module, cancellationToken);
        await context.SubscriptionCredits.AddAsync(credit, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetValidModulesTypesAsync([subscription], cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidModulesTypesAsyncExcludesFutureCredits()
    {
        var subscription = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var module = moduleGenerator.Generate();
        var credit = creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscription).RuleFor(c => c.ModuleId, module.Id).RuleFor(c => c.StartDate, now.AddDays(10)).RuleFor(c => c.EndDate, now.AddDays(20)).Generate();

        await context.Modules.AddAsync(module, cancellationToken);
        await context.SubscriptionCredits.AddAsync(credit, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetValidModulesTypesAsync([subscription], cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidModulesTypesAsyncReturnsDuplicateModuleTypesOnlyOnce()
    {
        var subscriptions = new Faker().Make(2, Guid.NewGuid);
        var now = DateTime.UtcNow;

        var module = moduleGenerator.Generate();
        var credits = subscriptions.Select(subscriptionId => creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscriptionId).RuleFor(c => c.ModuleId, module.Id).RuleFor(c => c.StartDate, now.AddDays(-10)).RuleFor(c => c.EndDate, now.AddDays(10)).Generate()).ToList();

        await context.Modules.AddAsync(module, cancellationToken);
        await context.SubscriptionCredits.AddRangeAsync(credits, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetValidModulesTypesAsync([.. subscriptions], cancellationToken);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0], Is.EqualTo(module.Type));
    }

    [Test]
    public async Task GetValidModulesTypesAsyncHandlesMultipleValidModules()
    {
        var subscription = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var moduleTypes = new[] { ModuleType.Accounting, ModuleType.Hr, ModuleType.Ecommerce };
        var modules = moduleTypes.Select(type => moduleGenerator.Clone().RuleFor(m => m.Type, type).Generate()).ToList();

        var credits = modules.Select(module => creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscription).RuleFor(c => c.ModuleId, module.Id).RuleFor(c => c.StartDate, now.AddDays(-10)).RuleFor(c => c.EndDate, now.AddDays(10)).Generate()).ToList();

        await context.Modules.AddRangeAsync(modules, cancellationToken);
        await context.SubscriptionCredits.AddRangeAsync(credits, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetValidModulesTypesAsync([subscription], cancellationToken);

        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result, Is.Unique);
        Assert.That(result, Is.EquivalentTo(modules.Select(m => m.Type)));
    }

    private void SetupFakers()
    {
        moduleGenerator = new Faker<ModuleModel>().RuleFor(m => m.Id, _ => Guid.NewGuid()).RuleFor(m => m.Type, f => f.PickRandom<ModuleType>()).RuleFor(m => m.Name, f => f.Commerce.ProductName());

        creditGenerator = new Faker<SubscriptionCreditModel>().RuleFor(c => c.Id, _ => Guid.NewGuid()).RuleFor(c => c.SubscriptionId, _ => Guid.NewGuid()).RuleFor(c => c.ModuleId, _ => Guid.NewGuid()).RuleFor(c => c.IsActive, _ => true).RuleFor(c => c.StartDate, f => f.Date.Past()).RuleFor(c => c.EndDate, f => f.Date.Future());
    }
}
