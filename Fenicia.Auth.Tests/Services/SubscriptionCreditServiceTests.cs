namespace Fenicia.Auth.Tests.Services;

using System.Net;

using Common;
using Common.Enums;

using Domains.Subscription;
using Domains.SubscriptionCredit;

using Microsoft.Extensions.Logging;

using Moq;

public class SubscriptionCreditServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Mock<ILogger<SubscriptionCreditService>> loggerMock;
    private Mock<ISubscriptionCreditRepository> subscriptionCreditRepositoryMock;
    private Mock<ISubscriptionService> subscriptionServiceMock;
    private SubscriptionCreditService sut;

    [SetUp]
    public void Setup()
    {
        this.loggerMock = new Mock<ILogger<SubscriptionCreditService>>();
        this.subscriptionCreditRepositoryMock = new Mock<ISubscriptionCreditRepository>();
        this.subscriptionServiceMock = new Mock<ISubscriptionService>();

        this.sut = new SubscriptionCreditService(this.loggerMock.Object, this.subscriptionCreditRepositoryMock.Object, this.subscriptionServiceMock.Object);
    }

    [Test]
    public async Task GetActiveModulesTypesAsync_WhenValidSubscriptionsExist_ReturnsModuleTypes()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var validSubscriptions = new List<Guid>(); // Replace with your actual subscription type
        var expectedModuleTypes = new List<ModuleType>
                                  {
                                      ModuleType.Accounting,
                                      ModuleType.Contracts
                                  };

        this.subscriptionServiceMock.Setup(x => x.GetValidSubscriptionsAsync(companyId, this.cancellationToken)).ReturnsAsync(new ApiResponse<List<Guid>>(validSubscriptions)); // Replace object with your actual type

        this.subscriptionCreditRepositoryMock.Setup(x => x.GetValidModulesTypesAsync(validSubscriptions, this.cancellationToken)).ReturnsAsync(expectedModuleTypes);

        // Act
        var result = await this.sut.GetActiveModulesTypesAsync(companyId, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedModuleTypes));

        this.subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, this.cancellationToken), Times.Once);
        this.subscriptionCreditRepositoryMock.Verify(x => x.GetValidModulesTypesAsync(validSubscriptions, this.cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetActiveModulesTypesAsync_WhenNoValidSubscriptions_ReturnsFailureResponse()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var errorMessage = "No valid subscriptions found";
        var errorResponse = new ApiResponse<List<Guid>>(data: null, HttpStatusCode.NotFound, errorMessage);

        this.subscriptionServiceMock.Setup(x => x.GetValidSubscriptionsAsync(companyId, this.cancellationToken)).ReturnsAsync(errorResponse);

        // Act
        var result = await this.sut.GetActiveModulesTypesAsync(companyId, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Data, Is.Null);
        }

        this.subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, this.cancellationToken), Times.Once);
        this.subscriptionCreditRepositoryMock.Verify(x => x.GetValidModulesTypesAsync(It.IsAny<List<Guid>>(), this.cancellationToken), Times.Never);
    }

    [Test]
    public async Task GetActiveModulesTypesAsync_WhenRepositoryReturnsEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var validSubscriptions = new List<Guid>(); // Replace with your actual subscription type
        var emptyModuleTypes = new List<ModuleType>();

        this.subscriptionServiceMock.Setup(x => x.GetValidSubscriptionsAsync(companyId, this.cancellationToken)).ReturnsAsync(new ApiResponse<List<Guid>>(validSubscriptions));

        this.subscriptionCreditRepositoryMock.Setup(x => x.GetValidModulesTypesAsync(validSubscriptions, this.cancellationToken)).ReturnsAsync(emptyModuleTypes);

        // Act
        var result = await this.sut.GetActiveModulesTypesAsync(companyId, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.Empty);

        this.subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, this.cancellationToken), Times.Once);
        this.subscriptionCreditRepositoryMock.Verify(x => x.GetValidModulesTypesAsync(validSubscriptions, this.cancellationToken), Times.Once);
    }
}
