using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;
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
    public async Task CreateNewOrderAsync_ThrowsWhenUserNotInCompany()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var request = new OrderRequest { Details = new[] { new OrderDetailRequest { ModuleId = Guid.NewGuid() } } };

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(false);

        Assert.ThrowsAsync<PermissionDeniedException>(async () => await sut!.CreateNewOrderAsync(userId, companyId, request, cancellationToken));
    }

    [Test]
    public async Task CreateNewOrderAsync_ThrowsWhenModulesCannotBePopulated()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = new[] { new OrderDetailRequest { ModuleId = moduleId } } };

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(true);

        var modResponses = new List<ModuleResponse> { new ModuleResponse { Id = moduleId, Name = "X", Amount = 10, Type = ModuleType.Ecommerce } };

        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(modResponses);
        moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken)).ReturnsAsync((ModuleResponse?)null);

        Assert.ThrowsAsync<ItemNotExistsException>(async () => await sut!.CreateNewOrderAsync(userId, companyId, request, cancellationToken));
    }

    [Test]
    public async Task CreateNewOrderAsync_ReturnsNullWhenNoModules()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = new[] { new OrderDetailRequest { ModuleId = moduleId } } };

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(true);

        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(new List<ModuleResponse>());
        moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken)).ReturnsAsync((ModuleResponse?)null);

        // When modules cannot be populated the service throws ItemNotExistsException (as implemented)
        Assert.ThrowsAsync<ItemNotExistsException>(async () => await sut!.CreateNewOrderAsync(userId, companyId, request, cancellationToken));
    }

    [Test]
    public async Task CreateNewOrderAsync_SavesOrderAndRunsMigrations()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = new[] { new OrderDetailRequest { ModuleId = moduleId } } };

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(true);

        var modResponses = new List<ModuleResponse> { new ModuleResponse { Id = moduleId, Name = "Basic", Amount = 15, Type = ModuleType.Basic } };

        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(modResponses);

        OrderModel? savedOrder = null;
        orderRepositoryMock.Setup(x => x.SaveOrderAsync(It.IsAny<OrderModel>(), cancellationToken))
                           .Returns((OrderModel o, CancellationToken _) =>
                           {
                               savedOrder = o;
                               return Task.FromResult(o);
                           });

        subscriptionServiceMock.Setup(x => x.CreateCreditsForOrderAsync(It.IsAny<OrderModel>(), It.IsAny<List<OrderDetailModel>>(), companyId, cancellationToken))
                               .ReturnsAsync(new SubscriptionResponse { Id = Guid.NewGuid(), Status = SubscriptionStatus.Active, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        migrationServiceMock.Setup(x => x.RunMigrationsAsync(companyId, It.IsAny<List<ModuleType>>(), cancellationToken)).Returns(Task.CompletedTask);

        var result = await sut!.CreateNewOrderAsync(userId, companyId, request, cancellationToken);

        Assert.That(result, Is.Not.Null);
        Assert.That(savedOrder, Is.Not.Null);
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
        var request = new OrderRequest { Details = new[] { new OrderDetailRequest { ModuleId = moduleId } } };

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(true);

        var modResponses = new List<ModuleResponse> { new ModuleResponse { Id = moduleId, Name = "Basic", Amount = 10, Type = ModuleType.Basic } };
        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(modResponses);

        OrderModel? savedOrder = null;
        orderRepositoryMock.Setup(x => x.SaveOrderAsync(It.IsAny<OrderModel>(), cancellationToken)).Returns((OrderModel o, CancellationToken _) => { savedOrder = o; return Task.FromResult(o); });
        subscriptionServiceMock.Setup(x => x.CreateCreditsForOrderAsync(It.IsAny<OrderModel>(), It.IsAny<List<OrderDetailModel>>(), companyId, cancellationToken)).ReturnsAsync(new SubscriptionResponse { Id = Guid.NewGuid(), Status = SubscriptionStatus.Active, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });

        var result = await sut!.CreateNewOrderAsync(userId, companyId, request, cancellationToken);

        Assert.That(result, Is.Not.Null);
        moduleServiceMock.Verify(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken), Times.Never);
    }

    [Test]
    public async Task CreateNewOrderAsync_WhenModuleServiceThrows_ReturnsItemNotExistsException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = new[] { new OrderDetailRequest { ModuleId = moduleId } } };

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
        var request = new OrderRequest { Details = new[] { new OrderDetailRequest { ModuleId = moduleId } } };

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(true);

        // Initial modules do NOT include Basic
        var modResponses = new List<ModuleResponse> { new ModuleResponse { Id = moduleId, Name = "Ecom", Amount = 20, Type = ModuleType.Ecommerce } };
        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(modResponses);

        // Basic module is returned by GetModuleByTypeAsync and should be added
        var basicModule = new ModuleResponse { Id = Guid.NewGuid(), Name = "Basic", Amount = 5, Type = ModuleType.Basic };
        moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken)).ReturnsAsync(basicModule);

        OrderModel? savedOrder = null;
        orderRepositoryMock.Setup(x => x.SaveOrderAsync(It.IsAny<OrderModel>(), cancellationToken)).Returns((OrderModel o, CancellationToken _) => { savedOrder = o; return Task.FromResult(o); });

        subscriptionServiceMock.Setup(x => x.CreateCreditsForOrderAsync(It.IsAny<OrderModel>(), It.IsAny<List<OrderDetailModel>>(), companyId, cancellationToken)).ReturnsAsync(new SubscriptionResponse { Id = Guid.NewGuid(), Status = SubscriptionStatus.Active, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        migrationServiceMock.Setup(x => x.RunMigrationsAsync(companyId, It.IsAny<List<ModuleType>>(), cancellationToken)).Returns(Task.CompletedTask);

        var result = await sut!.CreateNewOrderAsync(userId, companyId, request, cancellationToken);

        Assert.That(result, Is.Not.Null);
        Assert.That(savedOrder, Is.Not.Null);
        // total should be ecom + basic
        Assert.That(savedOrder!.TotalAmount, Is.EqualTo(25));

        // Verify that basic module was requested
        moduleServiceMock.Verify(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken), Times.Once);
        // Verify migrations include both types
        migrationServiceMock.Verify(x => x.RunMigrationsAsync(companyId, It.Is<List<ModuleType>>(l => l.Contains(ModuleType.Ecommerce) && l.Contains(ModuleType.Basic)), cancellationToken), Times.Once);
    }

    [Test]
    public async Task PopulateModules_PrivateMethod_BehaviorViaReflection()
    {
        // Arrange
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest { Details = new[] { new OrderDetailRequest { ModuleId = moduleId } } };

        // Case A: modules already include Basic
        var basicResponse = new ModuleResponse { Id = Guid.NewGuid(), Name = "Basic", Amount = 1, Type = ModuleType.Basic };
        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(new List<ModuleResponse> { basicResponse });

        var method = typeof(OrderService).GetMethod("PopulateModules", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null);

        var resultTask = (Task<List<ModuleModel>?>)method!.Invoke(sut!, new object[] { request, cancellationToken })!;
        var result = await resultTask;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Any(m => m.Type == ModuleType.Basic));

        // Case B: modules don't include Basic and GetModuleByTypeAsync returns null -> method returns null
        moduleServiceMock.Reset();
        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(new List<ModuleResponse>());
        moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken)).ReturnsAsync((ModuleResponse?)null);

        var resultTask2 = (Task<List<ModuleModel>?>)method.Invoke(sut!, new object[] { request, cancellationToken })!;
        var result2 = await resultTask2;

        Assert.That(result2, Is.Null);

        // Case C: modules don't include Basic and GetModuleByTypeAsync returns a basic module -> basic is added
        moduleServiceMock.Reset();
        var ecom = new ModuleResponse { Id = moduleId, Name = "Ecom", Amount = 10, Type = ModuleType.Ecommerce };
        var basic = new ModuleResponse { Id = Guid.NewGuid(), Name = "Basic", Amount = 2, Type = ModuleType.Basic };
        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(new List<ModuleResponse> { ecom });
        moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken)).ReturnsAsync(basic);

        var resultTask3 = (Task<List<ModuleModel>?>)method.Invoke(sut!, new object[] { request, cancellationToken })!;
        var result3 = await resultTask3;

        Assert.That(result3, Is.Not.Null);
        Assert.That(result3!.Count, Is.EqualTo(2));
        Assert.That(result3.Any(m => m.Type == ModuleType.Basic));

        // Case D: GetModulesToOrderAsync throws -> catch and return null
        moduleServiceMock.Reset();
        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ThrowsAsync(new Exception("boom"));

        var resultTask4 = (Task<List<ModuleModel>?>)method.Invoke(sut!, new object[] { request, cancellationToken })!;
        var result4 = await resultTask4;

        Assert.That(result4, Is.Null);
    }
}
