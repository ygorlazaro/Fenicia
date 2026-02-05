using Fenicia.Auth.Domains.Subscription;
using Fenicia.Common.Data.Models.Auth;
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
        var companyId = Guid.NewGuid();
        var orderDetails = new List<OrderDetailModel>
                           {
                               new () { Id = Guid.NewGuid(), ModuleId = Guid.NewGuid() },
                               new () { Id = Guid.NewGuid(), ModuleId = Guid.NewGuid() }
                           };

        var order = new OrderModel { Id = Guid.NewGuid(), Details = orderDetails };

        var result = await sut.CreateCreditsForOrderAsync(order, orderDetails, companyId, cancellationToken);

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Status, Is.EqualTo(SubscriptionStatus.Active));
            Assert.That(result.OrderId, Is.EqualTo(order.Id));
            Assert.That(result.StartDate.Date, Is.EqualTo(DateTime.UtcNow.Date));
            Assert.That(result.EndDate.Date, Is.EqualTo(DateTime.UtcNow.AddMonths(1).Date));
        }
    }

    [Test]
    public void CreateCreditsForOrderAsyncWithEmptyDetailsReturnsBadRequest()
    {
        var companyId = Guid.NewGuid();
        var emptyDetails = new List<OrderDetailModel>();
        var order = new OrderModel { Id = Guid.NewGuid(), Details = emptyDetails };

        Assert.ThrowsAsync<ArgumentException>(async () => await sut.CreateCreditsForOrderAsync(order, emptyDetails, companyId, cancellationToken));
    }

    [Test]
    public async Task GetValidSubscriptionsAsyncReturnsValidSubscriptions()
    {
        var companyId = Guid.NewGuid();
        var expectedSubscriptions = new List<Guid>
                                    {
                                        Guid.NewGuid(),
                                        Guid.NewGuid(),
                                        Guid.NewGuid()
                                    };

        subscriptionRepositoryMock.Setup(x => x.GetValidSubscriptionAsync(companyId, cancellationToken)).ReturnsAsync(expectedSubscriptions);

        var result = await sut.GetValidSubscriptionsAsync(companyId, cancellationToken);

        Assert.That(result, Is.EqualTo(expectedSubscriptions));

        subscriptionRepositoryMock.Verify(x => x.GetValidSubscriptionAsync(companyId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetValidSubscriptionsAsyncWhenNoSubscriptionsReturnsEmptyList()
    {
        var companyId = Guid.NewGuid();
        var emptyList = new List<Guid>();

        subscriptionRepositoryMock.Setup(x => x.GetValidSubscriptionAsync(companyId, cancellationToken)).ReturnsAsync(emptyList);

        var result = await sut.GetValidSubscriptionsAsync(companyId, cancellationToken);

        Assert.That(result, Is.Empty);

        subscriptionRepositoryMock.Verify(x => x.GetValidSubscriptionAsync(companyId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task CreateCreditsForOrderAsyncVerifyCreditsDates()
    {
        var companyId = Guid.NewGuid();
        var orderDetails = new List<OrderDetailModel>
                           {
                               new () { Id = Guid.NewGuid(), ModuleId = Guid.NewGuid() }
                           };

        var order = new OrderModel { Id = Guid.NewGuid(), Details = orderDetails };

        SubscriptionModel capturedSubscription = null!;
        subscriptionRepositoryMock.Setup(x => x.Add(It.IsAny<SubscriptionModel>())).Callback<SubscriptionModel>(s => capturedSubscription = s);
        subscriptionRepositoryMock.Setup(x => x.SaveChangesAsync(cancellationToken)).ReturnsAsync(1);

        await sut.CreateCreditsForOrderAsync(order, orderDetails, companyId, cancellationToken);

        Assert.That(capturedSubscription, Is.Not.Null);
        Assert.That(capturedSubscription.Credits, Has.Count.EqualTo(1));

        var credit = capturedSubscription.Credits.First();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(credit.StartDate.Date, Is.EqualTo(DateTime.UtcNow.Date));
            Assert.That(credit.EndDate.Date, Is.EqualTo(DateTime.UtcNow.AddMonths(1).Date));
            Assert.That(credit.IsActive, Is.True);
            Assert.That(credit.OrderDetailId, Is.EqualTo(orderDetails[0].Id));
            Assert.That(credit.ModuleId, Is.EqualTo(orderDetails[0].ModuleId));
        }
    }
}
