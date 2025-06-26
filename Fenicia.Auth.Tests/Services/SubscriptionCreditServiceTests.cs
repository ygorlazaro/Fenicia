using System.Net;

using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.Subscription.Logic;
using Fenicia.Auth.Domains.SubscriptionCredit;
using Fenicia.Auth.Domains.SubscriptionCredit.Logic;
using Fenicia.Common;
using Fenicia.Common.Enums;

using Microsoft.Extensions.Logging;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class SubscriptionCreditServiceTests
{
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

        _sut = new SubscriptionCreditService(
            _loggerMock.Object,
            _subscriptionCreditRepositoryMock.Object,
            _subscriptionServiceMock.Object
        );
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

        _subscriptionServiceMock
            .Setup(x => x.GetValidSubscriptionsAsync(companyId, TODO))
            .ReturnsAsync(new ApiResponse<List<Guid>>(validSubscriptions)); // Replace object with your actual type

        _subscriptionCreditRepositoryMock
            .Setup(x => x.GetValidModulesTypesAsync(validSubscriptions))
            .ReturnsAsync(expectedModuleTypes);

        // Act
        var result = await _sut.GetActiveModulesTypesAsync(companyId, TODO);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedModuleTypes));

        _subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, TODO), Times.Once);
        _subscriptionCreditRepositoryMock.Verify(
            x => x.GetValidModulesTypesAsync(validSubscriptions),
            Times.Once
        );
    }

    [Test]
    public async Task GetActiveModulesTypesAsync_WhenNoValidSubscriptions_ReturnsFailureResponse()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var errorMessage = "No valid subscriptions found";
        var errorResponse = new ApiResponse<List<Guid>>(
            null,
            HttpStatusCode.NotFound,
            errorMessage
        );

        _subscriptionServiceMock
            .Setup(x => x.GetValidSubscriptionsAsync(companyId, TODO))
            .ReturnsAsync(errorResponse);

        // Act
        var result = await _sut.GetActiveModulesTypesAsync(companyId, TODO);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Message, Is.EqualTo(errorMessage));
            Assert.That(result.Data, Is.Null);
        });

        _subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, TODO), Times.Once);
        _subscriptionCreditRepositoryMock.Verify(
            x => x.GetValidModulesTypesAsync(It.IsAny<List<Guid>>()),
            Times.Never
        );
    }

    [Test]
    public async Task GetActiveModulesTypesAsync_WhenRepositoryReturnsEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var validSubscriptions = new List<Guid>(); // Replace with your actual subscription type
        var emptyModuleTypes = new List<ModuleType>();

        _subscriptionServiceMock
            .Setup(x => x.GetValidSubscriptionsAsync(companyId, TODO))
            .ReturnsAsync(new ApiResponse<List<Guid>>(validSubscriptions));

        _subscriptionCreditRepositoryMock
            .Setup(x => x.GetValidModulesTypesAsync(validSubscriptions))
            .ReturnsAsync(emptyModuleTypes);

        // Act
        var result = await _sut.GetActiveModulesTypesAsync(companyId, TODO);

        // Assert
        Assert.That(result.Data, Is.Empty);

        _subscriptionServiceMock.Verify(x => x.GetValidSubscriptionsAsync(companyId, TODO), Times.Once);
        _subscriptionCreditRepositoryMock.Verify(
            x => x.GetValidModulesTypesAsync(validSubscriptions),
            Times.Once
        );
    }
}
