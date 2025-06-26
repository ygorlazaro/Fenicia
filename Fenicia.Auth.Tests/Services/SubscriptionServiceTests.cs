namespace Fenicia.Auth.Tests.Services;

using System.Net;

using AutoMapper;

using Domains.Order.Data;
using Domains.OrderDetail.Data;
using Domains.Subscription.Data;
using Domains.Subscription.Logic;

using Enums;

using Microsoft.Extensions.Logging;

using Moq;

public class SubscriptionServiceTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private Mock<ILogger<SubscriptionService>> _loggerMock;
    private Mock<IMapper> _mapperMock;
    private Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
    private SubscriptionService _sut;

    [SetUp]
    public void Setup()
    {
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<SubscriptionService>>();
        _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        _sut = new SubscriptionService(_mapperMock.Object, _loggerMock.Object, _subscriptionRepositoryMock.Object);
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

        _mapperMock.Setup(x => x.Map<SubscriptionResponse>(It.IsAny<SubscriptionModel>())).Returns(expectedResponse);

        // Act
        var result = await _sut.CreateCreditsForOrderAsync(order, orderDetails, companyId, _cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResponse));

        _subscriptionRepositoryMock.Verify(x => x.SaveSubscriptionAsync(It.Is<SubscriptionModel>(s => s.CompanyId == companyId && s.OrderId == order.Id && s.Status == SubscriptionStatus.Active && s.Credits.Count == orderDetails.Count), _cancellationToken), Times.Once);
    }

    [Test]
    public async Task CreateCreditsForOrderAsync_WithEmptyDetails_ReturnsBadRequest()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var order = new OrderModel { Id = Guid.NewGuid(), Details = [] };
        var emptyDetails = new List<OrderDetailModel>();

        // Act
        var result = await _sut.CreateCreditsForOrderAsync(order, emptyDetails, companyId, _cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Data, Is.Null);
        });

        _subscriptionRepositoryMock.Verify(x => x.SaveSubscriptionAsync(It.IsAny<SubscriptionModel>(), _cancellationToken), Times.Never);
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

        _subscriptionRepositoryMock.Setup(x => x.GetValidSubscriptionAsync(companyId, _cancellationToken)).ReturnsAsync(expectedSubscriptions);

        // Act
        var result = await _sut.GetValidSubscriptionsAsync(companyId, _cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedSubscriptions));

        _subscriptionRepositoryMock.Verify(x => x.GetValidSubscriptionAsync(companyId, _cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetValidSubscriptionsAsync_WhenNoSubscriptions_ReturnsEmptyList()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var emptyList = new List<Guid>();

        _subscriptionRepositoryMock.Setup(x => x.GetValidSubscriptionAsync(companyId, _cancellationToken)).ReturnsAsync(emptyList);

        // Act
        var result = await _sut.GetValidSubscriptionsAsync(companyId, _cancellationToken);

        // Assert
        Assert.That(result.Data, Is.Empty);

        _subscriptionRepositoryMock.Verify(x => x.GetValidSubscriptionAsync(companyId, _cancellationToken), Times.Once);
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
        _subscriptionRepositoryMock.Setup(x => x.SaveSubscriptionAsync(It.IsAny<SubscriptionModel>(), _cancellationToken)).Callback<SubscriptionModel, CancellationToken>((s, _) => capturedSubscription = s);

        // Act
        await _sut.CreateCreditsForOrderAsync(order, orderDetails, companyId, _cancellationToken);

        // Assert
        Assert.That(capturedSubscription, Is.Not.Null);
        Assert.That(capturedSubscription.Credits, Has.Count.EqualTo(expected: 1));

        var credit = capturedSubscription.Credits.First();
        Assert.Multiple(() =>
        {
            Assert.That(credit.StartDate.Date, Is.EqualTo(DateTime.UtcNow.Date));
            Assert.That(credit.EndDate.Date, Is.EqualTo(DateTime.UtcNow.AddMonths(months: 1).Date));
            Assert.That(credit.IsActive, Is.True);
            Assert.That(credit.OrderDetailId, Is.EqualTo(orderDetails[index: 0].Id));
            Assert.That(credit.ModuleId, Is.EqualTo(orderDetails[index: 0].ModuleId));
        });
    }
}
