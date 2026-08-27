using Fenicia.Auth.Domains.Security.Command;
using Fenicia.Auth.Domains.Security.Handler;
using Fenicia.Common.Exceptions;

namespace Fenicia.Auth.Tests.Domains.Security;

public class HashStringHandlerTests
{
    private readonly HashStringHandler handler = new();

    [Fact]
    public async Task Handle_WhenPasswordIsValid_ReturnsHashedPassword()
    {
        var result = await handler.Handle(new HashStringCommand("MyPassword123"), CancellationToken.None);
        Assert.NotNull(result);
        Assert.NotEqual("MyPassword123", result);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsNull_ThrowsInvalidRequestException()
    {
        await Assert.ThrowsAsync<InvalidRequestException>(() => handler.Handle(new HashStringCommand(null!), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenPasswordIsEmpty_ThrowsInvalidRequestException()
    {
        await Assert.ThrowsAsync<InvalidRequestException>(() => handler.Handle(new HashStringCommand(string.Empty), CancellationToken.None));
    }
}
