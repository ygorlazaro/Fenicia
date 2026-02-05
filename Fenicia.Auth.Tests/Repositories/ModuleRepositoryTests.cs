using Bogus;

using Fenicia.Auth.Domains.Module;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Enums;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

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
        options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        context = new AuthContext(options);
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
    public async Task GetAllOrderedAsyncReturnsModulesWithPagination()
    {
        var modules = new List<ModuleModel>();
        for (var i = 0; i < 15; i++)
        {
            modules.Add(new ModuleModel
            {
                Id = Guid.NewGuid(),
                Type = (ModuleType)((i % 5) + 1),
                Name = faker.Commerce.ProductName()
            });
        }

        await context.Modules.AddRangeAsync(modules, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var page1 = await sut.GetAllAsync(cancellationToken, 1, 10);
        var page2 = await sut.GetAllAsync(cancellationToken, 2, 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page1, Has.Count.EqualTo(10));
            Assert.That(page2, Has.Count.EqualTo(5));
        }

        Assert.That(page1, Is.Ordered.By("Type"));
    }

    [Test]
    public async Task GetAllOrderedAsyncReturnsEmptyListWhenNoModules()
    {
        var result = await sut.GetAllAsync(cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetManyOrdersAsyncReturnsRequestedModules()
    {
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

        var result = await sut.GetManyOrdersAsync(requestedIDs, cancellationToken);

        Assert.That(result, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Select(m => m.Id), Is.EquivalentTo(requestedIDs));
            Assert.That(result, Is.Ordered.By("Type"));
        }
    }

    [Test]
    public async Task GetManyOrdersAsyncReturnsEmptyListWhenNoMatchingIDs()
    {
        var nonExistentIDs = new[] { Guid.NewGuid(), Guid.NewGuid() };

        var result = await sut.GetManyOrdersAsync(nonExistentIDs, cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetUserModulesAsyncReturnsModulesForActiveSubscription()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var module = new ModuleModel { Id = Guid.NewGuid(), Name = "M1", Type = ModuleType.Basic };
        await context.Modules.AddAsync(module, cancellationToken);

        var subscription = new SubscriptionModel { Id = Guid.NewGuid(), CompanyId = companyId, Status = SubscriptionStatus.Active, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1) };
        await context.Subscriptions.AddAsync(subscription, cancellationToken);

        var credit = new SubscriptionCreditModel { Id = Guid.NewGuid(), SubscriptionId = subscription.Id, ModuleId = module.Id, IsActive = true, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1) };
        await context.SubscriptionCredits.AddAsync(credit, cancellationToken);

        var userRole = new UserRoleModel { UserId = userId, CompanyId = companyId };
        await context.UserRoles.AddAsync(userRole, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetUserModulesAsync(userId, companyId, cancellationToken);

        Assert.That(result, Is.Not.Empty);
        Assert.That(result.First().Id, Is.EqualTo(module.Id));
    }

    [Test]
    public async Task GetModuleAndSubmoduleAsync_IncludesSubmodules()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var module = new ModuleModel { Id = Guid.NewGuid(), Name = "M2", Type = ModuleType.Basic };
        var sub = new SubmoduleModel { Id = Guid.NewGuid(), Name = "s", Route = "/s", ModuleId = module.Id };
        module.Submodules = [sub];

        await context.Modules.AddAsync(module, cancellationToken);

        var subscription = new SubscriptionModel { Id = Guid.NewGuid(), CompanyId = companyId, Status = SubscriptionStatus.Active, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1) };
        await context.Subscriptions.AddAsync(subscription, cancellationToken);

        var credit = new SubscriptionCreditModel { Id = Guid.NewGuid(), SubscriptionId = subscription.Id, ModuleId = module.Id, IsActive = true, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1) };
        await context.SubscriptionCredits.AddAsync(credit, cancellationToken);

        var userRole = new UserRoleModel { UserId = userId, CompanyId = companyId };
        await context.UserRoles.AddAsync(userRole, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetModuleAndSubmoduleAsync(userId, companyId, cancellationToken);

        Assert.That(result, Is.Not.Empty);
        Assert.That(result.First().Submodules, Is.Not.Null.And.Not.Empty);
        Assert.That(result.First().Submodules.First().Id, Is.EqualTo(sub.Id));
    }

    [Test]
    public async Task GetModuleByTypeAsyncReturnsModuleWhenExists()
    {
        var moduleType = ModuleType.Accounting;
        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Type = moduleType,
            Name = faker.Commerce.ProductName()
        };

        await context.Modules.AddAsync(module, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetModuleByTypeAsync(moduleType, cancellationToken);

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Type, Is.EqualTo(moduleType));
            Assert.That(result.Id, Is.EqualTo(module.Id));
        }
    }

    [Test]
    public async Task GetModuleByTypeAsyncReturnsNullWhenNotExists()
    {
        var result = await sut.GetModuleByTypeAsync(ModuleType.Accounting, cancellationToken);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CountAsyncReturnsCorrectCount()
    {
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

        var count = await sut.CountAsync(cancellationToken);

        Assert.That(count, Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task CountAsyncReturnsZeroWhenNoModules()
    {
        var count = await sut.CountAsync(cancellationToken);

        Assert.That(count, Is.Zero);
    }

    [Test]
    public async Task LoadModulesAtDatabaseAsync_SavesAndReturnsAllOrderedByType()
    {
        var modules = new List<ModuleModel>
        {
            new () { Id = Guid.NewGuid(), Name = "B", Type = ModuleType.Basic },
            new () { Id = Guid.NewGuid(), Name = "A", Type = ModuleType.Accounting },
            new () { Id = Guid.NewGuid(), Name = "P", Type = ModuleType.Pos }
        };

        var result = await sut.LoadModulesAtDatabaseAsync(modules, cancellationToken);

        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result, Is.Ordered.By("Type"));
        Assert.That(result.Select(m => m.Id), Is.EquivalentTo(modules.Select(m => m.Id)));
    }

    [Test]
    public async Task GetAllOrderedAsyncRespectsPaginationWithDifferentPageSizes()
    {
        var modules = new List<ModuleModel>();
        for (var i = 0; i < 25; i++)
        {
            modules.Add(new ModuleModel
            {
                Id = Guid.NewGuid(),
                Type = (ModuleType)((i % 5) + 1),
                Name = faker.Commerce.ProductName()
            });
        }

        await context.Modules.AddRangeAsync(modules, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var page1Size5 = await sut.GetAllAsync(cancellationToken, 1, 5);
        Assert.That(page1Size5, Has.Count.EqualTo(5));

        var page2Size15 = await sut.GetAllAsync(cancellationToken, 2, 15);
        Assert.That(page2Size15, Has.Count.EqualTo(10));
    }
}
