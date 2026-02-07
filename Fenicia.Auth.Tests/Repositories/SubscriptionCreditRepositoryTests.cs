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
        this.options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        this.context = new AuthContext(this.options);
        this.sut = new SubscriptionCreditRepository(this.context);

        SetupFakers();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }

    [Test]
    public async Task GetValidModulesTypesAsyncReturnsValidModules()
    {
        var subscriptionIDs = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var now = DateTime.UtcNow;

        var modules = this.moduleGenerator.Generate(3);
        var credits = subscriptionIDs.Select(subscriptionId => this.creditGenerator.Clone()
            .RuleFor(c => c.SubscriptionId, subscriptionId).RuleFor(c => c.ModuleId, f => f.PickRandom(modules).Id)
            .RuleFor(c => c.StartDate, now.AddDays(-10)).RuleFor(c => c.EndDate, now.AddDays(10)).Generate()).ToList();

        await this.context.Modules.AddRangeAsync(modules, this.cancellationToken);
        await this.context.SubscriptionCredits.AddRangeAsync(credits, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetValidModulesTypesAsync([.. subscriptionIDs], this.cancellationToken);

        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Has.Count.LessThanOrEqualTo(modules.Count));
    }

    [Test]
    public async Task GetValidModulesTypesAsyncReturnsEmptyListWhenNoValidSubscriptions()
    {
        var nonExistentSubscriptions = new Faker().Make(3, Guid.NewGuid);

        var result = await this.sut.GetValidModulesTypesAsync([.. nonExistentSubscriptions], this.cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidModulesTypesAsyncExcludesInactiveCredits()
    {
        var subscription = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var module = this.moduleGenerator.Generate();
        var credit = this.creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscription)
            .RuleFor(c => c.ModuleId, module.Id).RuleFor(c => c.IsActive, false)
            .RuleFor(c => c.StartDate, now.AddDays(-10)).RuleFor(c => c.EndDate, now.AddDays(10)).Generate();

        await this.context.Modules.AddAsync(module, this.cancellationToken);
        await this.context.SubscriptionCredits.AddAsync(credit, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetValidModulesTypesAsync([subscription], this.cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidModulesTypesAsyncExcludesExpiredCredits()
    {
        var subscription = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var module = this.moduleGenerator.Generate();
        var credit = this.creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscription)
            .RuleFor(c => c.ModuleId, module.Id).RuleFor(c => c.StartDate, now.AddDays(-20))
            .RuleFor(c => c.EndDate, now.AddDays(-10)).Generate();

        await this.context.Modules.AddAsync(module, this.cancellationToken);
        await this.context.SubscriptionCredits.AddAsync(credit, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetValidModulesTypesAsync([subscription], this.cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidModulesTypesAsyncExcludesFutureCredits()
    {
        var subscription = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var module = this.moduleGenerator.Generate();
        var credit = this.creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscription)
            .RuleFor(c => c.ModuleId, module.Id).RuleFor(c => c.StartDate, now.AddDays(10))
            .RuleFor(c => c.EndDate, now.AddDays(20)).Generate();

        await this.context.Modules.AddAsync(module, this.cancellationToken);
        await this.context.SubscriptionCredits.AddAsync(credit, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetValidModulesTypesAsync([subscription], this.cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetValidModulesTypesAsyncReturnsDuplicateModuleTypesOnlyOnce()
    {
        var subscriptions = new Faker().Make(2, Guid.NewGuid);
        var now = DateTime.UtcNow;

        var module = this.moduleGenerator.Generate();
        var credits = subscriptions.Select(subscriptionId =>
            this.creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscriptionId)
                .RuleFor(c => c.ModuleId, module.Id).RuleFor(c => c.StartDate, now.AddDays(-10))
                .RuleFor(c => c.EndDate, now.AddDays(10)).Generate()).ToList();

        await this.context.Modules.AddAsync(module, this.cancellationToken);
        await this.context.SubscriptionCredits.AddRangeAsync(credits, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetValidModulesTypesAsync([.. subscriptions], this.cancellationToken);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0], Is.EqualTo(module.Type));
    }

    [Test]
    public async Task GetValidModulesTypesAsyncHandlesMultipleValidModules()
    {
        var subscription = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var moduleTypes = new[] { ModuleType.Accounting, ModuleType.Hr, ModuleType.Ecommerce };
        var modules = moduleTypes.Select(type => this.moduleGenerator.Clone().RuleFor(m => m.Type, type).Generate())
            .ToList();

        var credits = modules.Select(module =>
            this.creditGenerator.Clone().RuleFor(c => c.SubscriptionId, subscription)
                .RuleFor(c => c.ModuleId, module.Id).RuleFor(c => c.StartDate, now.AddDays(-10))
                .RuleFor(c => c.EndDate, now.AddDays(10)).Generate()).ToList();

        await this.context.Modules.AddRangeAsync(modules, this.cancellationToken);
        await this.context.SubscriptionCredits.AddRangeAsync(credits, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetValidModulesTypesAsync([subscription], this.cancellationToken);

        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result, Is.Unique);
        Assert.That(result, Is.EquivalentTo(modules.Select(m => m.Type)));
    }

    private void SetupFakers()
    {
        this.moduleGenerator = new Faker<ModuleModel>().RuleFor(m => m.Id, _ => Guid.NewGuid())
            .RuleFor(m => m.Type, f => f.PickRandom<ModuleType>()).RuleFor(m => m.Name, f => f.Commerce.ProductName());

        this.creditGenerator = new Faker<SubscriptionCreditModel>().RuleFor(c => c.Id, _ => Guid.NewGuid())
            .RuleFor(c => c.SubscriptionId, _ => Guid.NewGuid()).RuleFor(c => c.ModuleId, _ => Guid.NewGuid())
            .RuleFor(c => c.IsActive, _ => true).RuleFor(c => c.StartDate, f => f.Date.Past())
            .RuleFor(c => c.EndDate, f => f.Date.Future());
    }
}