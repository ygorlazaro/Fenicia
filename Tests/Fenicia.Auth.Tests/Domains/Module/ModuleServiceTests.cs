using Bogus;
using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Module;

public class ModuleServiceTests
{
    private readonly Faker _faker;
    private readonly Mock<IModuleRepository> _mockModuleRepository;
    private readonly Mock<ISubscriptionService> _mockSubscriptionService;
    private readonly Mock<IUserRoleService> _mockUserRoleService;
    private readonly ModuleService _service;

    public ModuleServiceTests()
    {
        _faker = new Faker();
        _mockModuleRepository = new Mock<IModuleRepository>();
        _mockUserRoleService = new Mock<IUserRoleService>();
        _mockSubscriptionService = new Mock<ISubscriptionService>();
        _mockModuleRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _service = new ModuleService(
            _mockModuleRepository.Object,
            _mockUserRoleService.Object,
            _mockSubscriptionService.Object);
    }

    [Fact]
    public async Task GetAllModulesAsync_WhenModulesExist_ReturnsPaginatedModules()
    {
        var module1 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 1
        };

        var module2 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.SocialNetwork,
            Price = 20.0m,
            IsActive = true,
            SortOrder = 2
        };

        _mockModuleRepository.Setup(r => r.Query())
            .Returns(new List<ModuleModel> { module1, module2 }.AsAsyncQueryable());

        var result = await _service.GetAllModulesAsync(new PaginationQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(2, result.Total);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PerPage);
    }

    [Fact]
    public async Task GetAllModulesAsync_ExcludesErpAndAuthTypes()
    {
        var authModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Auth,
            Price = 50.0m,
            IsActive = true,
            SortOrder = 1
        };

        var basicModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 2
        };

        _mockModuleRepository.Setup(r => r.Query())
            .Returns(new List<ModuleModel> { authModule, basicModule }.AsAsyncQueryable());

        var result = await _service.GetAllModulesAsync(new PaginationQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(basicModule.Name, result.Data[0].Name);
        Assert.Equal(1, result.Total);
    }

    [Fact]
    public async Task GetAllModulesAsync_ExcludesInactiveModules()
    {
        var activeModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Active Module",
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 1
        };

        var inactiveModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Inactive Module",
            Type = ModuleType.SocialNetwork,
            Price = 20.0m,
            IsActive = false,
            SortOrder = 2
        };

        _mockModuleRepository.Setup(r => r.Query())
            .Returns(new List<ModuleModel> { activeModule, inactiveModule }.AsAsyncQueryable());

        var result = await _service.GetAllModulesAsync(new PaginationQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(activeModule.Name, result.Data[0].Name);
    }

    [Fact]
    public async Task GetAllModulesAsync_WhenPaginationIsApplied_ReturnsCorrectPage()
    {
        var modules = new List<ModuleModel>();
        for (var i = 0; i < 25; i++)
        {
            modules.Add(
                new ModuleModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"Module {_faker.Commerce.ProductName()} {i}",
                    Type = (ModuleType)((i % 10) + 1),
                    Price = 10.0m,
                    IsActive = true,
                    SortOrder = i
                });
        }

        _mockModuleRepository.Setup(r => r.Query()).Returns(modules.AsAsyncQueryable());

        var result = await _service.GetAllModulesAsync(new PaginationQuery(2), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
        Assert.Equal(25, result.Total);
        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PerPage);
        Assert.Equal(3, result.Pages);
    }

    [Fact]
    public async Task GetAllModulesAsync_WhenNoModulesExist_ReturnsEmptyPagination()
    {
        _mockModuleRepository.Setup(r => r.Query()).Returns(new List<ModuleModel>().AsAsyncQueryable());

        var result = await _service.GetAllModulesAsync(new PaginationQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task GetAllModulesAsync_WhenPageExceedsTotalPages_ReturnsEmptyData()
    {
        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Basic Module",
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 1
        };

        _mockModuleRepository.Setup(r => r.Query()).Returns(new List<ModuleModel> { module }.AsAsyncQueryable());

        var result = await _service.GetAllModulesAsync(new PaginationQuery(10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(1, result.Total);
    }

    [Fact]
    public async Task GetAllModulesAsync_ResultsAreOrderedBySortOrder()
    {
        var module1 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Social Network Module",
            Type = ModuleType.SocialNetwork,
            Price = 20.0m,
            IsActive = true,
            SortOrder = 3
        };

        var module2 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Basic Module",
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 1
        };

        var module3 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "HR Module",
            Type = ModuleType.Hr,
            Price = 30.0m,
            IsActive = true,
            SortOrder = 2
        };

        _mockModuleRepository.Setup(r => r.Query())
            .Returns(new List<ModuleModel> { module1, module2, module3 }.AsAsyncQueryable());

        var result = await _service.GetAllModulesAsync(new PaginationQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.Data.Count);
        Assert.Equal("Basic Module", result.Data[0].Name);
        Assert.Equal(1, result.Data[0].SortOrder);
        Assert.Equal("HR Module", result.Data[1].Name);
        Assert.Equal(2, result.Data[1].SortOrder);
        Assert.Equal("Social Network Module", result.Data[2].Name);
        Assert.Equal(3, result.Data[2].SortOrder);
    }

    [Fact]
    public async Task GetAllModulesAsync_WithDefaultRequest_ReturnsFirstPage()
    {
        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 1
        };

        _mockModuleRepository.Setup(r => r.Query()).Returns(new List<ModuleModel> { module }.AsAsyncQueryable());

        var result = await _service.GetAllModulesAsync(new PaginationQuery(1, 20), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PerPage);
    }

    [Fact]
    public async Task GetAllModulesAsync_VerifiesResponseContainsAllFields()
    {
        var moduleId = Guid.NewGuid();
        const string description = "Test module description";
        const string icon = "icon-test";
        const int sortOrder = 5;

        var module = new ModuleModel
        {
            Id = moduleId,
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            Description = description,
            Icon = icon,
            IsActive = true,
            SortOrder = sortOrder
        };

        _mockModuleRepository.Setup(r => r.Query()).Returns(new List<ModuleModel> { module }.AsAsyncQueryable());

        var result = await _service.GetAllModulesAsync(new PaginationQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Data);
        var moduleResponse = result.Data[0];

        Assert.Equal(moduleId, moduleResponse.Id);
        Assert.Equal(module.Name, moduleResponse.Name);
        Assert.Equal(ModuleType.Basic, moduleResponse.Type);
        Assert.Equal(description, moduleResponse.Description);
        Assert.Equal(icon, moduleResponse.Icon);
        Assert.True(moduleResponse.IsActive);
        Assert.Equal(sortOrder, moduleResponse.SortOrder);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserHasActiveSubscription_ReturnsModules()
    {
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

        var userRole = new UserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        _mockUserRoleService.Setup(s => s.GetUserRoleAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userRole);
        _mockSubscriptionService
            .Setup(s => s.GetActiveSubscriptionsByCompanyAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([subscription]);
        _mockSubscriptionService.Setup(s =>
                s.GetActiveModulesForSubscriptionAsync(subscription.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([module]);
        _mockModuleRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([module]);

        var result = await _service.GetUserModulesAsync(companyId, userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(moduleId, result[0].Id);
        Assert.Equal("Test Module", result[0].Name);
        Assert.Equal(ModuleType.Accounting, result[0].Type);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserHasNoSubscription_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        _mockUserRoleService.Setup(s => s.GetUserRoleAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserRoleModel?)null);

        var result = await _service.GetUserModulesAsync(companyId, userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenSubscriptionIsInactive_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        _mockUserRoleService.Setup(s => s.GetUserRoleAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new UserRoleModel
                    { Id = Guid.NewGuid(), UserId = userId, CompanyId = companyId, RoleId = Guid.NewGuid() });
        _mockSubscriptionService
            .Setup(s => s.GetActiveSubscriptionsByCompanyAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _service.GetUserModulesAsync(companyId, userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenSubscriptionCreditIsInactive_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        Guid.NewGuid();
        var userRoleId = Guid.NewGuid();

        var now = DateTime.UtcNow;

        var subscription = new SubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = companyId,
            Status = SubscriptionStatus.Active,
            StartDate = now.AddDays(-10),
            EndDate = now.AddDays(20),
            OrderId = Guid.NewGuid()
        };

        var userRole = new UserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        _mockUserRoleService.Setup(s => s.GetUserRoleAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userRole);
        _mockSubscriptionService
            .Setup(s => s.GetActiveSubscriptionsByCompanyAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([subscription]);
        _mockSubscriptionService.Setup(s =>
                s.GetActiveModulesForSubscriptionAsync(subscription.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _service.GetUserModulesAsync(companyId, userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenSubscriptionIsExpired_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        _mockUserRoleService.Setup(s => s.GetUserRoleAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new UserRoleModel
                    { Id = Guid.NewGuid(), UserId = userId, CompanyId = companyId, RoleId = Guid.NewGuid() });
        _mockSubscriptionService
            .Setup(s => s.GetActiveSubscriptionsByCompanyAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _service.GetUserModulesAsync(companyId, userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserHasMultipleModules_ReturnsAllModules()
    {
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

        var userRole = new UserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        _mockUserRoleService.Setup(s => s.GetUserRoleAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userRole);
        _mockSubscriptionService
            .Setup(s => s.GetActiveSubscriptionsByCompanyAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([subscription]);
        _mockSubscriptionService.Setup(s =>
                s.GetActiveModulesForSubscriptionAsync(subscription.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([module1, module2]);
        _mockModuleRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([module1, module2]);

        var result = await _service.GetUserModulesAsync(companyId, userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserIsNotInCompany_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        Guid.NewGuid();
        Guid.NewGuid();
        Guid.NewGuid();
        Guid.NewGuid();

        _mockUserRoleService.Setup(s => s.GetUserRoleAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserRoleModel?)null);

        var result = await _service.GetUserModulesAsync(companyId, userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserModulesAsync_RemovesDuplicateModules()
    {
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

        var userRole = new UserRoleModel
        {
            Id = userRoleId,
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        _mockUserRoleService.Setup(s => s.GetUserRoleAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userRole);
        _mockSubscriptionService
            .Setup(s => s.GetActiveSubscriptionsByCompanyAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([subscription]);
        _mockSubscriptionService.Setup(s =>
                s.GetActiveModulesForSubscriptionAsync(subscription.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([module, module]);
        _mockModuleRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([module]);

        var result = await _service.GetUserModulesAsync(companyId, userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
    }
}