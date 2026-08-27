using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.Security.DTOs.Commands;
using Fenicia.Common.Exceptions;

namespace Fenicia.Auth.Tests.Domains.Security;

public class HashStringHandlerTests
{
    [Fact]
    public async Task Handle_WhenPasswordIsValid_ReturnsHashedPassword()
    {
        var result = SecurityService.Hash("MyPassword123");
        Assert.NotNull(result);
        Assert.NotEqual("MyPassword123", result);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsNull_ThrowsInvalidRequestException()
    {
        await Assert.ThrowsAsync<InvalidRequestException>(() => Task.FromResult(SecurityService.Hash(null!)));
    }

    [Fact]
    public async Task Handle_WhenPasswordIsEmpty_ThrowsInvalidRequestException()
    {
        await Assert.ThrowsAsync<InvalidRequestException>(() => Task.FromResult(SecurityService.Hash(string.Empty)));
    }
}
