using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.Order.DTOs;
using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Order;

public class OrderServiceTests
{
    private readonly Mock<IModuleService> _mockModuleService;
    private readonly Mock<IRepository<OrderModel>> _mockOrderRepository;
    private readonly Mock<ISubscriptionService> _mockSubscriptionService;
    private readonly Mock<IUserRoleService> _mockUserRoleService;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _mockModuleService = new Mock<IModuleService>();
        _mockOrderRepository = new Mock<IRepository<OrderModel>>();
        _mockSubscriptionService = new Mock<ISubscriptionService>();
        _mockUserRoleService = new Mock<IUserRoleService>();
        _service = new OrderService(_mockModuleService.Object, _mockOrderRepository.Object, _mockSubscriptionService.Object, _mockUserRoleService.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenUserDoesNotBelongToCompany_ThrowsPermissionDeniedException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var modules = new List<Guid> { Guid.NewGuid() };

        _mockUserRoleService.Setup(s => s.AnyIdAndCompanyAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.CreateAsync(command, CancellationToken.None));
        Assert.Equal("User does not exists at the company", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenModulesNotFound_ThrowsItemNotExistsException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var modules = new List<Guid> { Guid.NewGuid() };

        _mockUserRoleService.Setup(s => s.AnyIdAndCompanyAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockModuleService.Setup(s => s.GetModulesByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.CreateAsync(command, CancellationToken.None));
        Assert.Equal("Modules not found", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenNoModulesRequested_ThrowsItemNotExistsException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var modules = new List<Guid>();

        _mockUserRoleService.Setup(s => s.AnyIdAndCompanyAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockModuleService.Setup(s => s.GetModulesByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.CreateAsync(command, CancellationToken.None));
        Assert.Equal("Modules not found", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenModuleIsBasicType_DoesNotAddAnotherBasic()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var basicModuleId = Guid.NewGuid();
        var modules = new List<Guid> { basicModuleId };

        var basicModule = new ModuleModel
        {
            Id = basicModuleId,
            Name = nameof(ModuleType.Basic),
            Type = ModuleType.Basic,
            Price = 50.00m
        };

        _mockUserRoleService.Setup(s => s.AnyIdAndCompanyAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockModuleService.Setup(s => s.GetModulesByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([basicModule]);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var result = await _service.CreateAsync(command, CancellationToken.None);

        Assert.NotNull(result);
        _mockOrderRepository.Verify(r => r.InsertAsync(It.IsAny<OrderModel>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockSubscriptionService.Verify(s => s.CreateSubscriptionAsync(It.IsAny<SubscriptionModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenModuleIsNotBasic_AddsBasicModuleAutomatically()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var accountingModuleId = Guid.NewGuid();
        var basicModuleId = Guid.NewGuid();
        var modules = new List<Guid> { accountingModuleId };

        var accountingModule = new ModuleModel
        {
            Id = accountingModuleId,
            Name = nameof(ModuleType.Accounting),
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var basicModule = new ModuleModel
        {
            Id = basicModuleId,
            Name = nameof(ModuleType.Basic),
            Type = ModuleType.Basic,
            Price = 50.00m
        };

        _mockUserRoleService.Setup(s => s.AnyIdAndCompanyAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockModuleService.Setup(s => s.GetModulesByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountingModule]);
        _mockModuleService.Setup(s => s.GetModuleByTypeAsync(ModuleType.Basic, It.IsAny<CancellationToken>()))
            .ReturnsAsync(basicModule);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var result = await _service.CreateAsync(command, CancellationToken.None);

        Assert.NotNull(result);
        _mockOrderRepository.Verify(r => r.InsertAsync(It.IsAny<OrderModel>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockSubscriptionService.Verify(s => s.CreateSubscriptionAsync(It.IsAny<SubscriptionModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenBasicModuleNotFound_ThrowsItemNotExistsException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var accountingModuleId = Guid.NewGuid();
        var modules = new List<Guid> { accountingModuleId };

        var accountingModule = new ModuleModel
        {
            Id = accountingModuleId,
            Name = nameof(ModuleType.Accounting),
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        _mockUserRoleService.Setup(s => s.AnyIdAndCompanyAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockModuleService.Setup(s => s.GetModulesByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountingModule]);
        _mockModuleService.Setup(s => s.GetModuleByTypeAsync(ModuleType.Basic, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ModuleModel?)null);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.CreateAsync(command, CancellationToken.None));
        Assert.Equal("Modules not found", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenDuplicateModuleIds_RemovesDuplicates()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var module1Id = Guid.NewGuid();
        var modules = new List<Guid> { module1Id, module1Id, module1Id };

        var module1 = new ModuleModel
        {
            Id = module1Id,
            Name = nameof(ModuleType.Accounting),
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var basicModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = nameof(ModuleType.Basic),
            Type = ModuleType.Basic,
            Price = 50.00m
        };

        _mockUserRoleService.Setup(s => s.AnyIdAndCompanyAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockModuleService.Setup(s => s.GetModulesByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([module1]);
        _mockModuleService.Setup(s => s.GetModuleByTypeAsync(ModuleType.Basic, It.IsAny<CancellationToken>()))
            .ReturnsAsync(basicModule);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var result = await _service.CreateAsync(command, CancellationToken.None);

        Assert.NotNull(result);
        _mockOrderRepository.Verify(r => r.InsertAsync(It.IsAny<OrderModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
