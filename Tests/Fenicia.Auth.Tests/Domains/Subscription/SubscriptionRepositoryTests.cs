using Bogus;
using Bogus.Extensions.Brazil;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Tests.Domains.Security;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Subscription;

public class SubscriptionRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly SubscriptionRepository _repository;

    public SubscriptionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _repository = new SubscriptionRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetUserSubscriptionsAsync_WhenUserHasActiveSubscription_ReturnsSubscription()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = new TestSecurityService().Hash(_faker.Internet.Password())
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        var subscription = new SubscriptionModel
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(10)
        };

        _db.AuthCompanies.Add(company);
        _db.AuthRoles.Add(role);
        _db.AuthUsers.Add(user);
        _db.AuthUserRoles.Add(userRole);
        _db.AuthSubscriptions.Add(subscription);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetUserSubscriptionsAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetUserSubscriptionsAsync_WhenUserHasNoSubscriptions_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();

        var result = await _repository.GetUserSubscriptionsAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserSubscriptionsAsync_WhenSubscriptionIsInactive_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = new TestSecurityService().Hash(_faker.Internet.Password())
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        var subscription = new SubscriptionModel
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Status = SubscriptionStatus.Inactive,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(10)
        };

        _db.AuthCompanies.Add(company);
        _db.AuthRoles.Add(role);
        _db.AuthUsers.Add(user);
        _db.AuthUserRoles.Add(userRole);
        _db.AuthSubscriptions.Add(subscription);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetUserSubscriptionsAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSubscriptionModulesAsync_WhenSubscriptionHasActiveCredits_ReturnsModules()
    {
        var subscriptionId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var creditId = Guid.NewGuid();

        var module = new ModuleModel
        {
            Id = moduleId,
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = _faker.Finance.Amount(10, 100)
        };

        var subscription = new SubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = Guid.NewGuid(),
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(10)
        };

        var credit = new SubscriptionCreditModel
        {
            Id = creditId,
            SubscriptionId = subscriptionId,
            ModuleId = moduleId,
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(10)
        };

        _db.AuthModules.Add(module);
        _db.AuthSubscriptions.Add(subscription);
        _db.AuthSubscriptionCredits.Add(credit);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetSubscriptionModulesAsync(subscriptionId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(moduleId, result[0].Id);
    }

    [Fact]
    public async Task GetSubscriptionModulesAsync_WhenCreditIsInactive_ReturnsEmptyList()
    {
        var subscriptionId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var creditId = Guid.NewGuid();

        var module = new ModuleModel
        {
            Id = moduleId,
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = _faker.Finance.Amount(10, 100)
        };

        var subscription = new SubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = Guid.NewGuid(),
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(10)
        };

        var credit = new SubscriptionCreditModel
        {
            Id = creditId,
            SubscriptionId = subscriptionId,
            ModuleId = moduleId,
            IsActive = false,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(10)
        };

        _db.AuthModules.Add(module);
        _db.AuthSubscriptions.Add(subscription);
        _db.AuthSubscriptionCredits.Add(credit);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetSubscriptionModulesAsync(subscriptionId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}