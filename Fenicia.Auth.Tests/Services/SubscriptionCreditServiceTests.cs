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
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private Mock<ILogger<SubscriptionCreditService>> _loggerMock;
    private Mock<ISubscriptionCreditRepository> _subscriptionCreditRepositoryMock;
    private Mock<ISubscriptionService> _subscriptionServiceMock;
    private SubscriptionCreditService _sut;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<SubscriptionCreditService>>();
        _subscriptionCreditRepositoryMock = new Mock<ISubscriptionCreditRepository>();
        _subscriptionServiceMock = new Mock<ISubscriptionService>();

        _sut = new SubscriptionCreditService(_loggerMock.Object, _subscriptionCreditRepositoryMock.Object, _subscriptionServiceMock.Object);
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

        _subscriptionServiceMock.Setup(x => x.GetValidSubscriptionsAsync(companyId, _cancellationToken)).ReturnsAsync(new ApiResponse<List<Guid>>(validSubscriptions)); // Replace object with your actual type

        _subscriptionCreditRepositoryMock.Setup(x => x.GetValidModulesTypesAsync(validSubscriptions, _cancellationToken)).ReturnsAsync(expectedModuleTypes);

        // Act
        var result = await _sut.GetActiveModulesTypesAsync(companyId, _cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedModuleTypes));

        _subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, _cancellationToken), Times.Once);
        _subscriptionCreditRepositoryMock.Verify(x => x.GetValidModulesTypesAsync(validSubscriptions, _cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetActiveModulesTypesAsync_WhenNoValidSubscriptions_ReturnsFailureResponse()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var errorMessage = "No valid subscriptions found";
        var errorResponse = new ApiResponse<List<Guid>>(data: null, HttpStatusCode.NotFound, errorMessage);

        _subscriptionServiceMock.Setup(x => x.GetValidSubscriptionsAsync(companyId, _cancellationToken)).ReturnsAsync(errorResponse);

        // Act
        var result = await _sut.GetActiveModulesTypesAsync(companyId, _cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Data, Is.Null);
        }

        _subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, _cancellationToken), Times.Once);
        _subscriptionCreditRepositoryMock.Verify(x => x.GetValidModulesTypesAsync(It.IsAny<List<Guid>>(), _cancellationToken), Times.Never);
    }

    [Test]
    public async Task GetActiveModulesTypesAsync_WhenRepositoryReturnsEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var validSubscriptions = new List<Guid>(); // Replace with your actual subscription type
        var emptyModuleTypes = new List<ModuleType>();

        _subscriptionServiceMock.Setup(x => x.GetValidSubscriptionsAsync(companyId, _cancellationToken)).ReturnsAsync(new ApiResponse<List<Guid>>(validSubscriptions));

        _subscriptionCreditRepositoryMock.Setup(x => x.GetValidModulesTypesAsync(validSubscriptions, _cancellationToken)).ReturnsAsync(emptyModuleTypes);

        // Act
        var result = await _sut.GetActiveModulesTypesAsync(companyId, _cancellationToken);

        // Assert
        Assert.That(result.Data, Is.Empty);

        _subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, _cancellationToken), Times.Once);
        _subscriptionCreditRepositoryMock.Verify(x => x.GetValidModulesTypesAsync(validSubscriptions, _cancellationToken), Times.Once);
    }
}
