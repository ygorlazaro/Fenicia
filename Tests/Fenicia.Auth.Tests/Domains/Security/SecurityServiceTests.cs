using Fenicia.Auth.Domains.Security;
using Fenicia.Common.Exceptions;

namespace Fenicia.Auth.Tests.Domains.Security;

public class SecurityServiceTests
{
    [Fact]
    public void Hash_WhenPasswordIsValid_ReturnsHashedPassword()
    {
        var service = new SecurityService();
        var result = service.Hash("MyPassword123");

        Assert.NotNull(result);
        Assert.NotEqual("MyPassword123", result);
    }

    [Fact]
    public void Hash_WhenPasswordIsNull_ThrowsInvalidRequestException()
    {
        var service = new SecurityService();

        Assert.Throws<InvalidRequestException>(() => service.Hash(null!));
    }

    [Fact]
    public void Hash_WhenPasswordIsEmpty_ThrowsInvalidRequestException()
    {
        var service = new SecurityService();

        Assert.Throws<InvalidRequestException>(() => service.Hash(string.Empty));
    }
}
