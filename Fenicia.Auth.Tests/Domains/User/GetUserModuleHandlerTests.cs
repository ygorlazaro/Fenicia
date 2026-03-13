using Fenicia.Auth.Domains.Module.Handlers;
using Fenicia.Auth.Domains.Module.Queries;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class GetUserModuleHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly GetUserModuleHandler handler;

    public GetUserModuleHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options, new TestCompanyContext());
        this.handler = new GetUserModuleHandler(this.db);
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handler_WhenUserHasActiveSubscription_ReturnsModules()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var subscriptionCreditId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();

        var now = DateTime.UtcNow;

        var module = new ModuleModel
        {
            Id = moduleId,
            Name = "Test Module",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var subscription = new SubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = companyId,
            Status = SubscriptionStatus.Active,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderId = Guid.NewGuid()
        };

        var subscriptionCredit = new SubscriptionCreditModel
        {
            Id = subscriptionCreditId,
            ModuleId = moduleId,
            SubscriptionId = subscriptionId,
            IsActive = true,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderDetailId = Guid.NewGuid()
        };

        var userRole = new UserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        this.db.AuthModules.Add(module);
        this.db.AuthSubscriptions.Add(subscription);
        this.db.AuthSubscriptionCredits.Add(subscriptionCredit);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetUserModulesQuery(companyId,
            userId);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        
        Assert.Equal(moduleId,
            result[0].Id);
        Assert.Equal("Test Module",
            result[0].Name);
        Assert.Equal(ModuleType.Accounting,
            result[0].Type);
    }

    [Fact]
    public async Task Handler_WhenUserHasNoSubscription_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var query = new GetUserModulesQuery(companyId,
            userId);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handler_WhenSubscriptionIsInactive_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var subscriptionCreditId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();

        var now = DateTime.UtcNow;

        var module = new ModuleModel
        {
            Id = moduleId,
            Name = "Test Module",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var subscription = new SubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = companyId,
            Status = SubscriptionStatus.Inactive,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderId = Guid.NewGuid()
        };

        var subscriptionCredit = new SubscriptionCreditModel
        {
            Id = subscriptionCreditId,
            ModuleId = moduleId,
            SubscriptionId = subscriptionId,
            IsActive = true,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderDetailId = Guid.NewGuid()
        };

        var userRole = new UserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        this.db.AuthModules.Add(module);
        this.db.AuthSubscriptions.Add(subscription);
        this.db.AuthSubscriptionCredits.Add(subscriptionCredit);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetUserModulesQuery(companyId,
            userId);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handler_WhenSubscriptionCreditIsInactive_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var subscriptionCreditId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();

        var now = DateTime.UtcNow;

        var module = new ModuleModel
        {
            Id = moduleId,
            Name = "Test Module",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var subscription = new SubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = companyId,
            Status = SubscriptionStatus.Active,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderId = Guid.NewGuid()
        };

        var subscriptionCredit = new SubscriptionCreditModel
        {
            Id = subscriptionCreditId,
            ModuleId = moduleId,
            SubscriptionId = subscriptionId,
            IsActive = false,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderDetailId = Guid.NewGuid()
        };

        var userRole = new UserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        this.db.AuthModules.Add(module);
        this.db.AuthSubscriptions.Add(subscription);
        this.db.AuthSubscriptionCredits.Add(subscriptionCredit);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetUserModulesQuery(companyId,
            userId);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handler_WhenSubscriptionIsExpired_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var subscriptionCreditId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();

        var now = DateTime.UtcNow;

        var module = new ModuleModel
        {
            Id = moduleId,
            Name = "Test Module",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var subscription = new SubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = companyId,
            Status = SubscriptionStatus.Active,
            StartDate = now.AddDays(-30),
            EndDate = now.AddDays(-10),
            OrderId = Guid.NewGuid()
        };

        var subscriptionCredit = new SubscriptionCreditModel
        {
            Id = subscriptionCreditId,
            ModuleId = moduleId,
            SubscriptionId = subscriptionId,
            IsActive = true,
            StartDate = now.AddDays(-30),
            EndDate = now.AddDays(-10),
            OrderDetailId = Guid.NewGuid()
        };

        var userRole = new UserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        this.db.AuthModules.Add(module);
        this.db.AuthSubscriptions.Add(subscription);
        this.db.AuthSubscriptionCredits.Add(subscriptionCredit);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetUserModulesQuery(companyId,
            userId);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
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

        var module1 = new ModuleModel
        {
            Id = module1Id,
            Name = "Module 1",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var module2 = new ModuleModel
        {
            Id = module2Id,
            Name = "Module 2",
            Type = ModuleType.Contracts,
            Price = 150.00m
        };

        var subscription = new SubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = companyId,
            Status = SubscriptionStatus.Active,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderId = Guid.NewGuid()
        };

        var credit1 = new SubscriptionCreditModel
        {
            ModuleId = module1Id,
            SubscriptionId = subscriptionId,
            IsActive = true,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderDetailId = Guid.NewGuid()
        };

        var credit2 = new SubscriptionCreditModel
        {
            ModuleId = module2Id,
            SubscriptionId = subscriptionId,
            IsActive = true,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderDetailId = Guid.NewGuid()
        };

        var userRole = new UserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        this.db.AuthModules.AddRange(module1,
            module2);
        this.db.AuthSubscriptions.Add(subscription);
        this.db.AuthSubscriptionCredits.AddRange(credit1,
            credit2);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetUserModulesQuery(companyId,
            userId);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2,
            result.Count);
    }

    [Fact]
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

        var module = new ModuleModel
        {
            Id = moduleId,
            Name = "Test Module",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var subscription = new SubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = differentCompanyId,
            Status = SubscriptionStatus.Active,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderId = Guid.NewGuid()
        };

        var subscriptionCredit = new SubscriptionCreditModel
        {
            ModuleId = moduleId,
            SubscriptionId = subscriptionId,
            IsActive = true,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderDetailId = Guid.NewGuid()
        };

        var userRole = new UserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = differentCompanyId,
            RoleId = Guid.NewGuid()
        };

        this.db.AuthModules.Add(module);
        this.db.AuthSubscriptions.Add(subscription);
        this.db.AuthSubscriptionCredits.Add(subscriptionCredit);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetUserModulesQuery(companyId,
            userId);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handler_RemovesDuplicateModules()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();

        var now = DateTime.UtcNow;

        var module = new ModuleModel
        {
            Id = moduleId,
            Name = "Test Module",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var subscription = new SubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = companyId,
            Status = SubscriptionStatus.Active,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderId = Guid.NewGuid()
        };

        var credit1 = new SubscriptionCreditModel
        {
            ModuleId = moduleId,
            SubscriptionId = subscriptionId,
            IsActive = true,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderDetailId = Guid.NewGuid()
        };

        var credit2 = new SubscriptionCreditModel
        {
            ModuleId = moduleId,
            SubscriptionId = subscriptionId,
            IsActive = true,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderDetailId = Guid.NewGuid()
        };

        var userRole = new UserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        this.db.AuthModules.Add(module);
        this.db.AuthSubscriptions.Add(subscription);
        this.db.AuthSubscriptionCredits.AddRange(credit1,
            credit2);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetUserModulesQuery(companyId,
            userId);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }
}
