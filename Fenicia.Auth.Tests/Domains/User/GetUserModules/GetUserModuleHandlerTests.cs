using Fenicia.Auth.Domains.User.GetUserModules;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User.GetUserModules;

[TestFixture]
public class GetUserModuleHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.handler = new GetUserModuleHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private DefaultContext context = null!;
    private GetUserModuleHandler handler = null!;

    [Test]
    public async Task Handler_WhenUserHasActiveSubscription_ReturnsModules()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var subscriptionCreditId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();
        var submoduleId = Guid.NewGuid();

        var now = DateTime.UtcNow;

        var module = new AuthModuleModel
        {
            Id = moduleId,
            Name = "Test Module",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var subscription = new AuthSubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = companyId,
            Status = SubscriptionStatus.Active,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderId = Guid.NewGuid()
        };

        var subscriptionCredit = new AuthSubscriptionCreditModel
        {
            Id = subscriptionCreditId,
            ModuleId = moduleId,
            SubscriptionId = subscriptionId,
            IsActive = true,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderDetailId = Guid.NewGuid()
        };

        var userRole = new AuthUserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        var submodule = new AuthSubmoduleModel
        {
            Id = submoduleId,
            ModuleId = moduleId,
            Name = "Test Submodule",
            Description = "Test Description",
            Route = "/test"
        };

        this.context.Modules.Add(module);
        this.context.Subscriptions.Add(subscription);
        this.context.SubscriptionCredits.Add(subscriptionCredit);
        this.context.UserRoles.Add(userRole);
        this.context.Submodules.Add(submodule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetUserModulesQuery(companyId, userId);

        // Act
        var result = await this.handler.Handler(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1), "Should return 1 module");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Id, Is.EqualTo(moduleId), "ModuleId should match");
            Assert.That(result[0].Name, Is.EqualTo("Test Module"), "ModuleName should match");
            Assert.That(result[0].Type, Is.EqualTo(ModuleType.Accounting), "ModuleType should match");
        }
    }

    [Test]
    public async Task Handler_WhenUserHasNoSubscription_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var query = new GetUserModulesQuery(companyId, userId);

        // Act
        var result = await this.handler.Handler(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty, "Should return empty list");
    }

    [Test]
    public async Task Handler_WhenSubscriptionIsInactive_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var subscriptionCreditId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();
        var submoduleId = Guid.NewGuid();

        var now = DateTime.UtcNow;

        var module = new AuthModuleModel
        {
            Id = moduleId,
            Name = "Test Module",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var subscription = new AuthSubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = companyId,
            Status = SubscriptionStatus.Inactive,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderId = Guid.NewGuid()
        };

        var subscriptionCredit = new AuthSubscriptionCreditModel
        {
            Id = subscriptionCreditId,
            ModuleId = moduleId,
            SubscriptionId = subscriptionId,
            IsActive = true,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderDetailId = Guid.NewGuid()
        };

        var userRole = new AuthUserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        var submodule = new AuthSubmoduleModel
        {
            Id = submoduleId,
            ModuleId = moduleId,
            Name = "Test Submodule",
            Description = "Test Description",
            Route = "/test"
        };

        this.context.Modules.Add(module);
        this.context.Subscriptions.Add(subscription);
        this.context.SubscriptionCredits.Add(subscriptionCredit);
        this.context.UserRoles.Add(userRole);
        this.context.Submodules.Add(submodule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetUserModulesQuery(companyId, userId);

        // Act
        var result = await this.handler.Handler(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty, "Should return empty list for inactive subscription");
    }

    [Test]
    public async Task Handler_WhenSubscriptionCreditIsInactive_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var subscriptionCreditId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();
        var submoduleId = Guid.NewGuid();

        var now = DateTime.UtcNow;

        var module = new AuthModuleModel
        {
            Id = moduleId,
            Name = "Test Module",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var subscription = new AuthSubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = companyId,
            Status = SubscriptionStatus.Active,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderId = Guid.NewGuid()
        };

        var subscriptionCredit = new AuthSubscriptionCreditModel
        {
            Id = subscriptionCreditId,
            ModuleId = moduleId,
            SubscriptionId = subscriptionId,
            IsActive = false,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderDetailId = Guid.NewGuid()
        };

        var userRole = new AuthUserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        var submodule = new AuthSubmoduleModel
        {
            Id = submoduleId,
            ModuleId = moduleId,
            Name = "Test Submodule",
            Description = "Test Description",
            Route = "/test"
        };

        this.context.Modules.Add(module);
        this.context.Subscriptions.Add(subscription);
        this.context.SubscriptionCredits.Add(subscriptionCredit);
        this.context.UserRoles.Add(userRole);
        this.context.Submodules.Add(submodule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetUserModulesQuery(companyId, userId);

        // Act
        var result = await this.handler.Handler(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty, "Should return empty list for inactive credit");
    }

    [Test]
    public async Task Handler_WhenSubscriptionIsExpired_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var subscriptionCreditId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();
        var submoduleId = Guid.NewGuid();

        var now = DateTime.UtcNow;

        var module = new AuthModuleModel
        {
            Id = moduleId,
            Name = "Test Module",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var subscription = new AuthSubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = companyId,
            Status = SubscriptionStatus.Active,
            StartDate = now.AddDays(-30),
            EndDate = now.AddDays(-10),
            OrderId = Guid.NewGuid()
        };

        var subscriptionCredit = new AuthSubscriptionCreditModel
        {
            Id = subscriptionCreditId,
            ModuleId = moduleId,
            SubscriptionId = subscriptionId,
            IsActive = true,
            StartDate = now.AddDays(-30),
            EndDate = now.AddDays(-10),
            OrderDetailId = Guid.NewGuid()
        };

        var userRole = new AuthUserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        var submodule = new AuthSubmoduleModel
        {
            Id = submoduleId,
            ModuleId = moduleId,
            Name = "Test Submodule",
            Description = "Test Description",
            Route = "/test"
        };

        this.context.Modules.Add(module);
        this.context.Subscriptions.Add(subscription);
        this.context.SubscriptionCredits.Add(subscriptionCredit);
        this.context.UserRoles.Add(userRole);
        this.context.Submodules.Add(submodule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetUserModulesQuery(companyId, userId);

        // Act
        var result = await this.handler.Handler(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty, "Should return empty list for expired subscription");
    }

    [Test]
    public async Task Handler_WhenUserHasMultipleModules_ReturnsAllModules()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var module1Id = Guid.NewGuid();
        var module2Id = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();

        var now = DateTime.UtcNow;

        var module1 = new AuthModuleModel
        {
            Id = module1Id,
            Name = "Module 1",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var module2 = new AuthModuleModel
        {
            Id = module2Id,
            Name = "Module 2",
            Type = ModuleType.Contracts,
            Price = 150.00m
        };

        var subscription = new AuthSubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = companyId,
            Status = SubscriptionStatus.Active,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderId = Guid.NewGuid()
        };

        var credit1 = new AuthSubscriptionCreditModel
        {
            ModuleId = module1Id,
            SubscriptionId = subscriptionId,
            IsActive = true,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderDetailId = Guid.NewGuid()
        };

        var credit2 = new AuthSubscriptionCreditModel
        {
            ModuleId = module2Id,
            SubscriptionId = subscriptionId,
            IsActive = true,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderDetailId = Guid.NewGuid()
        };

        var userRole = new AuthUserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        this.context.Modules.AddRange(module1, module2);
        this.context.Subscriptions.Add(subscription);
        this.context.SubscriptionCredits.AddRange(credit1, credit2);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetUserModulesQuery(companyId, userId);

        // Act
        var result = await this.handler.Handler(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2), "Should return 2 modules");
    }

    [Test]
    public async Task Handler_WhenUserIsNotInCompany_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var differentCompanyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();

        var now = DateTime.UtcNow;

        var module = new AuthModuleModel
        {
            Id = moduleId,
            Name = "Test Module",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var subscription = new AuthSubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = differentCompanyId,
            Status = SubscriptionStatus.Active,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderId = Guid.NewGuid()
        };

        var subscriptionCredit = new AuthSubscriptionCreditModel
        {
            ModuleId = moduleId,
            SubscriptionId = subscriptionId,
            IsActive = true,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderDetailId = Guid.NewGuid()
        };

        var userRole = new AuthUserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = differentCompanyId,
            RoleId = Guid.NewGuid()
        };

        this.context.Modules.Add(module);
        this.context.Subscriptions.Add(subscription);
        this.context.SubscriptionCredits.Add(subscriptionCredit);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetUserModulesQuery(companyId, userId);

        // Act
        var result = await this.handler.Handler(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty, "Should return empty list when user is not in company");
    }

    [Test]
    public async Task Handler_RemovesDuplicateModules()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();

        var now = DateTime.UtcNow;

        var module = new AuthModuleModel
        {
            Id = moduleId,
            Name = "Test Module",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var subscription = new AuthSubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = companyId,
            Status = SubscriptionStatus.Active,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderId = Guid.NewGuid()
        };

        var credit1 = new AuthSubscriptionCreditModel
        {
            ModuleId = moduleId,
            SubscriptionId = subscriptionId,
            IsActive = true,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderDetailId = Guid.NewGuid()
        };

        var credit2 = new AuthSubscriptionCreditModel
        {
            ModuleId = moduleId,
            SubscriptionId = subscriptionId,
            IsActive = true,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderDetailId = Guid.NewGuid()
        };

        var userRole = new AuthUserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        this.context.Modules.Add(module);
        this.context.Subscriptions.Add(subscription);
        this.context.SubscriptionCredits.AddRange(credit1, credit2);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetUserModulesQuery(companyId, userId);

        // Act
        var result = await this.handler.Handler(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1), "Should return unique modules only");
    }
}
