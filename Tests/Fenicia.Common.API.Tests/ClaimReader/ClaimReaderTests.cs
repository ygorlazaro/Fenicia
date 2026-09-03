using System.Security.Claims;
using AwesomeAssertions;

namespace Fenicia.Common.API.Tests.ClaimReader;

public class ClaimReaderTests
{
    [Fact]
    public void UserId_ShouldReturnGuid_WhenClaimExists()
    {
        var userId = Guid.NewGuid();
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

        var result = API.ClaimReader.UserId(principal);

        result.Should().Be(userId);
    }

    [Fact]
    public void UserId_ShouldThrowUnauthorizedAccessException_WhenClaimDoesNotExist()
    {
        var claims = new List<Claim> { new("name", "Test") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

        Action act = () => API.ClaimReader.UserId(principal);

        act.Should().Throw<UnauthorizedAccessException>();
    }

    [Fact]
    public void UserId_ShouldThrowUnauthorizedAccessException_WhenClaimsAreEmpty()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity([]));

        Action act = () => API.ClaimReader.UserId(principal);

        act.Should().Throw<UnauthorizedAccessException>();
    }
}