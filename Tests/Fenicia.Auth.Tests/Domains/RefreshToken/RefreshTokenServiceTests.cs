using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.RefreshToken.DTOs;
using Fenicia.Common.Exceptions;

using Moq;

namespace Fenicia.Auth.Tests.Domains.RefreshToken;

public class RefreshTokenServiceTests
{
    private readonly Mock<IRefreshTokenRepository> _mockRepository;
    private readonly RefreshTokenService _service;

    public RefreshTokenServiceTests()
    {
        _mockRepository = new Mock<IRefreshTokenRepository>();
        _service = new RefreshTokenService(_mockRepository.Object);
    }

    [Fact]
    public async Task GenerateAsync_GeneratesValidRefreshToken()
    {
        var userId = Guid.NewGuid();

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<RefreshTokenModel>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.GenerateAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(32, Convert.FromBase64String(result).Length);
    }

    [Fact]
    public async Task GenerateAsync_GeneratesUniqueTokensForEachCall()
    {
        var userId = Guid.NewGuid();

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<RefreshTokenModel>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var token1 = await _service.GenerateAsync(userId, CancellationToken.None);
        var token2 = await _service.GenerateAsync(userId, CancellationToken.None);
        var token3 = await _service.GenerateAsync(userId, CancellationToken.None);

        Assert.NotEqual(token1, token2);
        Assert.NotEqual(token2, token3);
        Assert.NotEqual(token1, token3);
    }

    [Fact]
    public async Task GenerateAsync_ForDifferentUsers_GeneratesDifferentTokens()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<RefreshTokenModel>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var token1 = await _service.GenerateAsync(userId1, CancellationToken.None);
        var token2 = await _service.GenerateAsync(userId2, CancellationToken.None);

        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public async Task GenerateAsync_MultipleTokensForSameUser_AreUnique()
    {
        var userId = Guid.NewGuid();
        var tokens = new List<string>();

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<RefreshTokenModel>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        for (var i = 0; i < 10; i++)
        {
            tokens.Add(await _service.GenerateAsync(userId, CancellationToken.None));
        }

        var distinctTokens = tokens.Distinct().ToList();
        Assert.Equal(10, distinctTokens.Count);
    }

    [Fact]
    public async Task InvalidateAsync_WhenTokenExists_SetsIsActiveToFalse()
    {
        const string refreshToken = "valid_refresh_token";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(5);

        var tokenResponse = new RefreshTokenModel(refreshToken, expirationDate, userId) { IsActive = true };

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(tokenResponse);

        await _service.InvalidateAsync(refreshToken, CancellationToken.None);

        var updatedToken = tokenResponse with { IsActive = false };
        _mockRepository.Verify(r => r.UpdateAsync(updatedToken, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InvalidateAsync_WhenTokenDoesNotExist_ReturnsSilently()
    {
        const string refreshToken = "non_existent_token";

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync((RefreshTokenModel?)null);

        await _service.InvalidateAsync(refreshToken, CancellationToken.None);

        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<RefreshTokenModel>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InvalidateAsync_WhenTokenIsNull_ThrowsArgumentNullException()
    {
        string? refreshToken = null;

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.InvalidateAsync(refreshToken!, CancellationToken.None));
    }

    [Fact]
    public async Task InvalidateAsync_WhenTokenIsEmptyString_ReturnsSilently()
    {
        var refreshToken = string.Empty;

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync((RefreshTokenModel?)null);

        await _service.InvalidateAsync(refreshToken, CancellationToken.None);

        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<RefreshTokenModel>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InvalidateAsync_WhenTokenIsAlreadyInactive_StillUpdates()
    {
        const string refreshToken = "already_inactive_token";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(5);

        var tokenResponse = new RefreshTokenModel(refreshToken, expirationDate, userId) { IsActive = false };

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(tokenResponse);

        await _service.InvalidateAsync(refreshToken, CancellationToken.None);

        _mockRepository.Verify(r => r.UpdateAsync(tokenResponse, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InvalidateAsync_WhenTokenIsExpired_StillInvalidates()
    {
        const string refreshToken = "expired_token";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(-1);

        var tokenResponse = new RefreshTokenModel(refreshToken, expirationDate, userId) { IsActive = true };

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(tokenResponse);

        await _service.InvalidateAsync(refreshToken, CancellationToken.None);

        var updatedToken = tokenResponse with { IsActive = false };
        _mockRepository.Verify(r => r.UpdateAsync(updatedToken, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InvalidateAsync_PreservesOtherTokenProperties()
    {
        const string refreshToken = "token_to_invalidate";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(5);

        var tokenResponse = new RefreshTokenModel(refreshToken, expirationDate, userId) { IsActive = true };

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(tokenResponse);

        await _service.InvalidateAsync(refreshToken, CancellationToken.None);

        var updatedToken = tokenResponse with { IsActive = false };
        _mockRepository.Verify(r => r.UpdateAsync(updatedToken, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InvalidateAsync_WhenMalformedJsonInRedis_ReturnsSilently()
    {
        const string refreshToken = "malformed_token";

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync((RefreshTokenModel?)null);

        await _service.InvalidateAsync(refreshToken, CancellationToken.None);

        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<RefreshTokenModel>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InvalidateAsync_MultipleInvalidationsForSameToken_WorksCorrectly()
    {
        const string refreshToken = "multi_invalidate_token";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(5);

        var tokenResponse = new RefreshTokenModel(refreshToken, expirationDate, userId) { IsActive = true };

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(tokenResponse);

        await _service.InvalidateAsync(refreshToken, CancellationToken.None);
        await _service.InvalidateAsync(refreshToken, CancellationToken.None);

        var updatedToken = tokenResponse with { IsActive = false };
        _mockRepository.Verify(r => r.UpdateAsync(updatedToken, It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task InvalidateAsync_VerifiesCorrectTTLIsSet()
    {
        const string refreshToken = "token_with_ttl";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(5);

        var tokenResponse = new RefreshTokenModel(refreshToken, expirationDate, userId) { IsActive = true };

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(tokenResponse);

        await _service.InvalidateAsync(refreshToken, CancellationToken.None);

        var updatedToken = tokenResponse with { IsActive = false };
        _mockRepository.Verify(r => r.UpdateAsync(updatedToken, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InvalidateAsync_WhenJsonDeserializationFails_ReturnsSilently()
    {
        const string refreshToken = "bad_json_token";

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync((RefreshTokenModel?)null);

        await _service.InvalidateAsync(refreshToken, CancellationToken.None);

        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<RefreshTokenModel>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenIsValidAndActive_ReturnsTrue()
    {
        var userId = Guid.NewGuid();
        const string refreshToken = "valid_refresh_token";

        var tokenResponse = new RefreshTokenModel(refreshToken, DateTime.UtcNow.AddDays(5), userId);

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(tokenResponse);

        var result = await _service.ValidateAsync(userId, refreshToken, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenDoesNotExistInRedis_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        const string refreshToken = "non_existent_token";

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync((RefreshTokenModel?)null);

        var result = await _service.ValidateAsync(userId, refreshToken, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenIsInactive_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        const string refreshToken = "inactive_token";

        var tokenResponse = new RefreshTokenModel(refreshToken, DateTime.UtcNow.AddDays(5), userId, false);

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(tokenResponse);

        var result = await _service.ValidateAsync(userId, refreshToken, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenIsExpired_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        const string refreshToken = "expired_token";

        var tokenResponse = new RefreshTokenModel(refreshToken, DateTime.UtcNow.AddDays(-1), userId);

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(tokenResponse);

        var result = await _service.ValidateAsync(userId, refreshToken, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenBelongsToDifferentUser_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        const string refreshToken = "wrong_user_token";

        var tokenResponse = new RefreshTokenModel(refreshToken, DateTime.UtcNow.AddDays(5), differentUserId);

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(tokenResponse);

        var result = await _service.ValidateAsync(userId, refreshToken, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenRefreshTokenIsNull_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var query = new ValidateTokenQuery(userId, null!);

        await Assert.ThrowsAsync<InvalidRequestException>(async () => await _service.ValidateAsync(query.UserId, query.RefreshToken, CancellationToken.None));
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenIsExpiringSoon_ReturnsTrue()
    {
        var userId = Guid.NewGuid();
        const string refreshToken = "expiring_soon_token";

        var tokenResponse = new RefreshTokenModel(refreshToken, DateTime.UtcNow.AddHours(1), userId);

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(tokenResponse);

        var result = await _service.ValidateAsync(userId, refreshToken, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenHasExactlyCurrentExpirationTime_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        const string refreshToken = "exact_expiration_token";

        var tokenResponse = new RefreshTokenModel(refreshToken, DateTime.UtcNow, userId);

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(tokenResponse);

        var result = await _service.ValidateAsync(userId, refreshToken, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenMalformedJsonInRedis_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        const string refreshToken = "malformed_token";

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync((RefreshTokenModel?)null);

        var result = await _service.ValidateAsync(userId, refreshToken, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenRefreshTokenIsWhitespace_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var query = new ValidateTokenQuery(userId, "   ");

        await Assert.ThrowsAsync<InvalidRequestException>(async () => await _service.ValidateAsync(query.UserId, query.RefreshToken, CancellationToken.None));
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenIsNullAfterDeserialization_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        const string refreshToken = "null_deserialize_token";

        _mockRepository.Setup(r => r.GetAsync(refreshToken, It.IsAny<CancellationToken>())).ReturnsAsync((RefreshTokenModel?)null);

        var result = await _service.ValidateAsync(userId, refreshToken, CancellationToken.None);

        Assert.False(result);
    }
}
