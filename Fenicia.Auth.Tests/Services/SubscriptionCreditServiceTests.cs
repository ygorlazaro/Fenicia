using System.Net;

using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.SubscriptionCredit;
using Fenicia.Common;
using Fenicia.Common.Enums;

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
        // Arrange
        var companyId = Guid.NewGuid();
        var validSubscriptions = new List<Guid>(); // Replace with your actual subscription type
        var expectedModuleTypes = new List<ModuleType>
                                  {
                                      ModuleType.Accounting,
                                      ModuleType.Contracts
                                  };

        subscriptionServiceMock.Setup(x => x.GetValidSubscriptionsAsync(companyId, cancellationToken)).ReturnsAsync(new ApiResponse<List<Guid>>(validSubscriptions)); // Replace object with your actual type

        subscriptionCreditRepositoryMock.Setup(x => x.GetValidModulesTypesAsync(validSubscriptions, cancellationToken)).ReturnsAsync(expectedModuleTypes);

        // Act
        var result = await sut.GetActiveModulesTypesAsync(companyId, cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedModuleTypes));

        subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, cancellationToken), Times.Once);
        subscriptionCreditRepositoryMock.Verify(x => x.GetValidModulesTypesAsync(validSubscriptions, cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetActiveModulesTypesAsyncWhenNoValidSubscriptionsReturnsFailureResponse()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var errorMessage = "No valid subscriptions found";
        var errorResponse = new ApiResponse<List<Guid>>(data: null, HttpStatusCode.NotFound, errorMessage);

        subscriptionServiceMock.Setup(x => x.GetValidSubscriptionsAsync(companyId, cancellationToken)).ReturnsAsync(errorResponse);

        // Act
        var result = await sut.GetActiveModulesTypesAsync(companyId, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Data, Is.Null);
        }

        subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, cancellationToken), Times.Once);
        subscriptionCreditRepositoryMock.Verify(x => x.GetValidModulesTypesAsync(It.IsAny<List<Guid>>(), cancellationToken), Times.Never);
    }

    [Test]
    public async Task GetActiveModulesTypesAsyncWhenRepositoryReturnsEmptyListReturnsEmptyList()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var validSubscriptions = new List<Guid>(); // Replace with your actual subscription type
        var emptyModuleTypes = new List<ModuleType>();

        subscriptionServiceMock.Setup(x => x.GetValidSubscriptionsAsync(companyId, cancellationToken)).ReturnsAsync(new ApiResponse<List<Guid>>(validSubscriptions));

        subscriptionCreditRepositoryMock.Setup(x => x.GetValidModulesTypesAsync(validSubscriptions, cancellationToken)).ReturnsAsync(emptyModuleTypes);

        // Act
        var result = await sut.GetActiveModulesTypesAsync(companyId, cancellationToken);

        // Assert
        Assert.That(result.Data, Is.Empty);

        subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, cancellationToken), Times.Once);
        subscriptionCreditRepositoryMock.Verify(x => x.GetValidModulesTypesAsync(validSubscriptions, cancellationToken), Times.Once);
    }
}
