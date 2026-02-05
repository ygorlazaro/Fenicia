using Bogus;

using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;
using Fenicia.Common.Enums;
using Fenicia.Common.Migrations.Services;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class OrderServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Faker faker;
    private Mock<IModuleService> moduleServiceMock;
    private Mock<IOrderRepository> orderRepositoryMock;
    private Mock<ISubscriptionService> subscriptionServiceMock;
    private OrderService sut;
    private Mock<IUserService> userServiceMock;
    private Mock<MigrationService> migrationServiceMock;

    [SetUp]
    public void Setup()
    {
        orderRepositoryMock = new Mock<IOrderRepository>();
        moduleServiceMock = new Mock<IModuleService>();
        subscriptionServiceMock = new Mock<ISubscriptionService>();
        userServiceMock = new Mock<IUserService>();
        migrationServiceMock = new Mock<MigrationService>();

        sut = new OrderService(orderRepositoryMock.Object, moduleServiceMock.Object, subscriptionServiceMock.Object, userServiceMock.Object, migrationServiceMock.Object);

        faker = new Faker();
    }

    [Test]
    public async Task CreateNewOrderAsyncWhenSuccessfulCreatesOrderAndSubscriptions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var moduleAmount = faker.Random.Decimal(min: 10, max: 1000);

        var request = new OrderRequest
        {
            Details = [new OrderDetailRequest { ModuleId = moduleId }]
        };

        var moduleResponses = new List<ModuleResponse>
                              {
                                  new ()
                                  {
                                      Id = moduleId,
                                      Amount = moduleAmount,
                                      Type = ModuleType.Accounting
                                  }
                              };

        var basicModuleResponse = new ModuleResponse
        {
            Id = Guid.NewGuid(),
            Type = ModuleType.Basic,
            Amount = faker.Random.Decimal(min: 10, max: 1000)
        };

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(true);

        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(moduleResponses);

        moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken)).ReturnsAsync(basicModuleResponse);

        // Act
        var result = await sut.CreateNewOrderAsync(userId, companyId, request, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        orderRepositoryMock.Verify(x => x.SaveOrderAsync(It.Is<OrderModel>(o => o.UserId == userId && o.Status == OrderStatus.Approved), cancellationToken), Times.Once);

        subscriptionServiceMock.Verify(x => x.CreateCreditsForOrderAsync(It.IsAny<OrderModel>(), It.IsAny<List<OrderDetailModel>>(), companyId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task CreateNewOrderAsyncWhenOrderIncludesBasicModuleDoesNotAddExtraBasicModule()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var basicModuleId = Guid.NewGuid();
        var moduleAmount = faker.Random.Decimal(min: 10, max: 1000);

        var request = new OrderRequest
        {
            Details = [new OrderDetailRequest { ModuleId = basicModuleId }]
        };

        var moduleResponses = new List<ModuleResponse>
                              {
                                  new ()
                                  {
                                      Id = basicModuleId,
                                      Amount = moduleAmount,
                                      Type = ModuleType.Basic
                                  }
                              };

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(true);

        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(moduleResponses);

        // Act
        var result = await sut.CreateNewOrderAsync(userId, companyId, request, cancellationToken);

        // Assert
        moduleServiceMock.Verify(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken), Times.Never);
    }
}
