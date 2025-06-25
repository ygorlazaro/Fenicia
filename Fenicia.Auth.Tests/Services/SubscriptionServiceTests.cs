using System.Net;

using AutoMapper;

using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.OrderDetail;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Enums;

using Microsoft.Extensions.Logging;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class SubscriptionServiceTests
{
    private Mock<IMapper> _mapperMock;
    private Mock<ILogger<SubscriptionService>> _loggerMock;
    private Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
    private SubscriptionService _sut;

    [SetUp]
    public void Setup()
    {
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<SubscriptionService>>();
        _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        _sut = new SubscriptionService(
            _mapperMock.Object,
            _loggerMock.Object,
            _subscriptionRepositoryMock.Object
        );
    }

    [Test]
    public async Task CreateCreditsForOrderAsync_WithValidOrder_ReturnsSuccess()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var orderDetails = new List<OrderDetailModel>
        {
            new() { Id = Guid.NewGuid(), ModuleId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), ModuleId = Guid.NewGuid() }
        };

        var order = new OrderModel { Id = Guid.NewGuid(), Details = orderDetails };

        var expectedResponse = new SubscriptionResponse();

        _mapperMock
            .Setup(x => x.Map<SubscriptionResponse>(It.IsAny<SubscriptionModel>()))
            .Returns(expectedResponse);

        // Act
        var result = await _sut.CreateCreditsForOrderAsync(order, orderDetails, companyId);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResponse));

        _subscriptionRepositoryMock.Verify(
            x =>
                x.SaveSubscription(
                    It.Is<SubscriptionModel>(s =>
                        s.CompanyId == companyId
                        && s.OrderId == order.Id
                        && s.Status == SubscriptionStatus.Active
                        && s.Credits.Count == orderDetails.Count
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task CreateCreditsForOrderAsync_WithEmptyDetails_ReturnsBadRequest()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var order = new OrderModel { Id = Guid.NewGuid(), Details = [] };
        var emptyDetails = new List<OrderDetailModel>();

        // Act
        var result = await _sut.CreateCreditsForOrderAsync(order, emptyDetails, companyId);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Data, Is.Null);
        });

        _subscriptionRepositoryMock.Verify(
            x => x.SaveSubscription(It.IsAny<SubscriptionModel>()),
            Times.Never
        );
    }

    [Test]
    public async Task GetValidSubscriptionsAsync_ReturnsValidSubscriptions()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var expectedSubscriptions = new List<Guid>
        {
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid()
        };

        _subscriptionRepositoryMock
            .Setup(x => x.GetValidSubscriptionAsync(companyId))
            .ReturnsAsync(expectedSubscriptions);

        // Act
        var result = await _sut.GetValidSubscriptionsAsync(companyId);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedSubscriptions));

        _subscriptionRepositoryMock.Verify(x => x.GetValidSubscriptionAsync(companyId), Times.Once);
    }

    [Test]
    public async Task GetValidSubscriptionsAsync_WhenNoSubscriptions_ReturnsEmptyList()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var emptyList = new List<Guid>();

        _subscriptionRepositoryMock
            .Setup(x => x.GetValidSubscriptionAsync(companyId))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _sut.GetValidSubscriptionsAsync(companyId);

        // Assert
        Assert.That(result.Data, Is.Empty);

        _subscriptionRepositoryMock.Verify(x => x.GetValidSubscriptionAsync(companyId), Times.Once);
    }

    [Test]
    public async Task CreateCreditsForOrderAsync_VerifyCreditsDates()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var orderDetails = new List<OrderDetailModel>
        {
            new() { Id = Guid.NewGuid(), ModuleId = Guid.NewGuid() }
        };

        var order = new OrderModel { Id = Guid.NewGuid(), Details = orderDetails };

        SubscriptionModel capturedSubscription = null!;
        _subscriptionRepositoryMock
            .Setup(x => x.SaveSubscription(It.IsAny<SubscriptionModel>()))
            .Callback<SubscriptionModel>(s => capturedSubscription = s);

        // Act
        await _sut.CreateCreditsForOrderAsync(order, orderDetails, companyId);

        // Assert
        Assert.That(capturedSubscription, Is.Not.Null);
        Assert.That(capturedSubscription.Credits, Has.Count.EqualTo(1));

        var credit = capturedSubscription.Credits.First();
        Assert.Multiple(() =>
        {
            Assert.That(credit.StartDate.Date, Is.EqualTo(DateTime.Now.Date));
            Assert.That(credit.EndDate.Date, Is.EqualTo(DateTime.Now.AddMonths(1).Date));
            Assert.That(credit.IsActive, Is.True);
            Assert.That(credit.OrderDetailId, Is.EqualTo(orderDetails[0].Id));
            Assert.That(credit.ModuleId, Is.EqualTo(orderDetails[0].ModuleId));
        });
    }
}
