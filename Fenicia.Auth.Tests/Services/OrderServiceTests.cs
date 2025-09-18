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
using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.User;

public class OrderServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Faker faker;
    private Mock<ILogger<OrderService>> loggerMock;
    private Mock<IModuleService> moduleServiceMock;
    private Mock<IOrderRepository> orderRepositoryMock;
    private Mock<ISubscriptionService> subscriptionServiceMock;
    private OrderService sut;
    private Mock<IUserService> userServiceMock;

    [SetUp]
    public void Setup()
    {
        this.loggerMock = new Mock<ILogger<OrderService>>();
        this.orderRepositoryMock = new Mock<IOrderRepository>();
        this.moduleServiceMock = new Mock<IModuleService>();
        this.subscriptionServiceMock = new Mock<ISubscriptionService>();
        this.userServiceMock = new Mock<IUserService>();

        this.sut = new OrderService(this.loggerMock.Object, this.orderRepositoryMock.Object, this.moduleServiceMock.Object, this.subscriptionServiceMock.Object, this.userServiceMock.Object);

        this.faker = new Faker();
    }

    [Test]
    public async Task CreateNewOrderAsync_WhenUserNotInCompany_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var request = new OrderRequest { Details = [] };

        this.userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, this.cancellationToken)).ReturnsAsync(new ApiResponse<bool>(data: false));

        // Act
        var result = await this.sut.CreateNewOrderAsync(userId, companyId, request, this.cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.BadRequest));
        });
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

        this.userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, this.cancellationToken)).ReturnsAsync(new ApiResponse<bool>(data: true));

        this.moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), this.cancellationToken)).ReturnsAsync(new ApiResponse<List<ModuleResponse>>(emptyModulesList));

        this.moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, this.cancellationToken)).ReturnsAsync(new ApiResponse<ModuleResponse>(data: null));

        // Act
        var result = await this.sut.CreateNewOrderAsync(userId, companyId, request, this.cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.BadRequest));
        });
    }

    [Test]
    public async Task CreateNewOrderAsync_WhenSuccessful_CreatesOrderAndSubscriptions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var moduleAmount = this.faker.Random.Decimal(min: 10, max: 1000);

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
            Amount = this.faker.Random.Decimal(min: 10, max: 1000)
        };

        var moduleModels = new List<ModuleModel>
                           {
                               new ()
                               {
                                   Id = moduleId,
                                   Amount = moduleAmount,
                                   Type = ModuleType.Accounting
                               },
                               new ()
                               {
                                   Id = basicModuleResponse.Id,
                                   Amount = basicModuleResponse.Amount,
                                   Type = ModuleType.Basic
                               }
                           };

        var expectedOrder = new OrderModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Approved,
            TotalAmount = moduleAmount + basicModuleResponse.Amount
        };

        this.userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, this.cancellationToken)).ReturnsAsync(new ApiResponse<bool>(data: true));

        this.moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), this.cancellationToken)).ReturnsAsync(new ApiResponse<List<ModuleResponse>>(moduleResponses));

        this.moduleServiceMock.Setup(x => x.GetModuleByTypeAsync(ModuleType.Basic, this.cancellationToken)).ReturnsAsync(new ApiResponse<ModuleResponse>(basicModuleResponse));

        // Act
        var result = await this.sut.CreateNewOrderAsync(userId, companyId, request, this.cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Data, Is.Not.Null);
        });

        this.orderRepositoryMock.Verify(x => x.SaveOrderAsync(It.Is<OrderModel>(o => o.UserId == userId && o.Status == OrderStatus.Approved), this.cancellationToken), Times.Once);

        this.subscriptionServiceMock.Verify(x => x.CreateCreditsForOrderAsync(It.IsAny<OrderModel>(), It.IsAny<List<OrderDetailModel>>(), companyId, this.cancellationToken), Times.Once);
    }

    [Test]
    public async Task CreateNewOrderAsync_WhenOrderIncludesBasicModule_DoesNotAddExtraBasicModule()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var basicModuleId = Guid.NewGuid();
        var moduleAmount = this.faker.Random.Decimal(min: 10, max: 1000);

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

        var moduleModels = new List<ModuleModel>
                           {
                               new ()
                               {
                                   Id = basicModuleId,
                                   Amount = moduleAmount,
                                   Type = ModuleType.Basic
                               }
                           };

        this.userServiceMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, this.cancellationToken)).ReturnsAsync(new ApiResponse<bool>(data: true));

        this.moduleServiceMock.Setup(x => x.GetModulesToOrderAsync(It.IsAny<IEnumerable<Guid>>(), this.cancellationToken)).ReturnsAsync(new ApiResponse<List<ModuleResponse>>(moduleResponses));

        // Act
        var result = await this.sut.CreateNewOrderAsync(userId, companyId, request, this.cancellationToken);

        // Assert
        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        this.moduleServiceMock.Verify(x => x.GetModuleByTypeAsync(ModuleType.Basic, this.cancellationToken), Times.Never);
    }
}
