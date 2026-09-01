using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Subscription;

public class SubscriptionServiceTests
{
    private readonly Mock<ISubscriptionRepository> _mockRepository;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IUserRoleService> _mockUserRoleService;
    private readonly SubscriptionService _service;

    public SubscriptionServiceTests()
    {
        _mockRepository = new Mock<ISubscriptionRepository>();
        _mockUserService = new Mock<IUserService>();
        _mockUserRoleService = new Mock<IUserRoleService>();
        _service = new SubscriptionService(_mockRepository.Object, _mockUserService.Object, _mockUserRoleService.Object);
    }

    [Fact]
    public async Task GetUserProfileAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        var nonExistentUserId = Guid.NewGuid();

        _mockUserService.Setup(s => s.GetByIdAsync(nonExistentUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetUserByIdResponse)null!);

        var result = await _service.GetUserProfileAsync(nonExistentUserId, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserProfileAsync_WhenUserExists_ReturnsProfileWithEmptyCollections()
    {
        var userId = Guid.NewGuid();
        var user = new UserModel
        {
            Id = userId,
            Name = "Test User",
            Email = "test@example.com",
            Password = new SecurityService().Hash("password123"),
            Created = DateTime.UtcNow
        };

        _mockUserService.Setup(s => s.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserByIdResponse(user.Id, user.Name, user.Email));
        _mockUserRoleService.Setup(s => s.GetUserRoleModelsByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _mockRepository.Setup(r => r.GetUserSubscriptionsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _service.GetUserProfileAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("Test User", result.Name);
        Assert.Equal("test@example.com", result.Email);
        Assert.Empty(result.Companies);
        Assert.Empty(result.Subscriptions);
    }
}
