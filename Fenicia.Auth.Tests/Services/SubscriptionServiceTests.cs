namespace Fenicia.Auth.Tests.Services;

using System.Net;

using Common.Database.Responses;

using Fenicia.Common.Database.Models.Auth;
using Common.Enums;

using Microsoft.Extensions.Logging;

using Moq;
using Fenicia.Auth.Domains.Subscription;

public class SubscriptionServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Mock<ILogger<SubscriptionService>> loggerMock;
    private Mock<ISubscriptionRepository> subscriptionRepositoryMock;
    private SubscriptionService sut;

    [SetUp]
    public void Setup()
    {
        this.loggerMock = new Mock<ILogger<SubscriptionService>>();
        this.subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        this.sut = new SubscriptionService(this.loggerMock.Object, this.subscriptionRepositoryMock.Object);
    }

    [Test]
    public async Task CreateCreditsForOrderAsync_WithValidOrder_ReturnsSuccess()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var orderDetails = new List<OrderDetailModel>
                           {
                               new () { Id = Guid.NewGuid(), ModuleId = Guid.NewGuid() },
                               new () { Id = Guid.NewGuid(), ModuleId = Guid.NewGuid() }
                           };

        var order = new OrderModel { Id = Guid.NewGuid(), Details = orderDetails };

        var expectedResponse = new SubscriptionResponse();

        // Act
        var result = await this.sut.CreateCreditsForOrderAsync(order, orderDetails, companyId, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResponse));

        this.subscriptionRepositoryMock.Verify(x => x.SaveSubscriptionAsync(It.Is<SubscriptionModel>(s => s.CompanyId == companyId && s.OrderId == order.Id && s.Status == SubscriptionStatus.Active && s.Credits.Count == orderDetails.Count), this.cancellationToken), Times.Once);
    }

    [Test]
    public async Task CreateCreditsForOrderAsync_WithEmptyDetails_ReturnsBadRequest()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var order = new OrderModel { Id = Guid.NewGuid(), Details = [] };
        var emptyDetails = new List<OrderDetailModel>();

        // Act
        var result = await this.sut.CreateCreditsForOrderAsync(order, emptyDetails, companyId, this.cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Data, Is.Null);
        });

        this.subscriptionRepositoryMock.Verify(x => x.SaveSubscriptionAsync(It.IsAny<SubscriptionModel>(), this.cancellationToken), Times.Never);
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

        this.subscriptionRepositoryMock.Setup(x => x.GetValidSubscriptionAsync(companyId, this.cancellationToken)).ReturnsAsync(expectedSubscriptions);

        // Act
        var result = await this.sut.GetValidSubscriptionsAsync(companyId, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedSubscriptions));

        this.subscriptionRepositoryMock.Verify(x => x.GetValidSubscriptionAsync(companyId, this.cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetValidSubscriptionsAsync_WhenNoSubscriptions_ReturnsEmptyList()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var emptyList = new List<Guid>();

        this.subscriptionRepositoryMock.Setup(x => x.GetValidSubscriptionAsync(companyId, this.cancellationToken)).ReturnsAsync(emptyList);

        // Act
        var result = await this.sut.GetValidSubscriptionsAsync(companyId, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.Empty);

        this.subscriptionRepositoryMock.Verify(x => x.GetValidSubscriptionAsync(companyId, this.cancellationToken), Times.Once);
    }

    [Test]
    public async Task CreateCreditsForOrderAsync_VerifyCreditsDates()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var orderDetails = new List<OrderDetailModel>
                           {
                               new () { Id = Guid.NewGuid(), ModuleId = Guid.NewGuid() }
                           };

        var order = new OrderModel { Id = Guid.NewGuid(), Details = orderDetails };

        SubscriptionModel capturedSubscription = null!;
        this.subscriptionRepositoryMock.Setup(x => x.SaveSubscriptionAsync(It.IsAny<SubscriptionModel>(), this.cancellationToken)).Callback<SubscriptionModel, CancellationToken>((s, _) => capturedSubscription = s);

        // Act
        await this.sut.CreateCreditsForOrderAsync(order, orderDetails, companyId, this.cancellationToken);

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
