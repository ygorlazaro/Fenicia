using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.SubscriptionCredit;
using Fenicia.Common.Enums.Auth;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class SubscriptionCreditServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Mock<ISubscriptionCreditRepository> subscriptionCreditRepositoryMock;
    private Mock<ISubscriptionService> subscriptionServiceMock;
    private SubscriptionCreditService sut;

    [SetUp]
    public void Setup()
    {
        subscriptionCreditRepositoryMock = new Mock<ISubscriptionCreditRepository>();
        subscriptionServiceMock = new Mock<ISubscriptionService>();

        sut = new SubscriptionCreditService(subscriptionCreditRepositoryMock.Object, subscriptionServiceMock.Object);
    }

    [Test]
    public async Task GetActiveModulesTypesAsyncWhenValidSubscriptionsExistReturnsModuleTypes()
    {
        var companyId = Guid.NewGuid();
        var validSubscriptions = new List<Guid> { Guid.NewGuid() };
        var expectedModuleTypes = new List<ModuleType>
                                  {
                                      ModuleType.Accounting,
                                      ModuleType.Contracts
                                  };

        subscriptionServiceMock.Setup(x => x.GetValidSubscriptionsAsync(companyId, cancellationToken)).ReturnsAsync(validSubscriptions);

        subscriptionCreditRepositoryMock.Setup(x => x.GetValidModulesTypesAsync(validSubscriptions, cancellationToken)).ReturnsAsync(expectedModuleTypes);

        var result = await sut.GetActiveModulesTypesAsync(companyId, cancellationToken);

        Assert.That(result, Is.EqualTo(expectedModuleTypes));

        subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, cancellationToken), Times.Once);
        subscriptionCreditRepositoryMock.Verify(x => x.GetValidModulesTypesAsync(validSubscriptions, cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetActiveModulesTypesAsyncWhenNoValidSubscriptionsReturnsFailureResponse()
    {
        var companyId = Guid.NewGuid();
        var errorResponse = new List<Guid>();

        subscriptionServiceMock.Setup(x => x.GetValidSubscriptionsAsync(companyId, cancellationToken)).ReturnsAsync(errorResponse);

        var result = await sut.GetActiveModulesTypesAsync(companyId, cancellationToken);

        Assert.That(result, Is.Empty);

        subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, cancellationToken), Times.Once);
        subscriptionCreditRepositoryMock.Verify(x => x.GetValidModulesTypesAsync(It.IsAny<List<Guid>>(), cancellationToken), Times.Never);
    }

    [Test]
    public async Task GetActiveModulesTypesAsyncWhenRepositoryReturnsEmptyListReturnsEmptyList()
    {
        var companyId = Guid.NewGuid();
        var validSubscriptions = new List<Guid> { Guid.NewGuid() };
        var emptyModuleTypes = new List<ModuleType>();

        subscriptionServiceMock.Setup(x => x.GetValidSubscriptionsAsync(companyId, cancellationToken)).ReturnsAsync(validSubscriptions);

        subscriptionCreditRepositoryMock.Setup(x => x.GetValidModulesTypesAsync(validSubscriptions, cancellationToken)).ReturnsAsync(emptyModuleTypes);

        var result = await sut.GetActiveModulesTypesAsync(companyId, cancellationToken);

        Assert.That(result, Is.Empty);

        subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, cancellationToken), Times.Once);
        subscriptionCreditRepositoryMock.Verify(x => x.GetValidModulesTypesAsync(validSubscriptions, cancellationToken), Times.Once);
    }
}
