using System.Net;

using Bogus;

using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;
using Fenicia.Common.Enums;

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

    [SetUp]
    public void Setup()
    {
        orderRepositoryMock = new Mock<IOrderRepository>();
        moduleServiceMock = new Mock<IModuleService>();
        subscriptionServiceMock = new Mock<ISubscriptionService>();
        userServiceMock = new Mock<IUserService>();

        sut = new OrderService(orderRepositoryMock.Object, moduleServiceMock.Object, subscriptionServiceMock.Object, userServiceMock.Object);

        faker = new Faker();
    }

    [Test]
    public async Task CreateNewOrderAsyncWhenUserNotInCompanyReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var request = new OrderRequest { Details = [] };

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(new ApiResponse<bool>(data: false));

        // Act
        var result = await sut.CreateNewOrderAsync(userId, companyId, request, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }

    [Test]
    public async Task CreateNewOrderAsyncWhenNoModulesFoundReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var request = new OrderRequest
        {
            Details = [new OrderDetailRequest { ModuleId = moduleId }]
        };

        var emptyModulesList = new List<ModuleResponse>();

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(new ApiResponse<bool>(data: true));

        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(new ApiResponse<List<ModuleResponse>>(emptyModulesList));

        moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken)).ReturnsAsync(new ApiResponse<ModuleResponse>(data: null));

        // Act
        var result = await sut.CreateNewOrderAsync(userId, companyId, request, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.BadRequest));
        }
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

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(new ApiResponse<bool>(data: true));

        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(new ApiResponse<List<ModuleResponse>>(moduleResponses));

        moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken)).ReturnsAsync(new ApiResponse<ModuleResponse>(basicModuleResponse));

        // Act
        var result = await sut.CreateNewOrderAsync(userId, companyId, request, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Data, Is.Not.Null);
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

        userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(new ApiResponse<bool>(data: true));

        moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), cancellationToken)).ReturnsAsync(new ApiResponse<List<ModuleResponse>>(moduleResponses));

        // Act
        var result = await sut.CreateNewOrderAsync(userId, companyId, request, cancellationToken);

        // Assert
        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        moduleServiceMock.Verify(x => x.GetModuleByTypeAsync(ModuleType.Basic, cancellationToken), Times.Never);
    }
}
