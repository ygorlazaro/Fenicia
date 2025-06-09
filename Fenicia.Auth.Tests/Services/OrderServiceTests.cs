using System.Net;
using AutoMapper;
using Bogus;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Enums;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fenicia.Auth.Tests.Services;

public class OrderServiceTests
{
    private Mock<IMapper> _mapperMock;
    private Mock<ILogger<OrderService>> _loggerMock;
    private Mock<IOrderRepository> _orderRepositoryMock;
    private Mock<IModuleService> _moduleServiceMock;
    private Mock<ISubscriptionService> _subscriptionServiceMock;
    private Mock<IUserService> _userServiceMock;
    private OrderService _sut;
    private Faker _faker;

    [SetUp]
    public void Setup()
    {
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<OrderService>>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _moduleServiceMock = new Mock<IModuleService>();
        _subscriptionServiceMock = new Mock<ISubscriptionService>();
        _userServiceMock = new Mock<IUserService>();

        _sut = new OrderService(
            _mapperMock.Object,
            _loggerMock.Object,
            _orderRepositoryMock.Object,
            _moduleServiceMock.Object,
            _subscriptionServiceMock.Object,
            _userServiceMock.Object
        );

        _faker = new Faker();
    }

    [Test]
    public async Task CreateNewOrderAsync_WhenUserNotInCompany_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var request = new OrderRequest { Details = new List<OrderDetailRequest>() };

        _userServiceMock
            .Setup(x => x.ExistsInCompanyAsync(userId, companyId))
            .ReturnsAsync(new ServiceResponse<bool>(false));

        // Act
        var result = await _sut.CreateNewOrderAsync(userId, companyId, request);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(result.Message, Is.EqualTo(TextConstants.UserNotInCompany));
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
            Details = new List<OrderDetailRequest> { new() { ModuleId = moduleId } },
        };

        var emptyModulesList = new List<ModuleResponse>();

        _userServiceMock
            .Setup(x => x.ExistsInCompanyAsync(userId, companyId))
            .ReturnsAsync(new ServiceResponse<bool>(true));

        _moduleServiceMock
            .Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(new ServiceResponse<List<ModuleResponse>>(emptyModulesList));

        _moduleServiceMock
            .Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic))
            .ReturnsAsync(new ServiceResponse<ModuleResponse>(null));

        _mapperMock.Setup(x => x.Map<List<ModuleModel>>(emptyModulesList)).Returns([]);

        // Act
        var result = await _sut.CreateNewOrderAsync(userId, companyId, request);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(result.Message, Is.EqualTo(TextConstants.ThereWasAnErrorSearchingModules));
    }

    [Test]
    public async Task CreateNewOrderAsync_WhenSuccessful_CreatesOrderAndSubscriptions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var moduleAmount = _faker.Random.Decimal(10, 1000);

        var request = new OrderRequest
        {
            Details = new List<OrderDetailRequest> { new() { ModuleId = moduleId } },
        };

        var moduleResponses = new List<ModuleResponse>
        {
            new()
            {
                Id = moduleId,
                Amount = moduleAmount,
                Type = ModuleType.Accounting,
            },
        };

        var basicModuleResponse = new ModuleResponse
        {
            Id = Guid.NewGuid(),
            Type = ModuleType.Basic,
            Amount = _faker.Random.Decimal(10, 1000),
        };

        var moduleModels = new List<ModuleModel>
        {
            new()
            {
                Id = moduleId,
                Amount = moduleAmount,
                Type = ModuleType.Accounting,
            },
            new()
            {
                Id = basicModuleResponse.Id,
                Amount = basicModuleResponse.Amount,
                Type = ModuleType.Basic,
            },
        };

        var expectedOrder = new OrderModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Approved,
            TotalAmount = moduleAmount + basicModuleResponse.Amount,
        };

        _userServiceMock
            .Setup(x => x.ExistsInCompanyAsync(userId, companyId))
            .ReturnsAsync(new ServiceResponse<bool>(true));

        _moduleServiceMock
            .Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(new ServiceResponse<List<ModuleResponse>>(moduleResponses));

        _moduleServiceMock
            .Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic))
            .ReturnsAsync(new ServiceResponse<ModuleResponse>(basicModuleResponse));

        _mapperMock
            .Setup(x => x.Map<List<ModuleModel>>(moduleResponses))
            .Returns(moduleModels.Where(m => m.Type != ModuleType.Basic).ToList());

        _mapperMock
            .Setup(x => x.Map<List<ModuleModel>>(It.Is<List<ModuleResponse>>(l => l.Count == 2)))
            .Returns(moduleModels);

        _mapperMock
            .Setup(x => x.Map<OrderResponse>(It.IsAny<OrderModel>()))
            .Returns(new OrderResponse { Id = expectedOrder.Id });

        // Act
        var result = await _sut.CreateNewOrderAsync(userId, companyId, request);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result.Data, Is.Not.Null);

        _orderRepositoryMock.Verify(
            x =>
                x.SaveOrderAsync(
                    It.Is<OrderModel>(o => o.UserId == userId && o.Status == OrderStatus.Approved)
                ),
            Times.Once
        );

        _subscriptionServiceMock.Verify(
            x =>
                x.CreateCreditsForOrderAsync(
                    It.IsAny<OrderModel>(),
                    It.IsAny<List<OrderDetailModel>>(),
                    companyId
                ),
            Times.Once
        );
    }

    [Test]
    public async Task CreateNewOrderAsync_WhenOrderIncludesBasicModule_DoesNotAddExtraBasicModule()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var basicModuleId = Guid.NewGuid();
        var moduleAmount = _faker.Random.Decimal(10, 1000);

        var request = new OrderRequest
        {
            Details = new List<OrderDetailRequest> { new() { ModuleId = basicModuleId } },
        };

        var moduleResponses = new List<ModuleResponse>
        {
            new()
            {
                Id = basicModuleId,
                Amount = moduleAmount,
                Type = ModuleType.Basic,
            },
        };

        var moduleModels = new List<ModuleModel>
        {
            new()
            {
                Id = basicModuleId,
                Amount = moduleAmount,
                Type = ModuleType.Basic,
            },
        };

        _userServiceMock
            .Setup(x => x.ExistsInCompanyAsync(userId, companyId))
            .ReturnsAsync(new ServiceResponse<bool>(true));

        _moduleServiceMock
            .Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(new ServiceResponse<List<ModuleResponse>>(moduleResponses));

        _mapperMock.Setup(x => x.Map<List<ModuleModel>>(moduleResponses)).Returns(moduleModels);

        _mapperMock
            .Setup(x => x.Map<OrderResponse>(It.IsAny<OrderModel>()))
            .Returns(new OrderResponse { Id = Guid.NewGuid() });

        // Act
        var result = await _sut.CreateNewOrderAsync(userId, companyId, request);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        _moduleServiceMock.Verify(x => x.GetModuleByTypeAsync(ModuleType.Basic), Times.Never);
    }
}
