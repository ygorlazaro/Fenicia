using System.Net;

using Fenicia.Auth.Domains.Subscription;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Enums;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class SubscriptionServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Mock<ISubscriptionRepository> subscriptionRepositoryMock;
    private SubscriptionService sut;

    [SetUp]
    public void Setup()
    {
        subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        sut = new SubscriptionService(subscriptionRepositoryMock.Object);
    }

    [Test]
    public async Task CreateCreditsForOrderAsyncWithValidOrderReturnsSuccess()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var orderDetails = new List<OrderDetailModel>
                           {
                               new () { Id = Guid.NewGuid(), ModuleId = Guid.NewGuid() },
                               new () { Id = Guid.NewGuid(), ModuleId = Guid.NewGuid() }
                           };

        var order = new OrderModel { Id = Guid.NewGuid(), Details = orderDetails };

        // Act
        var result = await sut.CreateCreditsForOrderAsync(order, orderDetails, companyId, cancellationToken);

        // Assert: compare relevant fields only
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data.Status, Is.EqualTo(SubscriptionStatus.Active));
        Assert.That(result.Data.OrderId, Is.EqualTo(order.Id));
        Assert.That(result.Data.StartDate.Date, Is.EqualTo(DateTime.UtcNow.Date));
        Assert.That(result.Data.EndDate.Date, Is.EqualTo(DateTime.UtcNow.AddMonths(1).Date));

        subscriptionRepositoryMock.Verify(x => x.SaveSubscriptionAsync(It.Is<SubscriptionModel>(s => s.CompanyId == companyId && s.OrderId == order.Id && s.Status == SubscriptionStatus.Active && s.Credits.Count == orderDetails.Count), cancellationToken), Times.Once);
    }

    [Test]
    public async Task CreateCreditsForOrderAsyncWithEmptyDetailsReturnsBadRequest()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var order = new OrderModel { Id = Guid.NewGuid(), Details = [] };
        var emptyDetails = new List<OrderDetailModel>();

        // Act
        var result = await sut.CreateCreditsForOrderAsync(order, emptyDetails, companyId, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Data, Is.Null);
        }

        subscriptionRepositoryMock.Verify(x => x.SaveSubscriptionAsync(It.IsAny<SubscriptionModel>(), cancellationToken), Times.Never);
    }

    [Test]
    public async Task GetValidSubscriptionsAsyncReturnsValidSubscriptions()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var expectedSubscriptions = new List<Guid>
                                    {
                                        Guid.NewGuid(),
                                        Guid.NewGuid(),
                                        Guid.NewGuid()
                                    };

        subscriptionRepositoryMock.Setup(x => x.GetValidSubscriptionAsync(companyId, cancellationToken)).ReturnsAsync(expectedSubscriptions);

        // Act
        var result = await sut.GetValidSubscriptionsAsync(companyId, cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedSubscriptions));

        subscriptionRepositoryMock.Verify(x => x.GetValidSubscriptionAsync(companyId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetValidSubscriptionsAsyncWhenNoSubscriptionsReturnsEmptyList()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var emptyList = new List<Guid>();

        subscriptionRepositoryMock.Setup(x => x.GetValidSubscriptionAsync(companyId, cancellationToken)).ReturnsAsync(emptyList);

        // Act
        var result = await sut.GetValidSubscriptionsAsync(companyId, cancellationToken);

        // Assert
        Assert.That(result.Data, Is.Empty);

        subscriptionRepositoryMock.Verify(x => x.GetValidSubscriptionAsync(companyId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task CreateCreditsForOrderAsyncVerifyCreditsDates()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var orderDetails = new List<OrderDetailModel>
                           {
                               new () { Id = Guid.NewGuid(), ModuleId = Guid.NewGuid() }
                           };

        var order = new OrderModel { Id = Guid.NewGuid(), Details = orderDetails };

        SubscriptionModel capturedSubscription = null!;
        subscriptionRepositoryMock.Setup(x => x.SaveSubscriptionAsync(It.IsAny<SubscriptionModel>(), cancellationToken)).Callback<SubscriptionModel, CancellationToken>((s, _) => capturedSubscription = s);

        // Act
        await sut.CreateCreditsForOrderAsync(order, orderDetails, companyId, cancellationToken);

        // Assert
        Assert.That(capturedSubscription, Is.Not.Null);
        Assert.That(capturedSubscription.Credits, Has.Count.EqualTo(expected: 1));

        var credit = capturedSubscription.Credits.First();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(credit.StartDate.Date, Is.EqualTo(DateTime.UtcNow.Date));
            Assert.That(credit.EndDate.Date, Is.EqualTo(DateTime.UtcNow.AddMonths(months: 1).Date));
            Assert.That(credit.IsActive, Is.True);
            Assert.That(credit.OrderDetailId, Is.EqualTo(orderDetails[index: 0].Id));
            Assert.That(credit.ModuleId, Is.EqualTo(orderDetails[index: 0].ModuleId));
        }
    }
}
