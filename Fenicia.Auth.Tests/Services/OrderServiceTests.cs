using System.Reflection;

using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Migrations.Services;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class OrderServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Mock<IMigrationService> migrationServiceMock = null!;
    private Mock<IModuleService> moduleServiceMock = null!;
    private Mock<IOrderRepository> orderRepositoryMock = null!;
    private Mock<ISubscriptionService> subscriptionServiceMock = null!;

    private OrderService? sut;
    private Mock<IUserService> userServiceMock = null!;

    [SetUp]
    public void Setup()
    {
        this.orderRepositoryMock = new Mock<IOrderRepository>();
        this.moduleServiceMock = new Mock<IModuleService>();
        this.subscriptionServiceMock = new Mock<ISubscriptionService>();
        this.userServiceMock = new Mock<IUserService>();
        this.migrationServiceMock = new Mock<IMigrationService>();

        this.sut = new OrderService(this.orderRepositoryMock.Object, this.moduleServiceMock.Object,
            this.subscriptionServiceMock.Object, this.userServiceMock.Object, this.migrationServiceMock.Object);
    }

    [Test]
    public void CreateNewOrderAsync_ThrowsWhenUserNotInCompany()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var request = new OrderRequest { Details = [new OrderDetailRequest { ModuleId = Guid.NewGuid() }] };

        this.userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, this.cancellationToken))
            .ReturnsAsync(false);

        Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.sut!.CreateNewOrderAsync(userId, companyId, request, this.cancellationToken));
    }

    [Test]
    public void CreateNewOrderAsync_ThrowsWhenModulesCannotBePopulated()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = [new OrderDetailRequest { ModuleId = moduleId }] };

        this.userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, this.cancellationToken))
            .ReturnsAsync(true);

        var modResponses = new List<ModuleResponse>
            { new() { Id = moduleId, Name = "X", Price = 10, Type = ModuleType.Ecommerce } };

        this.moduleServiceMock
            .Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), this.cancellationToken))
            .ReturnsAsync(modResponses);
        this.moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, this.cancellationToken))
            .ReturnsAsync((ModuleResponse?)null);

        Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.sut!.CreateNewOrderAsync(userId, companyId, request, this.cancellationToken));
    }

    [Test]
    public void CreateNewOrderAsync_ReturnsNullWhenNoModules()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = [new OrderDetailRequest { ModuleId = moduleId }] };

        this.userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, this.cancellationToken))
            .ReturnsAsync(true);

        this.moduleServiceMock
            .Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), this.cancellationToken))
            .ReturnsAsync([]);
        this.moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, this.cancellationToken))
            .ReturnsAsync((ModuleResponse?)null);

        Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.sut!.CreateNewOrderAsync(userId, companyId, request, this.cancellationToken));
    }

    [Test]
    public async Task CreateNewOrderAsync_SavesOrderAndRunsMigrations()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = [new OrderDetailRequest { ModuleId = moduleId }] };

        this.userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, this.cancellationToken))
            .ReturnsAsync(true);

        var modResponses = new List<ModuleResponse>
            { new() { Id = moduleId, Name = "Basic", Price = 15, Type = ModuleType.Basic } };

        this.moduleServiceMock
            .Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), this.cancellationToken))
            .ReturnsAsync(modResponses);

        OrderModel? savedOrder = null;
        this.orderRepositoryMock.Setup(x => x.Add(It.IsAny<OrderModel>())).Callback<OrderModel>(o => savedOrder = o);
        this.orderRepositoryMock.Setup(x => x.SaveChangesAsync(this.cancellationToken)).ReturnsAsync(1);

        this.subscriptionServiceMock.Setup(x => x.CreateCreditsForOrderAsync(It.IsAny<OrderModel>(),
                It.IsAny<List<OrderDetailModel>>(), companyId, this.cancellationToken))
            .ReturnsAsync(new SubscriptionResponse
            {
                Id = Guid.NewGuid(), Status = SubscriptionStatus.Active, StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            });
        this.migrationServiceMock
            .Setup(x => x.RunMigrationsAsync(companyId, It.IsAny<List<ModuleType>>(), this.cancellationToken))
            .Returns(Task.CompletedTask);

        var result = await this.sut!.CreateNewOrderAsync(userId, companyId, request, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(savedOrder, Is.Not.Null);
        }

        Assert.That(savedOrder!.TotalAmount, Is.EqualTo(15));

        this.subscriptionServiceMock.Verify(
            x => x.CreateCreditsForOrderAsync(It.IsAny<OrderModel>(), It.IsAny<List<OrderDetailModel>>(), companyId,
                this.cancellationToken), Times.Once);
        this.migrationServiceMock.Verify(
            x => x.RunMigrationsAsync(companyId, It.Is<List<ModuleType>>(l => l.Contains(ModuleType.Basic)),
                this.cancellationToken), Times.Once);
    }

    [Test]
    public async Task CreateNewOrderAsync_DoesNotCallGetModuleByType_WhenBasicAlreadyIncluded()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = [new OrderDetailRequest { ModuleId = moduleId }] };

        this.userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, this.cancellationToken))
            .ReturnsAsync(true);

        var modResponses = new List<ModuleResponse>
            { new() { Id = moduleId, Name = "Basic", Price = 10, Type = ModuleType.Basic } };
        this.moduleServiceMock
            .Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), this.cancellationToken))
            .ReturnsAsync(modResponses);

        this.orderRepositoryMock.Setup(x => x.Add(It.IsAny<OrderModel>())).Callback<OrderModel>(o => _ = o);
        this.orderRepositoryMock.Setup(x => x.SaveChangesAsync(this.cancellationToken)).ReturnsAsync(1);
        this.subscriptionServiceMock
            .Setup(x => x.CreateCreditsForOrderAsync(It.IsAny<OrderModel>(), It.IsAny<List<OrderDetailModel>>(),
                companyId, this.cancellationToken)).ReturnsAsync(new SubscriptionResponse
            {
                Id = Guid.NewGuid(), Status = SubscriptionStatus.Active, StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            });

        var result = await this.sut!.CreateNewOrderAsync(userId, companyId, request, this.cancellationToken);

        Assert.That(result, Is.Not.Null);
        this.moduleServiceMock.Verify(x => x.GetModuleByTypeAsync(ModuleType.Basic, this.cancellationToken),
            Times.Never);
    }

    [Test]
    public void CreateNewOrderAsync_WhenModuleServiceThrows_ReturnsItemNotExistsException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = [new OrderDetailRequest { ModuleId = moduleId }] };

        this.userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, this.cancellationToken))
            .ReturnsAsync(true);
        this.moduleServiceMock
            .Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), this.cancellationToken))
            .ThrowsAsync(new Exception("boom"));

        Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.sut!.CreateNewOrderAsync(userId, companyId, request, this.cancellationToken));
    }

    [Test]
    public async Task CreateNewOrderAsync_AddsBasicWhenMissingAndProceeds()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = [new OrderDetailRequest { ModuleId = moduleId }] };

        this.userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, this.cancellationToken))
            .ReturnsAsync(true);

        var modResponses = new List<ModuleResponse>
            { new() { Id = moduleId, Name = "Ecom", Price = 20, Type = ModuleType.Ecommerce } };
        this.moduleServiceMock
            .Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), this.cancellationToken))
            .ReturnsAsync(modResponses);

        var basicModule = new ModuleResponse
            { Id = Guid.NewGuid(), Name = "Basic", Price = 5, Type = ModuleType.Basic };
        this.moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, this.cancellationToken))
            .ReturnsAsync(basicModule);

        OrderModel? savedOrder = null;
        this.orderRepositoryMock.Setup(x => x.Add(It.IsAny<OrderModel>())).Callback<OrderModel>(o => savedOrder = o);
        this.orderRepositoryMock.Setup(x => x.SaveChangesAsync(this.cancellationToken)).ReturnsAsync(1);

        this.subscriptionServiceMock
            .Setup(x => x.CreateCreditsForOrderAsync(It.IsAny<OrderModel>(), It.IsAny<List<OrderDetailModel>>(),
                companyId, this.cancellationToken)).ReturnsAsync(new SubscriptionResponse
            {
                Id = Guid.NewGuid(), Status = SubscriptionStatus.Active, StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            });
        this.migrationServiceMock
            .Setup(x => x.RunMigrationsAsync(companyId, It.IsAny<List<ModuleType>>(), this.cancellationToken))
            .Returns(Task.CompletedTask);

        var result = await this.sut!.CreateNewOrderAsync(userId, companyId, request, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(savedOrder, Is.Not.Null);
        }

        Assert.That(savedOrder!.TotalAmount, Is.EqualTo(25));

        this.moduleServiceMock.Verify(x => x.GetModuleByTypeAsync(ModuleType.Basic, this.cancellationToken),
            Times.Once);
        this.migrationServiceMock.Verify(
            x => x.RunMigrationsAsync(companyId,
                It.Is<List<ModuleType>>(l => l.Contains(ModuleType.Ecommerce) && l.Contains(ModuleType.Basic)),
                this.cancellationToken), Times.Once);
    }

    [Test]
    public async Task PopulateModules_PrivateMethod_BehaviorViaReflection()
    {
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = [new OrderDetailRequest { ModuleId = moduleId }] };

        var basicResponse = new ModuleResponse
            { Id = Guid.NewGuid(), Name = "Basic", Price = 1, Type = ModuleType.Basic };
        this.moduleServiceMock
            .Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), this.cancellationToken))
            .ReturnsAsync([basicResponse]);

        var method = typeof(OrderService).GetMethod("PopulateModules", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null);

        var resultTask = (Task<List<ModuleModel>?>)method!.Invoke(this.sut!, [request, this.cancellationToken])!;
        var result = await resultTask;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Any(m => m.Type == ModuleType.Basic));

        this.moduleServiceMock.Reset();
        this.moduleServiceMock
            .Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), this.cancellationToken))
            .ReturnsAsync([]);
        this.moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, this.cancellationToken))
            .ReturnsAsync((ModuleResponse?)null);

        var resultTask2 = (Task<List<ModuleModel>?>)method.Invoke(this.sut!, [request, this.cancellationToken])!;
        var result2 = await resultTask2;

        Assert.That(result2, Is.Null);

        this.moduleServiceMock.Reset();
        var ecom = new ModuleResponse { Id = moduleId, Name = "Ecom", Price = 10, Type = ModuleType.Ecommerce };
        var basic = new ModuleResponse { Id = Guid.NewGuid(), Name = "Basic", Price = 2, Type = ModuleType.Basic };
        this.moduleServiceMock
            .Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), this.cancellationToken))
            .ReturnsAsync([ecom]);
        this.moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, this.cancellationToken))
            .ReturnsAsync(basic);

        var resultTask3 = (Task<List<ModuleModel>?>)method.Invoke(this.sut!, [request, this.cancellationToken])!;
        var result3 = await resultTask3;

        Assert.That(result3, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result3!, Has.Count.EqualTo(2));
            Assert.That(result3.Any(m => m.Type == ModuleType.Basic));
        }

        this.moduleServiceMock.Reset();
        this.moduleServiceMock
            .Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), this.cancellationToken))
            .ThrowsAsync(new Exception("boom"));

        var resultTask4 = (Task<List<ModuleModel>?>)method.Invoke(this.sut!, [request, this.cancellationToken])!;
        var result4 = await resultTask4;

        Assert.That(result4, Is.Null);
    }
}