namespace Fenicia.Auth.Tests.Services;

using System.Net;

using Bogus;

using Common;
using Common.Enums;

using Fenicia.Common.Database.Models.Auth;
using Common.Database.Requests;
using Common.Database.Responses;

using Microsoft.Extensions.Logging;

using Moq;
using Fenicia.Auth.Domains.Module;
using Domains.Order;
using Domains.Subscription;
using Domains.User;

public class OrderServiceTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private Faker _faker;
    private Mock<ILogger<OrderService>> _loggerMock;
    private Mock<IModuleService> _moduleServiceMock;
    private Mock<IOrderRepository> _orderRepositoryMock;
    private Mock<ISubscriptionService> _subscriptionServiceMock;
    private OrderService _sut;
    private Mock<IUserService> _userServiceMock;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<OrderService>>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _moduleServiceMock = new Mock<IModuleService>();
        _subscriptionServiceMock = new Mock<ISubscriptionService>();
        _userServiceMock = new Mock<IUserService>();

        _sut = new OrderService(_loggerMock.Object, _orderRepositoryMock.Object, _moduleServiceMock.Object, _subscriptionServiceMock.Object, _userServiceMock.Object);

        _faker = new Faker();
    }

    [Test]
    public async Task CreateNewOrderAsync_WhenUserNotInCompany_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var request = new OrderRequest { Details = [] };

        _userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, _cancellationToken)).ReturnsAsync(new ApiResponse<bool>(data: false));

        // Act
        var result = await _sut.CreateNewOrderAsync(userId, companyId, request, _cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }

    [Test]
    public async Task CreateNewOrderAsync_WhenNoModulesFound_ReturnsBadRequest()
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

        _userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, _cancellationToken)).ReturnsAsync(new ApiResponse<bool>(data: true));

        _moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), _cancellationToken)).ReturnsAsync(new ApiResponse<List<ModuleResponse>>(emptyModulesList));

        _moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, _cancellationToken)).ReturnsAsync(new ApiResponse<ModuleResponse>(data: null));

        // Act
        var result = await _sut.CreateNewOrderAsync(userId, companyId, request, _cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }

    [Test]
    public async Task CreateNewOrderAsync_WhenSuccessful_CreatesOrderAndSubscriptions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var moduleAmount = _faker.Random.Decimal(min: 10, max: 1000);

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
            Amount = _faker.Random.Decimal(min: 10, max: 1000)
        };

        _userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, _cancellationToken)).ReturnsAsync(new ApiResponse<bool>(data: true));

        _moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), _cancellationToken)).ReturnsAsync(new ApiResponse<List<ModuleResponse>>(moduleResponses));

        _moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, _cancellationToken)).ReturnsAsync(new ApiResponse<ModuleResponse>(basicModuleResponse));

        // Act
        var result = await _sut.CreateNewOrderAsync(userId, companyId, request, _cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Data, Is.Not.Null);
        }

        _orderRepositoryMock.Verify(x => x.SaveOrderAsync(It.Is<OrderModel>(o => o.UserId == userId && o.Status == OrderStatus.Approved), _cancellationToken), Times.Once);

        _subscriptionServiceMock.Verify(x => x.CreateCreditsForOrderAsync(It.IsAny<OrderModel>(), It.IsAny<List<OrderDetailModel>>(), companyId, _cancellationToken), Times.Once);
    }

    [Test]
    public async Task CreateNewOrderAsync_WhenOrderIncludesBasicModule_DoesNotAddExtraBasicModule()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var basicModuleId = Guid.NewGuid();
        var moduleAmount = _faker.Random.Decimal(min: 10, max: 1000);

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

        _userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, _cancellationToken)).ReturnsAsync(new ApiResponse<bool>(data: true));

        _moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), _cancellationToken)).ReturnsAsync(new ApiResponse<List<ModuleResponse>>(moduleResponses));

        // Act
        var result = await _sut.CreateNewOrderAsync(userId, companyId, request, _cancellationToken);

        // Assert
        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        _moduleServiceMock.Verify(x => x.GetModuleByTypeAsync(ModuleType.Basic, _cancellationToken), Times.Never);
    }
}
