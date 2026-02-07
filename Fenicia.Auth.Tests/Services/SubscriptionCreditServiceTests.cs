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
        this.subscriptionCreditRepositoryMock = new Mock<ISubscriptionCreditRepository>();
        this.subscriptionServiceMock = new Mock<ISubscriptionService>();

        this.sut = new SubscriptionCreditService(this.subscriptionCreditRepositoryMock.Object, this.subscriptionServiceMock.Object);
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

        this.subscriptionServiceMock.Setup(x => x.GetValidSubscriptionsAsync(companyId, this.cancellationToken)).ReturnsAsync(validSubscriptions);

        this.subscriptionCreditRepositoryMock.Setup(x => x.GetValidModulesTypesAsync(validSubscriptions, this.cancellationToken)).ReturnsAsync(expectedModuleTypes);

        var result = await this.sut.GetActiveModulesTypesAsync(companyId, this.cancellationToken);

        Assert.That(result, Is.EqualTo(expectedModuleTypes));

        this.subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, this.cancellationToken), Times.Once);
        this.subscriptionCreditRepositoryMock.Verify(x => x.GetValidModulesTypesAsync(validSubscriptions, this.cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetActiveModulesTypesAsyncWhenNoValidSubscriptionsReturnsFailureResponse()
    {
        var companyId = Guid.NewGuid();
        var errorResponse = new List<Guid>();

        this.subscriptionServiceMock.Setup(x => x.GetValidSubscriptionsAsync(companyId, this.cancellationToken)).ReturnsAsync(errorResponse);

        var result = await this.sut.GetActiveModulesTypesAsync(companyId, this.cancellationToken);

        Assert.That(result, Is.Empty);

        this.subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, this.cancellationToken), Times.Once);
        this.subscriptionCreditRepositoryMock.Verify(x => x.GetValidModulesTypesAsync(It.IsAny<List<Guid>>(), this.cancellationToken), Times.Never);
    }

    [Test]
    public async Task GetActiveModulesTypesAsyncWhenRepositoryReturnsEmptyListReturnsEmptyList()
    {
        var companyId = Guid.NewGuid();
        var validSubscriptions = new List<Guid> { Guid.NewGuid() };
        var emptyModuleTypes = new List<ModuleType>();

        this.subscriptionServiceMock.Setup(x => x.GetValidSubscriptionsAsync(companyId, this.cancellationToken)).ReturnsAsync(validSubscriptions);

        this.subscriptionCreditRepositoryMock.Setup(x => x.GetValidModulesTypesAsync(validSubscriptions, this.cancellationToken)).ReturnsAsync(emptyModuleTypes);

        var result = await this.sut.GetActiveModulesTypesAsync(companyId, this.cancellationToken);

        Assert.That(result, Is.Empty);

        this.subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, this.cancellationToken), Times.Once);
        this.subscriptionCreditRepositoryMock.Verify(x => x.GetValidModulesTypesAsync(validSubscriptions, this.cancellationToken), Times.Once);
    }
}