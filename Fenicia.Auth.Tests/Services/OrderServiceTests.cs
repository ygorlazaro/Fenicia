using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Enums;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Migrations.Services;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class OrderServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Mock<IOrderRepository> orderRepositoryMock = null!;
    private Mock<IModuleService> moduleServiceMock = null!;
    private Mock<ISubscriptionService> subscriptionServiceMock = null!;
    private Mock<IUserService> userServiceMock = null!;
    private Mock<IMigrationService> migrationServiceMock = null!;

    private OrderService? sut;

    [SetUp]
    public void Setup()
    {
        orderRepositoryMock = new Mock<IOrderRepository>();
        moduleServiceMock = new Mock<IModuleService>();
        subscriptionServiceMock = new Mock<ISubscriptionService>();
        userServiceMock = new Mock<IUserService>();
        migrationServiceMock = new Mock<IMigrationService>();

        sut = new OrderService(orderRepositoryMock.Object, moduleServiceMock.Object, subscriptionServiceMock.Object, userServiceMock.Object, migrationServiceMock.Object);
    }

    [Test]
    public void CreateNewOrderAsync_ThrowsWhenUserNotInCompany()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var request = new OrderRequest { Details = [new OrderDetailRequest { ModuleId = Guid.NewGuid() }] };

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(false);

        Assert.ThrowsAsync<PermissionDeniedException>(async () => await sut!.CreateNewOrderAsync(userId, companyId, request, cancellationToken));
    }

    [Test]
    public void CreateNewOrderAsync_ThrowsWhenModulesCannotBePopulated()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = [new OrderDetailRequest { ModuleId = moduleId }] };

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(true);

        var modResponses = new List<ModuleResponse> { new() { Id = moduleId, Name = "X", Price = 10, Type = ModuleType.Ecommerce } };

        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(modResponses);
        moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken)).ReturnsAsync((ModuleResponse?)null);

        Assert.ThrowsAsync<ItemNotExistsException>(async () => await sut!.CreateNewOrderAsync(userId, companyId, request, cancellationToken));
    }

    [Test]
    public void CreateNewOrderAsync_ReturnsNullWhenNoModules()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = [new OrderDetailRequest { ModuleId = moduleId }] };

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(true);

        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync([]);
        moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken)).ReturnsAsync((ModuleResponse?)null);

        Assert.ThrowsAsync<ItemNotExistsException>(async () => await sut!.CreateNewOrderAsync(userId, companyId, request, cancellationToken));
    }

    [Test]
    public async Task CreateNewOrderAsync_SavesOrderAndRunsMigrations()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = [new OrderDetailRequest { ModuleId = moduleId }] };

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(true);

        var modResponses = new List<ModuleResponse> { new() { Id = moduleId, Name = "Basic", Price = 15, Type = ModuleType.Basic } };

        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(modResponses);

        OrderModel? savedOrder = null;
        orderRepositoryMock.Setup(x => x.Add(It.IsAny<OrderModel>())).Callback<OrderModel>(o => savedOrder = o);
        orderRepositoryMock.Setup(x => x.SaveChangesAsync(cancellationToken)).ReturnsAsync(1);

        subscriptionServiceMock.Setup(x => x.CreateCreditsForOrderAsync(It.IsAny<OrderModel>(), It.IsAny<List<OrderDetailModel>>(), companyId, cancellationToken))
            .ReturnsAsync(new SubscriptionResponse { Id = Guid.NewGuid(), Status = SubscriptionStatus.Active, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        migrationServiceMock.Setup(x => x.RunMigrationsAsync(companyId, It.IsAny<List<ModuleType>>(), cancellationToken)).Returns(Task.CompletedTask);

        var result = await sut!.CreateNewOrderAsync(userId, companyId, request, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(savedOrder, Is.Not.Null);
        }

        Assert.That(savedOrder!.TotalAmount, Is.EqualTo(15));

        subscriptionServiceMock.Verify(x => x.CreateCreditsForOrderAsync(It.IsAny<OrderModel>(), It.IsAny<List<OrderDetailModel>>(), companyId, cancellationToken), Times.Once);
        migrationServiceMock.Verify(x => x.RunMigrationsAsync(companyId, It.Is<List<ModuleType>>(l => l.Contains(ModuleType.Basic)), cancellationToken), Times.Once);
    }

    [Test]
    public async Task CreateNewOrderAsync_DoesNotCallGetModuleByType_WhenBasicAlreadyIncluded()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = [new OrderDetailRequest { ModuleId = moduleId }] };

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(true);

        var modResponses = new List<ModuleResponse> { new() { Id = moduleId, Name = "Basic", Price = 10, Type = ModuleType.Basic } };
        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(modResponses);

        OrderModel? savedOrder = null;
        orderRepositoryMock.Setup(x => x.Add(It.IsAny<OrderModel>())).Callback<OrderModel>(o => savedOrder = o);
        orderRepositoryMock.Setup(x => x.SaveChangesAsync(cancellationToken)).ReturnsAsync(1);
        subscriptionServiceMock.Setup(x => x.CreateCreditsForOrderAsync(It.IsAny<OrderModel>(), It.IsAny<List<OrderDetailModel>>(), companyId, cancellationToken)).ReturnsAsync(new SubscriptionResponse { Id = Guid.NewGuid(), Status = SubscriptionStatus.Active, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });

        var result = await sut!.CreateNewOrderAsync(userId, companyId, request, cancellationToken);

        Assert.That(result, Is.Not.Null);
        moduleServiceMock.Verify(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken), Times.Never);
    }

    [Test]
    public void CreateNewOrderAsync_WhenModuleServiceThrows_ReturnsItemNotExistsException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = [new OrderDetailRequest { ModuleId = moduleId }] };

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(true);
        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ThrowsAsync(new Exception("boom"));

        Assert.ThrowsAsync<ItemNotExistsException>(async () => await sut!.CreateNewOrderAsync(userId, companyId, request, cancellationToken));
    }

    [Test]
    public async Task CreateNewOrderAsync_AddsBasicWhenMissingAndProceeds()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = [new OrderDetailRequest { ModuleId = moduleId }] };

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(true);

        var modResponses = new List<ModuleResponse> { new() { Id = moduleId, Name = "Ecom", Price = 20, Type = ModuleType.Ecommerce } };
        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(modResponses);

        var basicModule = new ModuleResponse { Id = Guid.NewGuid(), Name = "Basic", Price = 5, Type = ModuleType.Basic };
        moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken)).ReturnsAsync(basicModule);

        OrderModel? savedOrder = null;
        orderRepositoryMock.Setup(x => x.Add(It.IsAny<OrderModel>())).Callback<OrderModel>(o => savedOrder = o);
        orderRepositoryMock.Setup(x => x.SaveChangesAsync(cancellationToken)).ReturnsAsync(1);

        subscriptionServiceMock.Setup(x => x.CreateCreditsForOrderAsync(It.IsAny<OrderModel>(), It.IsAny<List<OrderDetailModel>>(), companyId, cancellationToken)).ReturnsAsync(new SubscriptionResponse { Id = Guid.NewGuid(), Status = SubscriptionStatus.Active, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        migrationServiceMock.Setup(x => x.RunMigrationsAsync(companyId, It.IsAny<List<ModuleType>>(), cancellationToken)).Returns(Task.CompletedTask);

        var result = await sut!.CreateNewOrderAsync(userId, companyId, request, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(savedOrder, Is.Not.Null);
        }

        Assert.That(savedOrder!.TotalAmount, Is.EqualTo(25));

        moduleServiceMock.Verify(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken), Times.Once);
        migrationServiceMock.Verify(x => x.RunMigrationsAsync(companyId, It.Is<List<ModuleType>>(l => l.Contains(ModuleType.Ecommerce) && l.Contains(ModuleType.Basic)), cancellationToken), Times.Once);
    }

    [Test]
    public async Task PopulateModules_PrivateMethod_BehaviorViaReflection()
    {
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = [new OrderDetailRequest { ModuleId = moduleId }] };

        var basicResponse = new ModuleResponse { Id = Guid.NewGuid(), Name = "Basic", Price = 1, Type = ModuleType.Basic };
        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync([basicResponse]);

        var method = typeof(OrderService).GetMethod("PopulateModules", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null);

        var resultTask = (Task<List<ModuleModel>?>)method!.Invoke(sut!, [request, cancellationToken])!;
        var result = await resultTask;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Any(m => m.Type == ModuleType.Basic));

        moduleServiceMock.Reset();
        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync([]);
        moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken)).ReturnsAsync((ModuleResponse?)null);

        var resultTask2 = (Task<List<ModuleModel>?>)method.Invoke(sut!, [request, cancellationToken])!;
        var result2 = await resultTask2;

        Assert.That(result2, Is.Null);

        moduleServiceMock.Reset();
        var ecom = new ModuleResponse { Id = moduleId, Name = "Ecom", Price = 10, Type = ModuleType.Ecommerce };
        var basic = new ModuleResponse { Id = Guid.NewGuid(), Name = "Basic", Price = 2, Type = ModuleType.Basic };
        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync([ecom]);
        moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken)).ReturnsAsync(basic);

        var resultTask3 = (Task<List<ModuleModel>?>)method.Invoke(sut!, [request, cancellationToken])!;
        var result3 = await resultTask3;

        Assert.That(result3, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result3!, Has.Count.EqualTo(2));
            Assert.That(result3.Any(m => m.Type == ModuleType.Basic));
        }

        moduleServiceMock.Reset();
        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ThrowsAsync(new Exception("boom"));

        var resultTask4 = (Task<List<ModuleModel>?>)method.Invoke(sut!, [request, cancellationToken])!;
        var result4 = await resultTask4;

        Assert.That(result4, Is.Null);
    }
}
